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

    //The Movies Section
    Console.WriteLine("Checking for new movies...");
    var seedMovies = new List<Movie>
       {
        new Movie
        {
            Title = "Dune",
            Description = "A story about paul atreides and how he became the lisan al-gaib. ",
            Genre = "Action, Fantasy, Drama",
            ReleaseYear = 2021,
            ThumbnailUrl = "https://exmple.com",
            VideoUrl = "https://example.com"
        },
        new Movie
        {
            Title = "The Rip",
            Description = "A movie about the truth of what goes on when dealing with stash houses by the Feds. ",
            Genre = "Action, Drama, Mystery",
            ReleaseYear = 2026,
            ThumbnailUrl = "https://example.com",
            VideoUrl = "https://example.com"

        },
        new Movie
        {
            Title = "Spiderman : Into The Spiderverse",
            Description = "Miles Morales becomes Newyorks new webslinger. well, he's not the only one. ",
            Genre = "Animation, Action",
            ReleaseYear = 2018,
            ThumbnailUrl = "https://example.com",
            VideoUrl = "https://example.com"

        },
        new Movie 
        {
            Title = "The Bad Guys",
            Description = "A band of misfits set out to see if they can be anything else asides from bad. ",
            Genre = "Animation, Comedy,",
            ReleaseYear = 2022,
            ThumbnailUrl = "https://example.com",
            VideoUrl = "https://example.com"
        },
        new Movie
        {
            Title = "Interstellar",
            Description = "A father and daugther's love is tested when time dilation comes into play. ",
            Genre = "Suspense, Sci-fi",
            ReleaseYear = 2013,
            ThumbnailUrl = "https://example.com",
            VideoUrl = "https://example.com"
        },
        new Movie
        {
            Title = "The Dark Knight",
            Description = "The Caped crusader meets his ultimate nemesis; The Joker. ",
            Genre = "Action, Thriller, Superhero",
            ReleaseYear = 2008,
            ThumbnailUrl = "https://example.com",
            VideoUrl = "https://example.com"
        },
        new Movie
        {
            Title = "Oppenheimer",
            Description = "The truth behind the manhattan project and what actually transpired. ",
            Genre = "Biography, Thriller, Drama",
            ReleaseYear = 2023,
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

    //The Series Section
    var seriesList = new List<Series>
    {
        new Series
        {
            Title = "Arcane",
            Description = "The Story of how two beloved character's from the hit game 'League Of Legends' came to be.",
            Seasons = new List<Season>
            {
                new Season
                {
                    SeasonNumber = 1,
                    Episodes = new List<Episode>
                    {
                        new Episode
                        {
                            EpisodeNumber =1,
                            Title = "...",
                            Description = "...",
                            VideoUrl = "..."
                        },
                        new Episode
                        {
                            EpisodeNumber =2,
                            Title = "...",
                            Description="...",
                            VideoUrl="..."
                        },
                        new Episode
                        {
                            EpisodeNumber =3,
                            Title = "...",
                            Description = "...",
                            VideoUrl = "..."
                        }
                    }
                }
            }
        },
        new Series
        {
            Title = "House Of The Dragon",
            Description ="The story of the Targaryen Civil War that led to near extinction of the Valyrians and thier dragons. ",
            Seasons = new List<Season>
            {
                new Season
                {
                    SeasonNumber = 1,
                    Episodes = new List<Episode>
                    {
                        new Episode
                        {
                           EpisodeNumber =1,
                           Title = "The Heirs of the Dragon",
                           Description = "The Princess learns she's going to be a sister to a boy. A Prince.",
                           VideoUrl = "https://example.com"
                        },
                        new Episode
                        {
                           EpisodeNumber =2,
                           Title = "The Rogue Prince",
                           Description = "Daemon takes matters to his hands when the court. ",
                           VideoUrl = "https://example.com"
                        },
                        new Episode
                        {
                           EpisodeNumber =3,
                           Title = "Second of His Name",
                           Description = "Viserys mourns continues to mourn the death of his son. ",
                           VideoUrl = "https://example.com"
                        },
                    }
                },
                new Season
                {
                    SeasonNumber = 2,
                    Episodes = new List<Episode>
                    {
                        new Episode
                        {
                           EpisodeNumber =1,
                           Title = "A Son for a Son. ",
                           Description = "Rhaenyra strategises how to take back her throne from her brother, Aegon. ",
                           VideoUrl = "https://example.com"
                        },
                        new Episode
                        {
                           EpisodeNumber = 2,
                           Title = "Rhaenyra the cruel",
                           Description = "The princess faces her biggest foe yet. herself.",
                           VideoUrl =  "https://example.com"
                        }
                    }
                }
            }
        }
        
    };
    foreach (var series in seriesList)
    {
        if (!await context.Series.AnyAsync(s => s.Title == series.Title))
        {
            context.Series.Add(series);
            Console.WriteLine($"Adding Series: {series.Title}");
        }
    }
    await context.SaveChangesAsync();
    Console.WriteLine("Everything has been saved!");
}
    app.Run();
