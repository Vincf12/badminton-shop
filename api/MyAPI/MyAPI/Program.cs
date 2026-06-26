using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using MyAPI.Infrastructure.Persistence;
using MyAPI.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IO;

// Dung alias SwaggerModels de tranh trung ten voi cac model OpenAPI.
using SwaggerModels = Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApplicationServices();

// In development, load the repo root .env for local runs only.
// When the API runs in Docker, compose-provided environment variables must win.
var runningInContainer = string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase);
if (builder.Environment.IsDevelopment() && !runningInContainer)
{
    var envPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "..", "..", ".env"));
    if (File.Exists(envPath))
    {
        var envValues = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in File.ReadAllLines(envPath))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
            {
                continue;
            }

            var separatorIndex = trimmed.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = trimmed[..separatorIndex].Trim();
            if (key.StartsWith("Jwt:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var value = trimmed[(separatorIndex + 1)..].Trim();
            envValues[key] = value;
        }

        builder.Configuration.AddInMemoryCollection(envValues);
    }
}

// Add services to the container.
builder.Services.AddControllers();

// Allow CORS from frontend during development to avoid "Failed to fetch" in browser
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DevCors", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });
}

// Cấu hình Kết nối Database MySQL
if (builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection")))
{
    var dbHost = builder.Configuration["MYSQL_HOST"] ?? "localhost";
    var dbPort = builder.Configuration["MYSQL_PORT"] ?? "3308";
    var dbName = builder.Configuration["MYSQL_DATABASE"] ?? "badminton_shop";
    var dbUser = builder.Configuration["MYSQL_USER"] ?? "app";
    var dbPassword = builder.Configuration["MYSQL_PASSWORD"] ?? "change-this-db-password";

    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["ConnectionStrings:DefaultConnection"] = $"Server={dbHost};Port={dbPort};Database={dbName};Uid={dbUser};Pwd={dbPassword};"
    });
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Missing connection string 'DefaultConnection'. Set it through environment variables or user secrets.");
    }

    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 0))
    );
});

// Cấu hình Xác thực JWT Authentication
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "MyAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "MyAPIClient";
var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("Missing JWT signing key. Set Jwt:Key through environment variables or user secrets."))
        )
    };
});

// Thêm dịch vụ Cấp quyền
builder.Services.AddAuthorization();

// --- Cấu hình Swagger UI (Đã sửa dùng Alias SwaggerModels) ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new SwaggerModels.OpenApiInfo
    {
        Title = "MyAPI",
        Version = "v1"
    });

    // Cấu hình nút khóa Bearer Token
    options.AddSecurityDefinition("Bearer", new SwaggerModels.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SwaggerModels.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = SwaggerModels.ParameterLocation.Header,
        Description = "Nhập trực tiếp chuỗi Token vào đây (Không gõ thêm chữ Bearer)."
    });

    // Áp dụng yêu cầu Token cho các API có thuộc tính [Authorize]
    options.AddSecurityRequirement(new SwaggerModels.OpenApiSecurityRequirement
    {
        {
            new SwaggerModels.OpenApiSecurityScheme
            {
                Reference = new SwaggerModels.OpenApiReference
                {
                    Type = SwaggerModels.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Cấu hình tránh cảnh báo HTTPS ở local
builder.Services.Configure<Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionOptions>(options =>
{
    options.HttpsPort = 7115;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Đường dẫn chuẩn xác để tránh lỗi Failed to fetch
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI v1");
        options.RoutePrefix = "swagger";
    });
}

// Only redirect to HTTPS in non-development environments to avoid
// HTTP -> HTTPS 307 redirects during local testing (which can cause
// Swagger UI "Failed to fetch" or mixed-content issues).
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

// Enable CORS only in development
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
}

app.UseAuthentication(); // 1. Bạn là ai?
app.UseAuthorization();  // 2. Bạn được làm gì?

app.MapControllers();
app.Run();
