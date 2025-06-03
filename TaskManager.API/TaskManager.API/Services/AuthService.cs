using TaskManager.API.Models;
using TaskManager.API.Repositories;
namespace TaskManager.API.Services
{
    public class AuthService : IAuthService
    {
        public AuthService(IUserRepository DefaultProductService, IJwtService _JwtService)
        {
            defaultProductService = DefaultProductService;
            jwtService = _JwtService;
        }
        IUserRepository defaultProductService;
        IJwtService jwtService; // Interface kullan

        public async Task<Models.User?> UserInsert(DTOs.AuthDTOs.RegisterDto dtouser) // Kullanıcı kayıt işlemi - DTO'yu User modeline çevirir ve Repository'ye gönderir
        {
            try
            {
                // kulanıcı adı uniqe olmalı => app girişinde username kulanıcam
                var existingUser = await defaultProductService.GetByUsername(dtouser.username);

                // Username'in daha önce kullanılıp kullanılmadığını kontrol et
                if (existingUser != null)// Username zaten var
                {
                    Console.WriteLine($"AuthService: Username zaten kullanımda - {dtouser.username}");
                    return null;
                }
                //
                // DTO'yu User modeline çevir
                var user = new Models.User()
                {
                    username = dtouser.username,
                    email = dtouser.email,
                    password_hash = Models.User.converttoMD5(dtouser.password) // Şifreyi MD5'le hashle
                };
                var result = await defaultProductService.Insert(user);

                // Kullanıcının başarıyla kaydedilip kaydedilmediğini kontrol et
                if (result == null)
                {
                    Console.WriteLine("AuthService: User oluşturulamadı");
                    return null;
                }
                Console.WriteLine($"AuthService: User başarıyla oluşturuldu - ID: {result.Id}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService Register hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<Models.User?> GetById(string Id) // ID'ye göre kullanıcı getirme - Repository'den kullanıcı bilgilerini çeker
        {
            try
            {
                // ID'nin boş olup olmadığını kontrol et
                if (Id == string.Empty)
                {
                    Console.WriteLine("AuthService: User ID boş");
                    return null;
                }
                var result = await defaultProductService.GetById(Id);

                // Kullanıcının bulunup bulunmadığını kontrol et
                if (result == null)
                {
                    Console.WriteLine($"AuthService: User bulunamadı - ID: {Id}");
                    return null;
                }
                Console.WriteLine($"AuthService: User başarıyla getirildi - ID: {result.Id}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService GetUserById hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<Models.User?> GetByUsername(string username) // Username'e göre kullanıcı getirme - Repository'den kullanıcı bilgilerini çeker
        {
            try
            {
                // Username'in boş veya null olup olmadığını kontrol et
                if (string.IsNullOrEmpty(username))
                {
                    Console.WriteLine("AuthService: Username boş");
                    return null;
                }
                // Repository'den kullanıcı getir
                var result = await defaultProductService.GetByUsername(username);

                // Kullanıcının bulunup bulunmadığını kontrol et
                if (result == null)
                {
                    Console.WriteLine($"AuthService: User bulunamadı - Username: {username}");
                    return null;
                }
                Console.WriteLine($"AuthService: User başarıyla getirildi - Username: {result.username}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService GetByUsername hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<DTOs.AuthDTOs.LoginResponseDto?> Login(DTOs.AuthDTOs.LoginDto dtolog) // Kullanıcı giriş işlemi - Username ve password kontrolü yapar
        {
            try
            {
                // Username ve password'ün boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(dtolog.username) || string.IsNullOrEmpty(dtolog.password))
                {
                    Console.WriteLine("AuthService: Username veya password boş");
                    return null;
                }
                // Username ile kullanıcı bul
                var result = await defaultProductService.GetByUsername(dtolog.username);

                // Kullanıcının sistemde var olup olmadığını kontrol et
                if (result == null)
                {
                    Console.WriteLine($"AuthService: Kullanıcı bulunamadı - Username: {dtolog.username}");
                    return null;
                }

                // Girilen şifrenin doğru olup olmadığını kontrol et
                if (result.password_hash != Models.User.converttoMD5(dtolog.password))
                {
                    Console.WriteLine($"AuthService: Hatalı şifre - Username: {dtolog.username}");
                    return null;
                }
                // JWT Token oluştur
                var token = jwtService.CreateJwtToken(
                    result.Id.ToString(),
                    result.username,
                    result.email
                );
                // LoginResponseDto oluştur ve döndür
                var response = new DTOs.AuthDTOs.LoginResponseDto()
                {
                    token = token,//oluştrduğumuz token ı response ve gelen kullanıcıyıda dto da döndük ki ikisini birden return edebilelim
                    user_id = result.Id.ToString(),
                    username = result.username,
                    email = result.email
                };
                Console.WriteLine($"AuthService: Başarılı giriş - Username: {result.username}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthService Login hatası: {ex.Message}");
                return null;
            }
        }
    }
}