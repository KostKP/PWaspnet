using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.Net.Http.Headers;

using aspnetapp.Helpers;
using aspnetapp.Utilities;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

PasswordUtility.Salt=builder.Configuration["PasswordHashSalt"];

// Add services to the container

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
builder.Services.AddSwaggerGen(options=> {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });

builder.Services.AddCors();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        RequireExpirationTime = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(opt => opt.UseNpgsql(connectionString));

var app = builder.Build();
app.UseCors(builder => builder.AllowAnyOrigin().WithHeaders(HeaderNames.Authorization, HeaderNames.ContentType));

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
//Console.WriteLine(System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("e3d660ca-5077-4707-806d-2bc59487c597")));
//Console.WriteLine(System.Text.ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String("ZTNkNjYwY2EtNTA3Ny00NzA3LTgwNmQtMmJjNTk0ODdjNTk3")));
app.Run();
