// wj-api/Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient("CinemaWorld", client =>
{
    client.BaseAddress = new Uri("https://webjetapitest.azurewebsites.net/api/cinemaworld/");
    // client.DefaultRequestHeaders.Add("x-access-token", 
    //     builder.Configuration["ApiTokens:ApiKey"]);
});

builder.Services.AddHttpClient("FilmWorld", client =>
{
    client.BaseAddress = new Uri("https://webjetapitest.azurewebsites.net/api/filmworld/");
    // client.DefaultRequestHeaders.Add("x-access-token", 
    //     builder.Configuration["ApiTokens:ApiKey"]);
});

builder.Services.AddScoped<wj_api.Services.IMovieService, wj_api.Services.MovieService>();
builder.Services.AddHttpContextAccessor();
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader();
        Console.WriteLine("CORS policy configured for http://localhost:3000");
    });
});

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});

var app = builder.Build();

// Configure the HTTP request pipeline
// app.UseHttpsRedirection(); // Comment out or remove this line
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();