using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text;
using System.Security.Cryptography;

namespace TaskManager.API.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } // MongoDB ObjectId - Benzersiz kullanıcı kimliği

        [BsonElement("username")]
        public string username { get; set; } = string.Empty; // Kullanıcı adı - Giriş için kullanılır

        [BsonElement("email")]
        public string email { get; set; } = string.Empty; // E-posta adresi

        [BsonElement("password_hash")]
        public string password_hash { get; set; } = string.Empty; // Şifrelenmiş şifre

        [BsonElement("created_at")]
        public DateTime created_at { get; set; } = DateTime.Now; // Hesap oluşturulma tarihi

        public static string converttoMD5(string TEXT) // MD5 şifreleme metodu
        {
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider(); // MD5 şifreleme nesnesi
            byte[] dizi = Encoding.UTF8.GetBytes(TEXT); // Metni byte dizisine çevir
            dizi = MD5.ComputeHash(dizi); // MD5 hash hesapla
            StringBuilder sb = new StringBuilder(); // String builder oluştur
            for (int i = 0; i < dizi.Length; i++)
            {
                sb.Append(dizi[i].ToString("x2").ToLower()); // Hex formatına çevir
            }
            return sb.ToString(); // Hash'lenmiş şifreyi döndür
        }
    }
}