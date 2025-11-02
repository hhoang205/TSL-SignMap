using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.Services;
using WebAppTrafficSign.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WebAppTrafficSign API",
        Version = "v1",
        Description = "API docs for WebAppTrafficSign"
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MyDb"),
        x => x.UseNetTopologySuite());
});

// Đăng ký các services
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICoinWalletService, CoinWalletService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ITrafficSignService, TrafficSignService>();

// Cấu hình JWT (có thể thêm vào appsettings.json)
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    { "Jwt:SecretKey", builder.Configuration["Jwt:SecretKey"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLongForHS256" },
    { "Jwt:Issuer", builder.Configuration["Jwt:Issuer"] ?? "WebAppTrafficSign" },
    { "Jwt:Audience", builder.Configuration["Jwt:Audience"] ?? "WebAppTrafficSignUsers" }
});

var app = builder.Build();

// Hiển thị Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAppTrafficSign API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
