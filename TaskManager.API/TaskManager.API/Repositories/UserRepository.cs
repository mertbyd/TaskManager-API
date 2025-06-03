using MongoDB.Bson;
using MongoDB.Driver;
using TaskManager.API.Configurations;

namespace TaskManager.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<Models.User> _collection; // MongoDB Users collection referansı

        public UserRepository() // Constructor - Collection bağlantısını kurar
        {
            _collection = DatabaseSetup.GetUsersCollection();
        }

        public async Task<Models.User?> Insert(Models.User user) // Yeni kullanıcı ekleme işlemi
        {
            try
            {
                await _collection.InsertOneAsync(user); // MongoDB'ye kullanıcı ekleme
                Console.WriteLine($"User başarıyla kaydedildi - ID: {user.Id}");
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UserRepository Insert hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<Models.User?> GetById(string id) // ID'ye göre kullanıcı getirme işlemi
        {
            try
            {
                if (!ObjectId.TryParse(id, out ObjectId objectId)) // Güvenli ObjectId parse
                {
                    Console.WriteLine($"Geçersiz ObjectId formatı: {id}");
                    return null;
                }

                var result = await _collection.Find(x => x.Id == objectId).FirstOrDefaultAsync(); // Tek kayıt getirme
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UserRepository GetById hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<Models.User?> GetByUsername(string username) // Username'e göre kullanıcı getirme işlemi
        {
            try
            {
                var result = await _collection.Find(x => x.username == username).FirstOrDefaultAsync(); // Username ile arama
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UserRepository GetByUsername hatası: {ex.Message}");
                return null;
            }
        }
    }
}