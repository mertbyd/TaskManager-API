using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TaskManager.API.Services
{
    public class JwtService : IJwtService
    {
        public JwtService(IConfiguration Configuration)
        {
            configuration = Configuration;
        }
        IConfiguration configuration;


        // Token oluşturma - User bilgilerinden JWT token üretir
        public string CreateJwtToken(string userId, string username, string email)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler(); // JWT handler oluştur
                                                                  // Secret key al (appsettings.json'dan)
                var secretKey = configuration["JwtSettings:SecretKey"] ?? "TaskManagerSecretKey2024TaskManagerSecretKey2024";//Bunu nerden okuduğunu araştır
                var key = Encoding.UTF8.GetBytes(secretKey);// appsettings.json'dan secret key oku, yoksa default kullan

                // Token içeriği (Claims)
                var claims = new[]
                 {
                 new Claim("user_id", userId),
                 new Claim("username", username),
                 new Claim("email", email),
                 new Claim("iat", DateTime.Now.ToString()) // Token oluşturma zamanı
                 };
                var tokenbody = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),// Token içeriği (kullanıcı bilgileri)
                    Expires = DateTime.UtcNow.AddDays(7), // 7 gün geçerli
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), // Şifreleme anahtarı
                           SecurityAlgorithms.HmacSha256Signature
                )
                };
                var token = tokenHandler.CreateToken(tokenbody);
                return tokenHandler.WriteToken(token);//token nesnesini string e çevirir
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JwtService CreateToken hatası: {ex.Message}");
                return string.Empty;
            }
        }


        // Token doğrulama - JWT token'dan user ID çıkarır
        public string GetUserIdFromToken(string token)
        {
            try
            {
                var tokenhandler = new JwtSecurityTokenHandler();
                var secretKey = configuration["JwtSettings:SecretKey"] ?? "TaskManagerSecretKey2024TaskManagerSecretKey2024";
                var key = Encoding.UTF8.GetBytes(secretKey);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // Token imzasını kontrol et (sahte token'ları engeller)
                    IssuerSigningKey = new SymmetricSecurityKey(key), // İmza kontrolü için kullanılacak anahtar
                    ValidateLifetime = true, // Token'ın süresi dolmuş mu kontrol et
                    ClockSkew = TimeSpan.Zero // Zaman farkı toleransı (0 = kesin zaman kontrolü)
                };
                // Token'ı doğrula
                var userPrincipal = tokenhandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                Claim? userIdClaim = userPrincipal.FindFirst("user_id");
                if (userIdClaim == null)
                {
                    return string.Empty;
                }
                else
                {
                    return userIdClaim.Value;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"JwtService ValidateToken hatası: {ex.Message}");
                return string.Empty;
            }
        }

    }
}
