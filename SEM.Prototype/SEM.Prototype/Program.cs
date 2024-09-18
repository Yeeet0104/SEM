using SEM.Prototype.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SEM.Prototype.Utils;
using SEM.Prototype.Controllers;
using SEM.Prototype.Services.Chatbot;
using SEM.Prototype.Services.Calc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
//builder.Services.AddSingleton<OllamaProviderLoader>();
builder.Services.AddSingleton<IChatbotService, ChatbotService>();
builder.Services.AddTransient<CalculatorService>();



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
