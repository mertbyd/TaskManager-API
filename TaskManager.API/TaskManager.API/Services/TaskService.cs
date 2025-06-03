using MongoDB.Bson;
using TaskManager.API.Repositories;
using TaskManager.API.Services;
namespace TaskManager.API.Services
{
    public class TaskService : ITaskService// Service - İş kuralları ve kontroller
    {
        private readonly ITaskRepository _taskRepository; // Task repository dependency
        public TaskService(ITaskRepository taskRepository, IJwtService jwtService) // Dependency injection constructor
        {
            _taskRepository = taskRepository;
            _jwtService = jwtService;
        }
        IJwtService _jwtService;

        public async Task<Models.TaskItem?> TaskInsert(DTOs.DTOs.CreateTaskDto taskDto, string userId) // Yeni görev oluşturma işlemi
        {
            try
            {
                var validPeriods = new[] { "daily", "weekly", "monthly" };

                // Period değerinin geçerli olup olmadığını kontrol et
                if (!validPeriods.Contains(taskDto.period.ToLower()))//eğer içinde yoksa false döner
                {
                    Console.WriteLine($"Service: Geçersiz period değeri - {taskDto.period}");
                    return null;
                }
                // DTO'yu Model'e çevirme
                var task = new Models.TaskItem
                {
                    title = taskDto.title,
                    description = taskDto.description,
                    priority = taskDto.priority,
                    created_at = DateTime.Now,
                    status = "Pending",
                    user_id = userId,
                    period = taskDto.period
                    // user_id JWT'den alınacak
                };
                var result = await _taskRepository.Insert(task); // Repository'ye gönderme
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TaskService Insert hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Models.TaskItem>?> TasktGetAll() // Tüm görevleri getirme işlemi
        {
            try
            {
                var tasks = await _taskRepository.GetAll(); // Repository'den veri çekme
                return tasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TaskService GetAll hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Models.TaskItem>?> TaskGetAllbyId(string userid)// User'a özel task'ları getir
        {
            try
            {
                var tasks = await _taskRepository.GetByUserId(userid); // Repository'den veri çekme
                return tasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TaskService GetAll hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<Models.TaskItem?> GetTaskById(string taskId, string userId) // ID'ye göre belirli bir görevi getirme
        {
            try
            {
                // TaskId ve UserId'nin boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(taskId) || string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("Service: TaskId veya UserId boş");
                    return null;
                }
                var result = await _taskRepository.GetByIdAndUserId(taskId, userId);

                // Görevin bulunup bulunmadığını kontrol et
                if (result == null)
                {
                    Console.WriteLine($"Service: Task bulunamadı - ID: {taskId}, User: {userId}");
                }
                else
                {
                    Console.WriteLine($"Service: Task başarıyla getirildi - ID: {taskId}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service GetTaskById hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteTask(string taskId, string userId) // Görevi silme işlemi
        {
            try
            {
                // TaskId ve UserId'nin boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(taskId) || string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("Service: TaskId veya UserId boş");
                    return false;
                }
                // Repository'ye gönder
                var result = await _taskRepository.DeleteByUserId(taskId, userId);

                // Silme işleminin başarılı olup olmadığını kontrol et
                if (result)
                {
                    Console.WriteLine($"TaskService: Task başarıyla silindi - ID: {taskId}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"TaskService: Task silinemedi - ID: {taskId}, User: {userId}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TaskService DeleteTask hatası: {ex.Message}");
                return false;
            }
        }

        public async Task<Models.TaskItem?> UpdateTask(string taskId, string userId, DTOs.DTOs.UpdateTaskDto updateDto) // Görevi tamamen güncelleme işlemi
        {
            try
            {
                // TaskId ve UserId'nin boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(taskId) || string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("Service: TaskId veya UserId boş");
                    return null;
                }

                // UpdateDto'nun null olup olmadığını kontrol et
                if (updateDto == null)
                {
                    Console.WriteLine("Service: UpdateDto null");
                    return null;
                }

                // Title'ın boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(updateDto.title))
                {
                    Console.WriteLine("Service: Title boş olamaz");
                    return null;
                }

                // Priority değerinin geçerli aralıkta olup olmadığını kontrol et
                if (updateDto.priority < 1 || updateDto.priority > 3)
                {
                    Console.WriteLine("Service: Priority 1-3 arası olmalı");
                    return null;
                }

                // Task'ın var olup olmadığını kontrol et
                var existingTask = await _taskRepository.GetByIdAndUserId(taskId, userId);// Task i çağır
                if (existingTask == null)
                {
                    Console.WriteLine($"Service: Task bulunamadı - ID: {taskId}, User: {userId}");
                    return null;
                }
                // DTO'yu Model'e çevir
                var updatedTask = new Models.TaskItem
                {
                    title = updateDto.title,
                    description = updateDto.description,
                    priority = updateDto.priority,
                    status = string.IsNullOrEmpty(updateDto.status) ? existingTask.status : updateDto.status
                };
                // Repository'ye gönder (bool döndürür)
                var isUpdated = await _taskRepository.UpdateTask(taskId, userId, updatedTask);

                // Güncelleme işleminin başarılı olup olmadığını kontrol et
                if (isUpdated == true)
                {
                    Console.WriteLine($"Service: Task başarıyla güncellendi - ID: {taskId}");
                    var updatedTaskResult = await _taskRepository.GetByIdAndUserId(taskId, userId);
                    return updatedTaskResult;
                }
                else
                {
                    Console.WriteLine($"Service: Task güncellenemedi - ID: {taskId}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service UpdateTask hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<Models.TaskItem?> PatchTask(string taskId, string userId, DTOs.DTOs.PatchTaskDto patchDto) // Görevi kısmi güncelleme işlemi
        {
            try
            {
                // TaskId ve UserId'nin boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(taskId) || string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("Service: TaskId veya UserId boş");
                    return null;
                }

                // En az bir güncellenecek alanın olup olmadığını kontrol et
                if (patchDto == null ||
                    (patchDto.title == null &&
                     patchDto.description == null &&
                     patchDto.priority == null &&
                     patchDto.status == null))
                {
                    Console.WriteLine("Service: Güncellenecek alan yok");
                    return null;
                }
                // Güncellenecek alanları Dictionary'e ekle
                var updates = new Dictionary<string, object>();

                // Title güncellenecek mi kontrol et
                if (patchDto.title != null)
                {
                    // Title'ın boş olup olmadığını kontrol et
                    if (string.IsNullOrEmpty(patchDto.title))
                    {
                        Console.WriteLine("Service: Title boş olamaz");
                        return null;
                    }
                    updates.Add("title", patchDto.title);
                }

                // Description güncellenecek mi kontrol et
                if (patchDto.description != null)
                {
                    updates.Add("description", patchDto.description);
                }

                // Priority güncellenecek mi kontrol et
                if (patchDto.priority != null)
                {
                    // Priority değerinin geçerli aralıkta olup olmadığını kontrol et
                    if (patchDto.priority < 1 || patchDto.priority > 3)
                    {
                        Console.WriteLine("Service: Priority 1-3 arası olmalı");
                        return null;
                    }
                    updates.Add("priority", patchDto.priority.Value);
                }

                // Status güncellenecek mi kontrol et
                if (patchDto.status != null)
                {
                    var validStatuses = new[] { "Pending", "InProgress", "Completed" };

                    // Status değerinin geçerli olup olmadığını kontrol et
                    if (!validStatuses.Contains(patchDto.status))
                    {
                        Console.WriteLine($"Service: Geçersiz status - {patchDto.status}");
                        return null;
                    }
                    updates.Add("status", patchDto.status);
                }
                // Repository'ye gönder
                var isUpdated = await _taskRepository.PatchTask(taskId, userId, updates);

                // Güncelleme işleminin başarılı olup olmadığını kontrol et
                if (isUpdated == true)
                {
                    Console.WriteLine($"Service: Task başarıyla yamalandı - ID: {taskId}");
                    // Güncellenmiş task'ı getir
                    return await _taskRepository.GetByIdAndUserId(taskId, userId);
                }
                else
                {
                    Console.WriteLine($"Service: Task yamalanırken hata - ID: {taskId}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service PatchTask hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Models.TaskItem>?> GetTasksByPeriod(string userId, string period) // Periyoda göre görevleri getirme işlemi
        {
            try
            {
                // UserId'nin boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("Service: UserId boş");
                    return null;
                }
                var validPeriods = new[] { "daily", "weekly", "monthly" };

                // Period değerinin geçerli olup olmadığını kontrol et
                if (!validPeriods.Contains(period.ToLower()))
                {
                    Console.WriteLine($"Service: Geçersiz period - {period}");
                    return null;
                }
                return await _taskRepository.GetTasksByPeriod(userId, period.ToLower());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service GetTasksByPeriod hatası: {ex.Message}");
                return null;
            }
        }
    }
}