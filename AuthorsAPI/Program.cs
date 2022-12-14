using AuthorsAPI.Configurations;
using AuthorsAPI.Context;
using AuthorsAPI.IRepository;
using AuthorsAPI.Model;
using AuthorsAPI.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
   builder.SetIsOriginAllowed(origin => true)
   .AllowCredentials()
   .AllowAnyMethod()
   .AllowAnyHeader());
});

builder.Services.AddIdentity<AuthorModel, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    string connStr;

    if (env == "Development")
    {
        connStr = builder.Configuration.GetConnectionString("DefaultConnection");
    }
    else
    {
        connStr = builder.Configuration.GetConnectionString("LiveConnection");

        // Use connection string provided at runtime by Heroku.
        //var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

        //connUrl = connUrl.Replace("postgres://", string.Empty);
        //var userPassSide = connUrl.Split("@")[0];
        //var hostSide = connUrl.Split("@")[1];

        //var user = userPassSide.Split(":")[0];
        //var password = "fdf12756a52bc5a34a212c7ad9453a0b9b8ebb4fabccfd1720e3643fffd389e9";
        //var host = hostSide.Split("/")[0];
        //var database = hostSide.Split("/")[1].Split("?")[0];

        //connStr = $"Server=ec2-54-87-99-12.compute-1.amazonaws.com;Database={database};User ID={user};Password={password};Port=5432;TrustServerCertificate=true;sslmode=Require";
    }

    options.UseNpgsql(connStr)
    ;
});

builder.Services.AddAutoMapper(typeof(MapperInitializer));

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    //options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {

                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["keyjwt"])),
                        ClockSkew = TimeSpan.Zero

                    };
                });


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthorAPI", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
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

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
