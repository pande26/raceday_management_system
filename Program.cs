using System.Text.Json.Serialization;
using raceday_management_system.Database;
using raceday_management_system.Services;

var builder = WebApplication.CreateBuilder(args);

//register services for authentication
builder.Services.AddScoped<password_hasher>();
builder.Services.AddScoped<auth_connection>();

//configure session services
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//add controllers with enum-as-string support
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//configure swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//enable session middleware
app.UseSession();

app.UseAuthorization();

app.MapControllers();

app.Run();