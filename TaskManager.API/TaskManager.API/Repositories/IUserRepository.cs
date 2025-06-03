namespace TaskManager.API.Repositories
{
    public interface IUserRepository
    {
        public Task<Models.User?> Insert(Models.User user); //Yeni kullanıcı ekleme
        public Task<Models.User?> GetById(string id); //ID'ye göre kullanıcı getirme
        public Task<Models.User?> GetByUsername(string username);// Username'e göre kullanıcı getirme
    }
}
