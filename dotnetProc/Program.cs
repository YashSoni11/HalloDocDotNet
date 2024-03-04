using HalloDoc_DAL.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using dotnetProc.Controllers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HalloDoc_BAL.Interface;
using HalloDoc_BAL.Repositery;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSession();
builder.Services.AddControllersWithViews();
var con = builder.Configuration.GetConnectionString("PSSQL");

builder.Services.AddDbContext<HalloDocContext>(q => q.UseNpgsql(con));
builder.Services.AddScoped<IAccount, Account>();
builder.Services.AddScoped<IAdmindashboard, Admindashboard>();
builder.Services.AddScoped<IPatientReq,PatientRequest>();
builder.Services.AddTransient<IEmailService,EmailService>();


var app = builder.Build();

// Configure the HTTP request pipeline.




if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
