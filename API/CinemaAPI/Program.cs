using CinemaAPI.Connection;
using CinemaAPI.Hubs;
using CinemaAPI.Interfaces;
using CinemaAPI.Services;
using CinemaAPI.UniversalMethods;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddDbContext<ContextDb>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("CinemaConnection")));

builder.Services.AddSingleton<JwtGenerator>();
builder.Services.AddSingleton<PasswordService>();
builder.Services.AddSingleton<QrCodeMaker>();

builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IPublicService, PublicService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICashierService, CashierService>();
builder.Services.AddScoped<IControllerService, ControllerService>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCinema", policy =>
    {
        policy.WithOrigins("http://localhost:5191")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.WebHost.UseUrls("http://localhost:5190");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowCinema");

var uploadsDir = Path.Combine(app.Environment.ContentRootPath, "uploads");
Directory.CreateDirectory(uploadsDir);
app.UseStaticFiles(new Microsoft.AspNetCore.Builder.StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsDir),
    RequestPath = "/uploads"
});

app.MapControllers();
app.MapHub<CinemaHub>("/cinemaHub");

app.Run();
