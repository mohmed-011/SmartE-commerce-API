
using Microsoft.EntityFrameworkCore;
using SmartE_commerce.Data;
using SmartE_commerce.MiddleWares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddTransient<WitherForcastService>();  // more than ins created
//builder.Services.AddScoped<IWitherForcastService, WitherForcastService>();     // 1 ins for rquset (defult)
//builder.Services.AddSingleton<WitherForcastService>();  // 1 ins per program
builder.Services.AddDbContext<ApplicationDbContext>(builder => builder.UseSqlServer("server=.;database=Smart_EcommerceV2;integrated security =true; trust server certificate = true "));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<RateLimitingMiddleware>();
app.UseMiddleware<ProfilingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();



app.MapControllers();

app.Run();
