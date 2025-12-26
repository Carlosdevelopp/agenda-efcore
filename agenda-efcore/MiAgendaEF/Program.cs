using DataAccess.Contract;
using DataAccess.Implementation;
using DataAccess.Implementation.Base;
using Infrastructure.Contract;
using Infrastructure.Implementation;
using Infrastructure.Implementation.Security;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MI_AGENDA_CONNECTION")));

builder.Services.AddScoped<IMiAgendaInfrastructure, MiAgendaInfrastructure>();
builder.Services.AddScoped<IMiAgendaDataAccess, MiAgendaDataAccess>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

//builder.Services.AddHttpContextAccessor();
//builder.Services.AddDistributedMemoryCache();

//Loggings
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Account/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
