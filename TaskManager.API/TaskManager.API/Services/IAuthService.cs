namespace TaskManager.API.Services
{
    public interface IAuthService
    {
       
        Task<Models.User?> UserInsert(DTOs.AuthDTOs.RegisterDto registerDto); // Yeni kullanıcı kayıt etmek için kullanılır
        Task<Models.User?> GetById(string id);// Id ye göre kulanıcı getirmek iççin kullanılır
        Task<Models.User?> GetByUsername(string username); // Username'e göre kullanıcı getirme
        Task<DTOs.AuthDTOs.LoginResponseDto?> Login(DTOs.AuthDTOs.LoginDto loginDto); // Kullanıcı giriş işlemi - LoginDto alır, LoginResponseDto döndürür
    }
}
