using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetflixClone.Data;
using NetflixClone.Models;
using NetflixClone.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddControllers();
builder.Services.AddHttpClient("Paystack", client =>
{
    client.BaseAddress = new Uri("https://api.paystack.co/");
    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + builder.Configuration["PayStack:SecretKey"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<PaystackService>();

var key = builder.Configuration["JWT:Key"]
    ?? throw new InvalidOperationException("Jwt: Key is missing in appsettings.json");
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
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });
var app = builder.Build();

app.Use((context, next) =>
{
    LoggedInUser.Token = context.Request.Headers["Authorization"].ToString();
    LoggedInUser.Token = LoggedInUser.Token.Replace("Bearer ", "");

    if (!string.IsNullOrEmpty(LoggedInUser.Token))
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadToken(LoggedInUser.Token) as JwtSecurityToken;
        if (token != null)
        {
            LoggedInUser.UserId = Convert.ToInt16(token.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value ?? "-1");
        }
    }
    return next();
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    if (!context.Plans.Any())
    {
        Console.WriteLine("Seeding plans into database..");
        context.Plans.AddRange(new Plan
        {
            Id = 1,
            Name = "Mobile",
            Price = 2500,
            VideoQuality = "Fair",
            Resolution = "480p",
            SpatialAudio = false,
            MaxDevices = 1,
            MaxDownloadDevices = 1,
            AllowedDeviceTypes = "mobile,tablet",
            Description = "Mobile devices only",
            PaystackPlanCode = "PLN_mobile_monthly"
        },
        new Plan
        {
            Id = 2,
            Name = "Basic",
            Price = 4000,
            VideoQuality = "Good",
            Resolution = "720p",
            SpatialAudio = false,
            MaxDevices = 1,
            MaxDownloadDevices = 1,
            AllowedDeviceTypes = "tv,computer,mobile,tablet",
            Description = "HD streaming on 1 device",
            PaystackPlanCode = "PLN_basic_monthly"
        },
        new Plan
        {
            Id = 3,
            Name = "Standard",
            Price = 6500,
            VideoQuality = "Great",
            Resolution = "1080p",
            SpatialAudio = false,
            MaxDevices = 2,
            MaxDownloadDevices = 2,
            AllowedDeviceTypes = "tv,computer,mobile,tablet",
            Description = "Full HD streaming, 2 devices",
            PaystackPlanCode = "PLN_standard_monthly"
        },
        new Plan
        {
            Id = 4,
            Name = "Premium",
            Price = 8500,
            VideoQuality = "Best",
            Resolution = "4K + HDR",
            SpatialAudio = true,
            MaxDevices = 4,
            MaxDownloadDevices = 6,
            AllowedDeviceTypes = "tv,computer,mobile,tablet",
            Description = "Ultra HD streaming with HDR and spatial audio",
            PaystackPlanCode = "PLN_premium_monthly"
        }
        );
        await context.SaveChangesAsync();
        Console.WriteLine("Successfully seeded 4 subscriptions plans!");
    }
    else
    {
        Console.WriteLine($"Found {context.Plans.Count()} existing plans in database");
    }
}
    app.Run();
