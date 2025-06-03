namespace TaskManager.API.DTOs
{
    public class AuthDTOs
    {
        public class RegisterDto // Kullanıcı kayıt olurken gönderilecek veri
        {
            public string username { get; set; } = ""; // Kullanıcı adı
            public string email { get; set; } = ""; // E-posta adresi
            public string password { get; set; } = ""; // Ham şifre
        }
        public class LoginDto // Kullanıcı giriş yaparken gönderilecek veri
        {
            public string username { get; set; } = ""; // Kullanıcı adı

            public string password { get; set; } = ""; // Ham şifre
        }
        public class LoginResponseDto // Giriş başarılı olduğunda döndürülecek veri
        {
            public string token { get; set; } = ""; // JWT token
            public string user_id { get; set; } = ""; // Kullanıcı ID'si
            public string username { get; set; } = ""; // Kullanıcı adı
            public string email { get; set; } = ""; // E-posta adresi
        }
        public class TokenCheckDto
        {
            public string token { get; set; } = "";
        }
    }
}