using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using TaskManager.API.Configurations;
using TaskManager.API.Models;

namespace TaskManager.API.Repositories
{
    public class TaskRepository : ITaskRepository // Repository - Sadece database sorgusu
    {
        private readonly IMongoCollection<Models.TaskItem> _collection; // MongoDB Tasks collection referansı

        public TaskRepository() // Constructor - Collection bağlantısını kurar
        {
            _collection = DatabaseSetup.GetTasksCollection();
        }

        // Yeni görev ekleme işlemi
        // MongoDB'ye yeni bir TaskItem kaydeder ve eklenen nesneyi geri döndürür
        public async Task<Models.TaskItem?> Insert(Models.TaskItem task)
        {
            try
            {
                await _collection.InsertOneAsync(task); // MongoDB'ye görev ekleme
                Console.WriteLine($"Repository: Görev başarıyla kaydedildi - ID: {task.Id}");
                return task;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository Insert hatası: {ex.Message}");
                return null;
            }
        }

        // Tüm görevleri getirme işlemi
        // Sistemdeki tüm TaskItem'ları liste halinde döndürür (test amaçlı)
        public async Task<List<Models.TaskItem>?> GetAll()
        {
            try
            {
                var tasks = await _collection.Find(_ => true).ToListAsync(); // Tüm kayıtları getir
                return tasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository GetAll hatası: {ex.Message}");
                return null;
            }
        }

        // Belirli kullanıcının tüm görevlerini getirme
        // userId parametresi ile eşleşen tüm TaskItem'ları liste halinde döndürür
        public async Task<List<Models.TaskItem>?> GetByUserId(string userId)
        {
            try
            {
                var tasks = await _collection.Find(x => x.user_id == userId).ToListAsync();
                Console.WriteLine($"Repository: {tasks.Count} görev bulundu - User ID: {userId}");
                return tasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository GetByUserId hatası: {ex.Message}");
                return null;
            }
        }

        // Belirli bir kullanıcıya ait id si verilen bir görevi getirir
        // taskId ve userId'nin her ikisi de eşleşen tek bir TaskItem döndürür
        public async Task<Models.TaskItem?> GetByIdAndUserId(string taskId, string userId)
        {
            try
            {
                var objectId = ObjectId.Parse(taskId); // String ID'yi MongoDB ObjectId'ye çevir
                var task = await _collection.Find(x => x.user_id == userId && x.Id == objectId).FirstOrDefaultAsync();
                return task;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository GetByIdAndUserId hatası: {ex.Message}");
                return null;
            }
        }

        // Belirli kullanıcıya ait bir görevi siler
        // taskId ve userId eşleşen kaydı MongoDB'den siler, başarı durumunu döndürür
        public async Task<bool> DeleteByUserId(string taskId, string userId)
        {
            try
            {
                var objectId = ObjectId.Parse(taskId);
                var deleteResult = await _collection.DeleteOneAsync(t => t.Id == objectId && t.user_id == userId);
                return deleteResult.DeletedCount > 0;//yapılan işlem sayısı < ve > e göre true false dönertrue-false döner
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository DeleteByUserId hatası: {ex.Message}");
                return false;
            }
        }

        // Görevi tamamen günceller (PUT işlemi)
        // Mevcut TaskItem'ı tamamen yeni verilerle değiştirir, bazı alanları korur
        public async Task<bool?> UpdateTask(string taskId, string userId, Models.TaskItem updatedTask)
        {
            try
            {
                var objectId = ObjectId.Parse(taskId);
                var task = await _collection.Find(x => x.Id == objectId && x.user_id == userId).FirstOrDefaultAsync();//Güncelenecek task i bul
                if (task == null)
                {
                    return false;
                }

                //task i güclerken id-oluşturma tarihi-userid-update_date verebilmek için
                // Güncellenecek task'ı hazırla - kritik alanları koru
                updatedTask.Id = objectId;
                updatedTask.user_id = userId;
                updatedTask.created_at = task.created_at; // Orijinal oluşturma tarihini koru
                updatedTask.update_date = DateTime.Now; // Güncelleme zamanını şimdi yap

                var result = await _collection.ReplaceOneAsync(x => x.Id == objectId && x.user_id == userId, updatedTask);
                Console.WriteLine($"Repository: Task güncellendi - ID: {taskId}, Modified: {result.ModifiedCount}");
                return result.ModifiedCount > 0;//yapılan işlem sayısı < ve > e göre true false dönertrue-false döner
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository UpdateTask hatası: {ex.Message}");
                return false;
            }
        }

        // Görevin sadece belirli alanlarını günceller (PATCH işlemi)
        // Sadece gönderilen alanları günceller, diğer alanlar değişmez
        public async Task<bool?> PatchTask(string taskId, string userId, Dictionary<string, object> updates)
        {
            try
            {
                var objectId = ObjectId.Parse(taskId);
                var task = await _collection.Find(x => x.Id == objectId && x.user_id == userId).FirstOrDefaultAsync();//Güncelenecek task i bul
                if (task == null)
                {
                    return false;
                }
                var updatebuilder = Builders<Models.TaskItem>.Update;//Models.TaskItem sınıfının özellikleri üzerinde lambda ifadeleriyle güncelleme yapmanızı sağlar
                //Ne yapar? MongoDB için güncelleme komutları oluşturan bir "fabrika" objesi oluşturur
                var updates_list = new List<UpdateDefinition<Models.TaskItem>>();//Birden fazla alanı aynı işlemde güncellemek için         

                // Gönderilen her alan için uygun güncelleme komutunu oluştur
                foreach (var item in updates)
                {
                    if (item.Key == "title" && item.Value is string titleStr && !string.IsNullOrEmpty(titleStr))
                    {
                        updates_list.Add(updatebuilder.Set(x => x.title, titleStr)); // item.Value'yu titleStr olarak kullan
                    }
                    else if (item.Key == "description" && item.Value is string descStr)
                    {
                        updates_list.Add(updatebuilder.Set(x => x.description, descStr));
                    }
                    else if (item.Key == "priority" && item.Value is int priorityValue)
                    {
                        updates_list.Add(updatebuilder.Set(x => x.priority, priorityValue));
                    }
                    else if (item.Key == "status" && item.Value is string statusStr && !string.IsNullOrEmpty(statusStr))
                    {
                        updates_list.Add(updatebuilder.Set(x => x.status, statusStr));
                    }
                }

                updates_list.Add(updatebuilder.Set(x => x.update_date, DateTime.Now)); // Her zaman güncelleme tarihi ekle
                var updateitem = updatebuilder.Combine(updates_list); // Tüm güncellemeleri birleştir
                var result = await _collection.UpdateOneAsync(x => x.Id == objectId && x.user_id == userId, updateitem);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository PatchTask hatası: {ex.Message}");
                return false;
            }
        }

        // Kullanıcıya ait, belirli döneme göre görevleri getirir (örneğin: daily, weekly, monthly)
        // userId ve period parametrelerine göre filtrelenmiş TaskItem listesi döndürür
        public async Task<List<TaskItem>> GetTasksByPeriod(string userId, string period)
        {
            try
            {
                var tasks = await _collection.Find(x => x.user_id == userId && x.period == period).ToListAsync();
                return tasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository GetTasksByPeriod hatası: {ex.Message}");
                return new List<Models.TaskItem>();
            }
        }
    }
}