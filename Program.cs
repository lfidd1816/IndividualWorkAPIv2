using System.Text.Json;
using Amazon;           
using Amazon.S3;
using Amazon.Runtime;      
using IndividualWorkAPI.DatabaseContext;
using IndividualWorkAPI.Interfaces;
using IndividualWorkAPI.Services;
using IndividualWorkAPI.Settings;
using IndividualWorkAPI.UniversalMethods;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(); 

builder.Services.AddDbContext<ContextDatabase>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TestDbString")));

var s3Config = new AmazonS3Config
{
    ServiceURL = "https://storage.yandexcloud.net",
    ForcePathStyle = true, 
    Timeout = TimeSpan.FromMinutes(10)
};

var s3Client = new AmazonS3Client(
    builder.Configuration["S3:AccessKey"],
    builder.Configuration["S3:SecretKey"],
    s3Config
);

builder.Services.AddSingleton<IAmazonS3>(s3Client);

builder.Services.Configure<S3Settings>(builder.Configuration.GetSection("S3"));
builder.Services.AddScoped<IS3ContainerService, S3ContainerService>();

builder.Services.AddScoped<IVideoService, VideoService>(); 
builder.Services.AddSingleton<JWTTokenGenerator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod(); 
    });
});

// JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Redirect("/swagger"));
app.UseStaticFiles();
app.UseCors("AllowBlazorWasm");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();