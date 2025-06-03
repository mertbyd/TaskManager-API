using MongoDB.Driver;
using TaskManager.API.Models;


namespace TaskManager.API.Configurations

{
    public class DatabaseSetup
    {
        private readonly IMongoDatabase _database;   // MongoDB database referansı
        private static string connectionString = "";
        private static string databaseName = "";
        private static MongoDB.Driver.MongoClient client;//connect nesnem
        private static IMongoDatabase collection;//collection=>Database


        public static MongoClient Connect(IConfiguration configuration) //IConfiguration=>appsettings.json ayar dosyasına erişmemizi sağlar
        {
            try
            {
                if (configuration == null)
                {
                    Console.WriteLine("Configuration null!");
                    throw new ArgumentNullException(nameof(configuration));
                }

                connectionString = configuration.GetConnectionString("MongoDB");//GetConnectionString Dosya içi erişim için 
                databaseName = configuration.GetSection("DatabaseSettings")["DatabaseName"];//GetSection json dosyasında belli bir bölümü seçer seçtiğimiz bölümü  GetSection("DatabaseSettings") den GetSection("DatabaseSettings")[databaseName] fieldını getir
                if (databaseName==string.Empty)//appsettings.json  DatabaseName kontrolü
                {
                    Console.WriteLine("Database name bulunamadi! appsettings.json kontrol edin.");
                    throw new ArgumentNullException(nameof(databaseName));
                }
                if (connectionString == null)//appsettings.json  connectionString kontrolü
                {
                    Console.WriteLine("Connect name bulunamadi! appsettings.json kontrol edin.");
                    throw new ArgumentNullException(nameof(connectionString));
                }

                client =new MongoClient(connectionString);
                collection = client.GetDatabase(databaseName);
                using (var testclient = new MongoClient(connectionString)) 
                {  
                    var testdb=testclient.GetDatabase(databaseName);
                    testdb.RunCommand<object>("{ping:1}");//hata kontrolü [DÖNECEĞİ HATAYI BUL ŞİMDİLİK Exception ile al]
                }
                client =new MongoClient(connectionString);
                Console.WriteLine("MongoDB baglantisi basarili!");

                return client;

            }
            
            catch (ArgumentNullException ex) 
            {
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Collection olusturma hatasi: {ex.Message}");
                return null;
            }

        }

        public static IMongoCollection<TaskItem> GetTasksCollection()//Tasks collectionunda işlem yapacağımda çağıracağım
        {
            return collection.GetCollection<TaskItem>("Tasks");
        }

        public static IMongoCollection<User> GetUsersCollection()// Users collection metodu 
        {
            return collection.GetCollection<User>("Users");
        }































        // Collections'ları oluştur  YAPAY ZEKA
        public static void CreateCollections()
        {
            try
            {
                if (collection == null)
                {
                    Console.WriteLine("Database baglantisi yok! Once Connect() cagirin.");
                    return;
                }

                Console.WriteLine("Collections olusturuluyor...");
                var existingCollections = collection.ListCollectionNames().ToList();

                // Tasks collection
                if (!existingCollections.Contains("Tasks"))
                {
                    collection.CreateCollection("Tasks");
                    Console.WriteLine("Tasks collection olusturuldu");
                }
                else
                {
                    Console.WriteLine("Tasks collection mevcut");
                }

                // YENİ: Users collection
                if (!existingCollections.Contains("Users"))
                {
                    collection.CreateCollection("Users");
                    Console.WriteLine("Users collection olusturuldu");
                }
                else
                {
                    Console.WriteLine("Users collection mevcut");
                }

                Console.WriteLine("Collections hazir!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Collection olusturma hatasi: {ex.Message}");
            }
        }
    }
}
