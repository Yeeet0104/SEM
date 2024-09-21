using Microsoft.EntityFrameworkCore;
using SEM.Prototype.Hubs;
using SEM.Prototype.Models;
using SEM.Prototype.Services.Chatbot;
using System;
using SEM.Prototype.Services.Calc;
using SEM.Prototype.Services.CourseExplorer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
//builder.Services.AddSingleton<OllamaProviderLoader>();
builder.Services.AddSingleton<IChatbotService, ChatbotService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));
builder.Services.AddTransient<CalculatorService>();
builder.Services.AddSingleton<CourseService>();


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

app.MapHub<ChatbotHub>("/chatbotHub");



app.Run();
