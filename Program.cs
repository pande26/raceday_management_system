using System.Text.Json.Serialization;
using raceday_management_system.Database;
using raceday_management_system.Services;

var builder = WebApplication.CreateBuilder(args);

//register database helpers
builder.Services.AddScoped<auth_connection>();
builder.Services.AddScoped<users_connection>();
builder.Services.AddScoped<events_connection>();
builder.Services.AddScoped<categories_connection>();

//register services
builder.Services.AddScoped<password_hasher>();

//configure session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSession();
app.UseAuthorization();
app.MapControllers();

app.Run();