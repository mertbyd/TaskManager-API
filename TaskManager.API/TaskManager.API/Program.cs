using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Text;
using TaskManager.API.Configurations;
using TaskManager.API.Repositories;
using TaskManager.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Controller'ları sisteme ekle
builder.Services.AddControllers();
// API dökümantasyonu için gerekli servisler
builder.Services.AddEndpointsApiExplorer();

// Swagger konfigürasyonu - API dökümantasyonu ve test arayüzü için
builder.Services.AddSwaggerGen(c =>
{
    // API hakkında temel bilgiler
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskManager API",
        Version = "v1",
        Description = "Görev Yönetim Sistemi API"
    });

    // JWT token için güvenlik tanımı - Swagger'da "Authorize" butonu için
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Her endpoint için JWT token gereksinimini belirle
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication sistemi kurulumu
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        // Token doğrulama parametreleri
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, // Secret key kontrolü yap
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JwtSettings:SecretKey"] ?? "TaskManagerSecretKey2024TaskManagerJwtSecretKey2024"
                )
            ),
            ValidateIssuer = false, // Token'ı kimin verdiğini kontrol etme
            ValidateAudience = false, // Token'ın kim için olduğunu kontrol etme
            ValidateLifetime = true, // Token'ın süresinin dolup dolmadığını kontrol et
            ClockSkew = TimeSpan.Zero // Zaman farkı toleransı
        };
    });

// Authorization (yetkilendirme) sistemi
builder.Services.AddAuthorization();

// MongoDB veritabanı bağlantısı kur
var client = DatabaseSetup.Connect(builder.Configuration);

// MongoDB bağlantı durumunu kontrol et
if (client != null)
{
    DatabaseSetup.CreateCollections(); // Koleksiyonları oluştur
    Console.WriteLine("MongoDB bağlantısı başarılı!");
}
else
{
    Console.WriteLine("MongoDB bağlantı hatası!");
}

// Dependency Injection - Interface'ler ile Class'ları eşleştir
// Repository katmanı - Veritabanı işlemleri
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// Service katmanı - İş mantığı
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

// HTTP request pipeline yapılandırması
// Development ortamında Swagger arayüzünü aktif et
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Swagger JSON endpoint'ini etkinleştir
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API v1");
        // Swagger UI sayfası konfigürasyonu
    });
}

// HTTPS yönlendirmesi - güvenlik için
app.UseHttpsRedirection();

// Authentication middleware - JWT token kontrolü
app.UseAuthentication();

// Authorization middleware - yetki kontrolü
app.UseAuthorization();

// Controller'ları route'lara eşle
app.MapControllers();

// Uygulama başlatma mesajları
Console.WriteLine("TaskManager API başlatıldı");
Console.WriteLine("Swagger UI: https://localhost:7016/swagger");
Console.WriteLine("API Endpoint: POST https://localhost:7016/api/Task");

// Uygulamayı çalıştır
app.Run();