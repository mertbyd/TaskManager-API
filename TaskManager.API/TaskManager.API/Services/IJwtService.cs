namespace TaskManager.API.Services
{
    public interface IJwtService
    {
        string CreateJwtToken(string userId, string usurname, string emal);//Token oluşturma - User bilgilerinden JWT token üretir
        string GetUserIdFromToken(string token);//Token doğrulama - JWT token'dan user ID çıkarır

    }
}
