using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TaskManager.API.Services;
namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IConfiguration _configuration; // JWT doğrulama için config'e erişim
        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            this.authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")] // Kullanıcı kayıt endpoint'i
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> CreateUser([FromBody] DTOs.AuthDTOs.RegisterDto dtoUser)
        {
            try
            {
                // DTO'nun null olup olmadığını kontrol et
                if (dtoUser == null)
                {
                    Console.WriteLine("AuthController: DTO null geldi");
                    return BadRequest(new DTOs.DTOs.ApiResponse()
                    {
                        data = null,
                        error_message = "JSON parse hatası - geçersiz veri",
                        success = false
                    });
                }

                // Username'in boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(dtoUser.username))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse()
                    {
                        data = null,
                        error_message = "Kullanıcı adı boş olamaz",
                        success = false
                    });
                }

                // Email'in boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(dtoUser.email))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse()
                    {
                        data = null,
                        error_message = "Email boş olamaz",
                        success = false
                    });
                }

                // Password'ün boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(dtoUser.password))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse()
                    {
                        data = null,
                        error_message = "Şifre boş olamaz",
                        success = false
                    });
                }

                // Service'e kullanıcı oluşturma isteği gönder
                var result = await authService.UserInsert(dtoUser);

                // Kullanıcı oluşturma işleminin başarılı olup olmadığını kontrol et
                if (result == null)
                {
                    return StatusCode(500, new DTOs.DTOs.ApiResponse()
                    {
                        success = false,
                        error_message = "Kullanıcı oluşturulurken hata oluştu",
                        data = null
                    });
                }
                Console.WriteLine($"AuthController: Başarılı kayıt - ID: {result.Id}");
                return Ok(new DTOs.DTOs.ApiResponse()
                {
                    success = true,
                    error_message = string.Empty,
                    data = result
                });
            }
            catch (Exception ex)
            {
                // Hata yönetimi
                Console.WriteLine($"AuthController hatası: {ex.Message}");
                return StatusCode(500, new DTOs.DTOs.ApiResponse
                {
                    success = false,
                    error_message = "Sunucu hatası: " + ex.Message,
                    data = null
                });
            }
        }

        [HttpGet("getid/{id}")] // ID'ye göre kullanıcı getirme endpoint'i
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> GetById(string id)
        {
            try
            {
                // ID'nin boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse()
                    {
                        data = null,
                        error_message = "User ID boş olamaz",
                        success = false
                    });
                }

                // Kullanıcıyı getir
                var result = await authService.GetById(id);

                // Kullanıcının bulunup bulunmadığını kontrol et
                if (result == null)
                {
                    return NotFound(new DTOs.DTOs.ApiResponse()
                    {
                        success = false,
                        error_message = "Kullanıcı bulunamadı",
                        data = null
                    });
                }
                return Ok(new DTOs.DTOs.ApiResponse()
                {
                    data = result,
                    success = true,
                    error_message = string.Empty,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DTOs.DTOs.ApiResponse
                {
                    success = false,
                    error_message = "Sunucu hatası: " + ex.Message,
                    data = null
                });
            }
        }

        [HttpGet("getusername/{username}")] // Username'e göre kullanıcı getirme endpoint'i
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> GetByUsername(string username)
        {
            try
            {
                // Username'in boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse()
                    {
                        data = null,
                        error_message = "Username boş olamaz",
                        success = false
                    });
                }

                // Username ile kullanıcı ara
                var result = await authService.GetByUsername(username);

                // Kullanıcının bulunup bulunmadığını kontrol et
                if (result == null)
                {
                    return NotFound(new DTOs.DTOs.ApiResponse()
                    {
                        success = false,
                        error_message = "Kullanıcı bulunamadı",
                        data = null
                    });
                }
                return Ok(new DTOs.DTOs.ApiResponse()
                {
                    data = result,
                    success = true,
                    error_message = string.Empty,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DTOs.DTOs.ApiResponse
                {
                    success = false,
                    error_message = "Sunucu hatası: " + ex.Message,
                    data = null
                });
            }
        }

        [HttpPost("login")] // Kullanıcı giriş endpoint'i
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> ControlLogin([FromBody] DTOs.AuthDTOs.LoginDto dtouser)
        {
            try
            {
                // DTO'nun null olup olmadığını kontrol et
                if (dtouser == null)
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse()
                    {
                        data = null,
                        success = false,
                        error_message = "JSON parse hatası",
                    });
                }

                // Giriş denemesi
                var result = await authService.Login(dtouser);

                // Giriş işleminin başarılı olup olmadığını kontrol et
                if (result == null)
                {
                    return NotFound(new DTOs.DTOs.ApiResponse()
                    {
                        data = null,
                        success = false,
                        error_message = "Hatalı kullanıcı adı veya şifre",
                    });
                }

                // Başarılı giriş
                return Ok(new DTOs.DTOs.ApiResponse()
                {
                    success = true,
                    data = result, // Token ve kullanıcı bilgileri
                    error_message = string.Empty,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DTOs.DTOs.ApiResponse
                {
                    success = false,
                    error_message = "Sunucu hatası: " + ex.Message,
                    data = null
                });
            }
        }
    }
}