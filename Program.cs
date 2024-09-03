using System.Text;
using DotNetWebApp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors((options) =>
{
    options.AddPolicy("DevCors" , (corsBuilder) =>
    {
        corsBuilder.WithOrigins("http://localHost:4200", "http://localHost:3000", "http://localHost:8000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
    
    options.AddPolicy("ProdCors" , (corsBuilder) =>
    {
        corsBuilder.WithOrigins("http://myProductions.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddScoped<IUserRepository,UserRepository>(); //Can be deleted

string? tokenKeyString = builder.Configuration.GetSection("AppSettings:TokenKey").Value;

var tokenKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(tokenKeyString ?? "")
);


TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
{
    IssuerSigningKey = tokenKey,
    ValidateIssuer = false,
    ValidateIssuerSigningKey = false, // used for creating tokens outside the API
    ValidateAudience = false
};

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = tokenValidationParameters;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevCors");
}
else
{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// app.MapGet("/weatherforecast", () =>
//     {
//     })
//     .WithName("GetWeatherForecast")
//     .WithOpenApi();

app.Run();

