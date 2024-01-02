using AngleSharp;
using EmailRep.NET;
using Microsoft.EntityFrameworkCore;
using Secure_The_Code.Data;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Connection String


builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#endregion

#region Email Rep


var settings = new EmailRepClientSettings();

builder.Configuration.GetSection(nameof(EmailRepClientSettings)).Bind(settings);

builder.Services.AddSingleton(settings);

builder.Services.AddHttpClient<IEmailRepClient, EmailRepClient>();

#endregion


var app = builder.Build();




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
