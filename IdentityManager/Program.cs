using IdentityManager.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityManager", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Type = SecuritySchemeType.Http,
            Name = "Authorization",
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
    
    opt.AddSecurityRequirement( new OpenApiSecurityRequirement
        { 
            {  new OpenApiSecurityScheme
             {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
            }
        });
});
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthorization();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();


app.MapGroup("/identity").MapIdentityApi<IdentityUser>();
var summaries = new[]
{
    "FreeBSD", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast(
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            )).ToArray();
        
        return forecast;
    })
    .RequireAuthorization()
    .WithName("GetWeatherForecast");


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}