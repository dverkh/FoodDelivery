using Microsoft.EntityFrameworkCore;
using FoodDelivery.Storage.EntityConfigurations;
using FoodDelivery.Domain.Contracts;
using FoodDelivery.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FoodDelivery.Storage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FoodDeliveryContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();
builder.Services.AddScoped<IOrderService, OrderService>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,

            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,

            IssuerSigningKey = AuthOptions.AccessToken.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userIdClaim = context.Principal.FindFirst(ClaimTypes.NameIdentifier);
                var iatClaim = context.Principal.FindFirst(JwtRegisteredClaimNames.Iat);

                if (userIdClaim == null || iatClaim == null)
                {
                    context.Fail("Недостаточно данных в токене.");
                    return;
                }

                if (!int.TryParse(userIdClaim.Value, out var userId))
                {
                    context.Fail("Некорректный идентификатор пользователя в токене.");
                    return;
                }

                if (!long.TryParse(iatClaim.Value, out var iatSeconds))
                {
                    context.Fail("Некорректный параметр 'iat' в токене.");
                    return;
                }

                var dbContext = context.HttpContext.RequestServices.GetRequiredService<FoodDeliveryContext>();
                var user = await dbContext.Clients.FindAsync(userId);

                if (user == null)
                {
                    context.Fail("Пользователь не найден.");
                    return;
                }

                var tokenIssuedAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(iatClaim.Value)).UtcDateTime;
                if (user.LastPasswordUpdateTime > tokenIssuedAt)
                {
                    context.Fail("Пароль был изменен после выпуска токена.");
                }
            }
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Client", policy => policy.RequireRole("Client"));
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "FoodDelivery API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Введите токен JWT в формате: Bearer {your token here}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
