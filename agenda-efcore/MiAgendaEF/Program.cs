using DataAccess.Contract;
using DataAccess.Implementation;
using DataAccess.Implementation.Base;
using Infrastructure.Contract;
using Infrastructure.Helpers;
using Infrastructure.Implementation;
using Infrastructure.Implementation.Security;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MI_AGENDA_CONNECTION")));

builder.Services.AddScoped<IMiAgendaInfrastructure, MiAgendaInfrastructure>();
builder.Services.AddScoped<IMiAgendaDataAccess, MiAgendaDataAccess>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ILocalFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IRedSocialHelper, RedSocialHelper>();


//Agregar autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

//Agregar autorización
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
//builder.Services.AddDistributedMemoryCache();

//Loggings
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Account/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
