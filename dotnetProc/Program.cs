using HalloDoc_DAL.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using dotnetProc.Controllers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HalloDoc_BAL.Interface;
using HalloDoc_BAL.Repositery;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(60000);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddControllersWithViews();
var con = builder.Configuration.GetConnectionString("PSSQL");

builder.Services.AddDbContext<HalloDocContext>(q => q.UseNpgsql(con));
builder.Services.AddScoped<IAccount, Account>();
builder.Services.AddScoped<IAdmindashboard, Admindashboard>();
builder.Services.AddScoped<IProviderDashboard, ProviderDashboardServices>();
builder.Services.AddScoped<IPatientReq,PatientRequest>();
builder.Services.AddScoped<IProvider,ProviderServices>();
builder.Services.AddScoped<IAuthManager,AuthManager>();
builder.Services.AddTransient<IEmailService,EmailService>();
builder.Services.AddTransient<IJwtServices, JwtServices>();

builder.Services.AddAutoMapper(typeof(Program));


var app = builder.Build();

// Configure the HTTP request pipeline.




if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Admin"))
    {
        context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        context.Response.Headers.Add("Pragma", "no-cache");
        context.Response.Headers.Add("Expires", "0");
    }

    if (context.Request.Path.StartsWithSegments("/Provider"))
    {
        context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        context.Response.Headers.Add("Pragma", "no-cache");
        context.Response.Headers.Add("Expires", "0");
    }

    await next.Invoke();
});

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
