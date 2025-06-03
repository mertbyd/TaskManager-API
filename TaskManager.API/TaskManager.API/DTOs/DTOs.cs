namespace TaskManager.API.DTOs
{
    public class DTOs
    {
        // POST isteğinde gelen veri formatı (client'dan API'ye)
        public class CreateTaskDto
        {
            //Models.TaskItem göndermiyoruz çünkü her field post isteği ile verilmesine gerek yok
            public string title { get; set; } = "";       // Görev başlığı (zorunlu)
            public string description { get; set; } = ""; // Görev açıklaması (isteğe bağlı)
            public int priority { get; set; } = 1; // Öncelik seviyesi (Low, Medium, High)
                                                   //public string status { get; set; } = "Pending"; => sonradan kulanıcı tarafından belirlneceği için post da gönderilmez            -!endpoint tanımla-
                                                   // public DateTime created_at { get; set; } = DateTime.Now;  Serviste oluşur
                                                   //public string user_id { get; set; } = "";     JWT'de  -Burda vermek güvenlik sorunu yaratır-
            public string period { get; set; } = "daily"; // daily, weekly, monthly
        }
        public class ApiResponse
        {
            public bool success { get; set; }      // İşlem başarılı mı? (true/false)
            public string error_message { get; set; } = "";  // Hata mesajı 
            public object? data { get; set; }      // Dönen veri  -null yada Tasklist|tASK
        }
        public class UpdateTaskDto//Günclemek için Put için
        {
            public string title { get; set; } = "";
            public string description { get; set; } = "";
            public int priority { get; set; } = 1;
            public string status { get; set; } = "";
        }
        public class PatchTaskDto//Farkı Put tamamnını güncelediği yerler içim  boş yerlere string.empty vermeliyiz 
                                 //ama patchte sadece dolu yerler günceleneceği için farklı dto olmalı
        {
            public string? title { get; set; } = null;
            public string? description { get; set; } = null;
            public int? priority { get; set; } = null;
            public string? status { get; set; } = null;
        }
    }
}
