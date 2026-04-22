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
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("Paystack", client =>
{
    client.BaseAddress = new Uri("https://api.paystack.co/");
    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + builder.Configuration["PayStack:SecretKey"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token in the text box below"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
        new string[]{}
    }
});

});
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
    var authHeader = context.Request.Headers["Authorization"].ToString();
    LoggedInUser.Token = authHeader.Replace("Bearer ", "").Trim();

    if (!string.IsNullOrEmpty(LoggedInUser.Token))
    {
        var handler = new JwtSecurityTokenHandler();
        if (handler.CanReadToken(LoggedInUser.Token))
        {
            var token = handler.ReadToken(LoggedInUser.Token) as JwtSecurityToken;
            if (token != null)
            {
                LoggedInUser.UserId = Convert.ToInt16(token.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value ?? "-1");
            }
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

    Console.WriteLine("Checking for new movies...");
    var seedMovies = new List<Movie>
       {
        new Movie
        {
            Title = "Arcane",
            Description = "A story about two sisters in a divided city.",
            Genre = "Animation",
            ReleaseYear = 2021,
            ThumbnailUrl = "https://exmple.com",
            VideoUrl = "https://example.com"
        },
        new Movie
        {
            Title = "Invincible",
            Description = "A teenager discovers his father is the most powerful superhero on the planet.",
            Genre = "Action, Animation",
            ReleaseYear = 2021,
            ThumbnailUrl = "https://example.com",
            VideoUrl = "https://example.com"

        },
        new Movie
        {
            Title = "The Boys",
            Description = "A group of misfits set out to take down corrupt superheroes.",
            Genre = "Action, Drama",
            ReleaseYear = 2019,
            ThumbnailUrl = "https://example.com",
            VideoUrl = "https://example.com"

        },
        new Movie 
        {
            Title = "Stranger Things",
            Description = "A group of kids uncover supernatural mysteries in their small town.",
            Genre = "Sci-Fi, Horror",
            ReleaseYear = 2016,
            ThumbnailUrl = "https://example.com",
            VideoUrl = "https://example.com"
        },
        new Movie
        {
            Title = "Money Heist",
            Description = "A criminal mastermind plans the biggest heist in history.",
            Genre = "Thriller, Crime",
            ReleaseYear = 2017,
            ThumbnailUrl = "https://example.com",
            VideoUrl = "https://example.com"
        },
        new Movie
        {
            Title = "The Witcher",
            Description = "A monster hunter struggles to find his place in a world where people often prove more wicked than beasts.",
            Genre = "Fantasy, Action",
            ReleaseYear = 2019,
            ThumbnailUrl = "https://example.com",
            VideoUrl = "https://example.com"
        },
        new Movie
        {
            Title = "Squid Game",
            Description = "Hundreds of cash-strapped contestants accept an invitation to compete in children's games for a tempting prize, but the stakes are deadly.",
            Genre = "Horror, Thriller",
            ReleaseYear = 2021,
            ThumbnailUrl = "https://example.com",
            VideoUrl = "https://example.com"
        }
       };

        foreach (var movie in seedMovies)
    {
        var exists = await context.Movies.AnyAsync(m => m.Title == movie.Title);
        if (!exists)
        {
            Console.WriteLine($"Adding new movie: {movie.Title}");
            context.Movies.Add(movie);
        }
    }
        await context.SaveChangesAsync();
    Console.WriteLine("Movie sync complete!");
}
    app.Run();
