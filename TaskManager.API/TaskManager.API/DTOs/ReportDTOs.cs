namespace TaskManager.API.DTOs
{
    public class ReportDTOs
    {
        // Rapor isteği için DTO
        public class ReportRequestDto
        {
            public DateTime? start_date { get; set; } = null; // Başlangıç tarihi (null ise bugün)
            public DateTime? end_date { get; set; } = null;   // Bitiş tarihi (null ise period'a göre)
        }

        // Rapor sonucu için DTO
        public class ReportResponseDto
        {
            public string period { get; set; } = ""; // "daily", "weekly", "monthly"
            public DateTime start_date { get; set; } // Rapor başlangıç
            public DateTime end_date { get; set; }   // Rapor bitiş
            public int total_tasks { get; set; }     // Toplam görev sayısı
            public int completed_tasks { get; set; } // Tamamlanan görev sayısı
            public int pending_tasks { get; set; }   // Bekleyen görev sayısı
            public int in_progress_tasks { get; set; } // Devam eden görev sayısı
            public List<TaskReportItemDto> tasks { get; set; } = new List<TaskReportItemDto>();
        }

        // Rapor içindeki görev bilgisi için DTO
        public class TaskReportItemDto
        {
            public string id { get; set; } = "";
            public string title { get; set; } = "";
            public string description { get; set; } = "";
            public string status { get; set; } = "";
            public int priority { get; set; }
            public string period { get; set; } = ""; // daily, weekly, monthly
            public DateTime created_at { get; set; }
        }
    }
}