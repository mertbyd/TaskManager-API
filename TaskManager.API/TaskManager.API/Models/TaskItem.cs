using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskManager.API.Models
{
    public class TaskItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } // MongoDB ObjectId - Benzersiz görev kimliği
        [BsonElement("title")]
        public string title { get; set; } = string.Empty; // Görev başlığı
        [BsonElement("description")]
        public string description { get; set; } = string.Empty; // Görev açıklaması
        [BsonElement("status")]
        public string status { get; set; } = "Pending"; // Görev durumu (Pending, Completed, InProgress)
        private int _priority = 1; // Öncelik seviyesi private field
        [BsonElement("priority")]//etiketi çakışma olmasın diye buna verdim
        public int priority // Öncelik seviyesi public property
        {
            get { return _priority; }
            set
            {
                if (value >= 1 && value <= 3) // 1-3 arası değer kontrolü
                {
                    _priority = value;
                }
                else
                {
                    _priority = 1; // Geçersizse default değer
                }
            }
        }
        [BsonElement("created_at")]
        public DateTime created_at { get; set; } = DateTime.Now; // Oluşturulma tarihi
        [BsonElement("user_id")]
        public string user_id { get; set; } = string.Empty; // Görevin sahibi kullanıcı ID'si
        [BsonElement("update_date")]
        public DateTime update_date { get; set; } = DateTime.Now;//Son güncelenme tarihi
        [BsonElement("period")]
        public string period { get; set; } = "daily"; // Varsayılan değer günlük
    }
}