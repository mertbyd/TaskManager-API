using TaskManager.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace TaskManager.API.Controllers
{
    [ApiController] // Bu controller'ın bir API controller olduğunu belirtir
    [Route("api/[controller]")] // URL yolunu belirler -> /api/Task
    [Authorize] // JWT token gerektiren bir güvenlik katmanı ekler - giriş yapmış kullanıcılar erişebilir
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _productHandler;
        public TaskController(ITaskService ProductHandler)
        {
            _productHandler = ProductHandler; // Dependency Injection ile service'i alıyoruz
        }

        // POST /api/Task - Yeni görev oluşturma endpoint'i
        [HttpPost]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), 200)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> CreateTask([FromBody] DTOs.DTOs.CreateTaskDto dtoTask)
        {
            try
            {
                // JWT token'dan kullanıcı ID'sini al
                var userId = User.FindFirst("user_id")?.Value;

                // UserId'nin JWT'de olup olmadığını kontrol et
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        data = null,
                        error_message = "User ID bulunamadı",
                        success = false
                    });
                }

                // DTO'nun null olup olmadığını kontrol et
                if (dtoTask == null)
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        data = null,
                        error_message = "JSON parse hatası",
                        success = false
                    });
                }

                // Title'ın boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(dtoTask.title))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        data = null,
                        error_message = "Görev başlığı boş olamaz",
                        success = false
                    });
                }

                // Service katmanına isteği gönder
                var result = await _productHandler.TaskInsert(dtoTask, userId);

                // Görev oluşturma işleminin başarılı olup olmadığını kontrol et
                if (result == null)
                {
                    return StatusCode(500, new DTOs.DTOs.ApiResponse
                    {
                        success = false,
                        error_message = "Görev oluşturulurken hata oluştu",
                        data = null
                    });
                }

                // Başarılı cevap döndür
                return Ok(new DTOs.DTOs.ApiResponse
                {
                    success = true,
                    error_message = string.Empty,
                    data = result
                });
            }
            catch (Exception ex)
            {
                // Beklenmeyen hatalar için genel hata yakalama
                return StatusCode(500, new DTOs.DTOs.ApiResponse
                {
                    success = false,
                    error_message = "Sunucu hatası: " + ex.Message,
                    data = null
                });
            }
        }

        // GET /api/Task - Tüm görevleri getir (admin kullanımı için)
        [HttpGet]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), 200)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> GetAllProduct()
        {
            try
            {
                var result = await _productHandler.TasktGetAll();

                // Görevlerin getirilme işleminin başarılı olup olmadığını kontrol et
                if (result == null)
                {
                    return StatusCode(500, new DTOs.DTOs.ApiResponse
                    {
                        success = false,
                        error_message = "Görevler getirilirken hata oluştu",
                        data = null
                    });
                }
                return Ok(new DTOs.DTOs.ApiResponse
                {
                    success = true,
                    data = result, // Tüm görevleri data kısmında döndür
                    error_message = string.Empty
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

        // GET /api/Task/my-tasks - Sadece giriş yapan kullanıcının görevlerini getir
        [HttpGet("my-tasks")]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), 200)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> GetMyTasks()
        {
            try
            {
                // JWT'den kullanıcı kimliğini al
                var userId = User.FindFirst("user_id")?.Value;

                // UserId'nin JWT'de olup olmadığını kontrol et
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        data = null,
                        success = false,
                        error_message = "User ID bulunamadı"
                    });
                }

                // Sadece bu kullanıcıya ait görevleri getir
                var result = await _productHandler.TaskGetAllbyId(userId);

                // Görevlerin getirilme işleminin başarılı olup olmadığını kontrol et
                if (result == null)
                {
                    return StatusCode(500, new DTOs.DTOs.ApiResponse
                    {
                        success = false,
                        error_message = "Görevler getirilirken hata oluştu",
                        data = null
                    });
                }
                return Ok(new DTOs.DTOs.ApiResponse
                {
                    success = true,
                    data = result,
                    error_message = string.Empty
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

        // GET /api/Task/{id} - ID'ye göre tek bir görev getir
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), 200)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> GetTaskById(string id)
        {
            try
            {
                var userId = User.FindFirst("user_id")?.Value;

                // UserId'nin JWT'de olup olmadığını kontrol et
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        data = null,
                        error_message = "User ID bulunamadı",
                        success = false
                    });
                }

                // ID ve userId ile görevi bul (yetki kontrolü için userId de kullanılır)
                var result = await _productHandler.GetTaskById(id, userId);

                // Görevin bulunup bulunmadığını kontrol et
                if (result == null)
                {
                    // 404 Not Found - Görev bulunamadı veya başkasının görevi
                    return NotFound(new DTOs.DTOs.ApiResponse
                    {
                        success = false,
                        error_message = "Task bulunamadı veya size ait değil",
                        data = null
                    });
                }
                return Ok(new DTOs.DTOs.ApiResponse
                {
                    success = true,
                    data = result,
                    error_message = string.Empty
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

        // DELETE /api/Task/{id} - Görev silme işlemi
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), 200)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> DeleteTask(string id)
        {
            try
            {
                // Güvenlik kontrolü - kullanıcı kimliği
                var userId = User.FindFirst("user_id")?.Value;

                // UserId'nin JWT'de olup olmadığını kontrol et
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        data = null,
                        error_message = "User ID bulunamadı",
                        success = false
                    });
                }

                // Task ID'nin boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        data = null,
                        error_message = "Task ID boş olamaz",
                        success = false
                    });
                }

                // Silme işlemi - sadece kendi görevlerini silebilir
                var result = await _productHandler.DeleteTask(id, userId);

                // Silme işleminin başarılı olup olmadığını kontrol et
                if (!result)
                {
                    return NotFound(new DTOs.DTOs.ApiResponse
                    {
                        success = false,
                        error_message = "Task bulunamadı veya silme yetkisi yok",
                        data = null
                    });
                }

                // Başarılı silme işlemi
                return Ok(new DTOs.DTOs.ApiResponse
                {
                    success = true,
                    error_message = string.Empty,
                    data = "Task başarıyla silindi"
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

        // PUT /api/Task/{id} - Görevi tamamen güncelle (tüm alanlar)
        // PUT: Resource'un tamamını değiştirir, eksik alanları default değerlerle doldurur
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), 200)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> UpdateTask(string id, [FromBody] DTOs.DTOs.UpdateTaskDto updateDto)
        {
            try
            {
                var userId = User.FindFirst("user_id")?.Value;

                // UserId'nin JWT'de olup olmadığını kontrol et
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        data = null,
                        error_message = "User ID bulunamadı",
                        success = false
                    });
                }

                // Görevi güncelle - service validasyon yapar
                var result = await _productHandler.UpdateTask(id, userId, updateDto);

                // Güncelleme işleminin başarılı olup olmadığını kontrol et
                if (result == null)
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        success = false,
                        error_message = "Task güncellenemedi - kontrolleri gözden geçirin",
                        data = null
                    });
                }

                // Güncellenmiş görevi döndür
                return Ok(new DTOs.DTOs.ApiResponse
                {
                    success = true,
                    error_message = string.Empty,
                    data = result
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

        // PATCH /api/Task/{id} - Görevi kısmen güncelle (sadece gönderilen alanlar)
        // PATCH: Resource'un sadece belirli kısımlarını günceller, diğer alanlar aynı kalır
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), 200)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> PatchTask(string id, [FromBody] DTOs.DTOs.PatchTaskDto patchDto)
        {
            try
            {
                var userId = User.FindFirst("user_id")?.Value;

                // UserId'nin JWT'de olup olmadığını kontrol et
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        data = null,
                        error_message = "User ID bulunamadı",
                        success = false
                    });
                }

                // Görevi parçalı güncelle - service layer validasyon yapar
                var result = await _productHandler.PatchTask(id, userId, patchDto);

                // Güncelleme işleminin başarılı olup olmadığını kontrol et
                if (result == null)
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        success = false,
                        error_message = "Task yamalanırken hata oluştu",
                        data = null
                    });
                }

                // Güncellenmiş görevi döndür
                return Ok(new DTOs.DTOs.ApiResponse
                {
                    success = true,
                    error_message = string.Empty,
                    data = result
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

        [HttpGet("period/{period}")] // Period'a göre görevleri getirme endpoint'i
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), 200)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> GetTasksByPeriod(string period)
        {
            try
            {
                var userId = User.FindFirst("user_id")?.Value;

                // UserId'nin JWT'de olup olmadığını kontrol et
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        data = null,
                        error_message = "User ID bulunamadı",
                        success = false
                    });
                }

                var result = await _productHandler.GetTasksByPeriod(userId, period);

                // Görevlerin getirilme işleminin başarılı olup olmadığını kontrol et
                if (result == null)
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        success = false,
                        error_message = "Görevler getirilirken hata oluştu",
                        data = null
                    });
                }
                return Ok(new DTOs.DTOs.ApiResponse
                {
                    success = true,
                    error_message = string.Empty,
                    data = result
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