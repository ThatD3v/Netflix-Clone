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
    builder.Configuration.GetConnectionString("DefaultConection"),
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

app.Run();
