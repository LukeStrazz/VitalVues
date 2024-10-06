using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Services.Interfaces;
using Services.Services;
using Auth0.AspNetCore.Authentication;
using VitalVues.Support;
using VVData.Data;
using Microsoft.Extensions.DependencyInjection;
using Hangfire; // Add Hangfire namespace


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

var secretsPath = builder.Configuration["Secrets"];
builder.Configuration.AddJsonFile(secretsPath, optional: false, reloadOnChange: true);

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
});

builder.Services.ConfigureSameSiteNoneCookies();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IBloodworkService, BloodworkService>();
builder.Services.AddScoped<IFastingService, FastService>();

builder.Services.AddScoped<IMailService, MailService>(); // Register the MailService

// Configure Hangfire services
builder.Services.AddHangfire(configuration => configuration
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

builder.Services.AddHangfireServer(); // Add the processing server

// Register the SendGridEmailService
builder.Services.AddTransient<SendGridEmailService>(provider =>
    new SendGridEmailService(builder.Configuration["SendGrid:SendGrid_API_Key"]));


builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<DatabaseContext>();

    dbContext.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseCookiePolicy();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Enable the Hangfire Dashboard for monitoring background jobs
app.UseHangfireDashboard(); 

// Enqueue a test job
//BackgroundJob.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));


app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapControllerRoute(
    name: "notifications",
    pattern: "send-notification",
    defaults: new { controller = "Notification", action = "SendNotification" });

    endpoints.MapControllerRoute(
        name: "sendgrid",
        pattern: "sendgrid-notification",
        defaults: new { controller = "SendGridNotification", action = "SendEmail" });

});

app.Run();

