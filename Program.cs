using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using raceday_management_system.Database;
using raceday_management_system.Services;

var builder = WebApplication.CreateBuilder(args);

//register database helpers
builder.Services.AddScoped<auth_connection>();
builder.Services.AddScoped<users_connection>();
builder.Services.AddScoped<events_connection>();
builder.Services.AddScoped<categories_connection>();
builder.Services.AddScoped<enrolments_connection>();
builder.Services.AddScoped<results_connection>();
builder.Services.AddScoped<dashboard_connection>();

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

//add controllers with enum-as-string support
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//configure swagger with proper documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //basic API info
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "raceday_management_system",
        Version = "v1",
        Description = "RESTful API for the RaceDay event management system. " +
                      "Supports Organiser and Participant roles with session-based authentication.",
        Contact = new OpenApiContact
        {
            Name = "RaceDay Team"
        }
    });

    //load the XML documentation file so Swagger shows our comments
    var xml_file = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xml_path = Path.Combine(AppContext.BaseDirectory, xml_file);

    if (File.Exists(xml_path))
    {
        options.IncludeXmlComments(xml_path);
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "raceday_management_system v1");
        options.DocumentTitle = "raceday_management_system API Documentation";
    });
}

app.UseHttpsRedirection();
app.UseSession();
app.UseAuthorization();
app.MapControllers();

app.Run();