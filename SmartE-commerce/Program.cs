
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartE_commerce.Classes;
using SmartE_commerce.Data;
using SmartE_commerce.MiddleWares;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddTransient<WitherForcastService>();  // more than ins created
//builder.Services.AddScoped<IWitherForcastService, WitherForcastService>();     // 1 ins for rquset (defult)
//builder.Services.AddSingleton<WitherForcastService>();  // 1 ins per program
builder.Services.AddDbContext<ApplicationDbContext>(builder => builder.UseSqlServer("Server=db14374.databaseasp.net; Database=db14374; User Id=db14374; Password=4Cd_Zo%57!Kn; Encrypt=False; MultipleActiveResultSets=True;"));
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
builder.Services.AddSingleton(jwtOptions);
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, option =>
    {
        option.SaveToken = true;
        option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audiencs,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SiningKey))

        };



    });
        var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<RateLimitingMiddleware>();
app.UseMiddleware<ProfilingMiddleware>();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();



app.MapControllers();
// Excel excel = new Excel();
app.Run();
