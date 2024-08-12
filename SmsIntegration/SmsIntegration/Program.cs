using Microsoft.EntityFrameworkCore;
using SmsIntegration.Data;
using SmsIntegration.Models;
using SmsIntegration.Data;
using SmsIntegration.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register TwilioSettings service
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Sms}/{action=Register}/{id?}");

app.Run();
