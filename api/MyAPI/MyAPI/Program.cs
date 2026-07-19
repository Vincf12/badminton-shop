using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IO;

// DÃ¹ng alias SwaggerModels Ä‘á»ƒ trÃ¡nh trÃ¹ng tÃªn vá»›i cÃ¡c model OpenAPI.
using SwaggerModels = Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// KHU Vá»°C 1: ÄÄ‚NG KÃ CÃC Cáº¤U HÃŒNH & SERVICES (DEPENDENCY INJECTION)
// Táº¥t cáº£ builder.Services.Add... Báº®T BUá»˜C pháº£i náº±m trong khu vá»±c nÃ y.
// =========================================================================

builder.Services.AddApplicationServices();

#region Äá»c file mÃ´i trÆ°á»ng .env á»Ÿ local (Development)
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
#endregion

// ÄÄƒng kÃ½ Controllers
builder.Services.AddControllers();

// Cáº¥u hÃ¬nh CORS cho mÃ´i trÆ°á»ng Development
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

#region Cáº¥u hÃ¬nh Káº¿t ná»‘i Database MySQL
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
#endregion

#region Cáº¥u hÃ¬nh XÃ¡c thá»±c JWT Authentication
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
#endregion

// ThÃªm dá»‹ch vá»¥ Cáº¥p quyá»n (Authorization)
builder.Services.AddAuthorization();

#region Cáº¥u hÃ¬nh Swagger UI (Sá»­ dá»¥ng Alias SwaggerModels)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new SwaggerModels.OpenApiInfo
    {
        Title = "MyAPI",
        Version = "v1"
    });

    // Cáº¥u hÃ¬nh nÃºt khÃ³a Bearer Token
    options.AddSecurityDefinition("Bearer", new SwaggerModels.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SwaggerModels.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = SwaggerModels.ParameterLocation.Header,
        Description = "Nháº­p trá»±c tiáº¿p chuá»—i Token vÃ o Ä‘Ã¢y (KhÃ´ng gÃµ thÃªm chá»¯ Bearer)."
    });

    // Ãp dá»¥ng yÃªu cáº§u Token cho cÃ¡c API cÃ³ thuá»™c tÃ­nh [Authorize]
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
#endregion

// Cáº¥u hÃ¬nh trÃ¡nh cáº£nh bÃ¡o HTTPS á»Ÿ local
builder.Services.Configure<Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionOptions>(options =>
{
    options.HttpsPort = 7115;
});

// [ÄÃƒ Sá»¬A]: Chuyá»ƒn dÃ²ng nÃ y tá»« dÆ°á»›i lÃªn trÃªn builder.Build() Ä‘á»ƒ trÃ¡nh lá»—i Read-Only
builder.Services.AddScoped<IStoreService, StoreService>();


// =========================================================================
// KHU Vá»°C 2: BUILD á»¨NG Dá»¤NG & Cáº¤U HÃŒNH HTTP REQUEST PIPELINE (MIDDLEWARE)
// =========================================================================

// KhÃ³a Services Collection, khá»Ÿi táº¡o á»©ng dá»¥ng
var app = builder.Build();

// Cáº¥u hÃ¬nh Swagger cho mÃ´i trÆ°á»ng Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // ÄÆ°á»ng dáº«n chuáº©n xÃ¡c Ä‘á»ƒ trÃ¡nh lá»—i Failed to fetch
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI v1");
        options.RoutePrefix = "swagger";
    });
}

// Chá»‰ redirect HTTPS ngoÃ i mÃ´i trÆ°á»ng dev Ä‘á»ƒ trÃ¡nh lá»—i Swagger local
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

// Báº­t CORS cho mÃ´i trÆ°á»ng Development
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
}

app.UseAuthentication(); // 1. Báº¡n lÃ  ai? (XÃ¡c thá»±c JWT)
app.UseAuthorization();  // 2. Báº¡n Ä‘Æ°á»£c lÃ m gÃ¬? (PhÃ¢n quyá»n)

app.MapControllers();

// Khá»Ÿi cháº¡y á»©ng dá»¥ng
app.Run();
