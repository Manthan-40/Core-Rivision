using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.PowerPlatform.Dataverse.Client;
using RevisioneNew.Interfaces;
using RevisioneNew.Services;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Events = new OpenIdConnectEvents
    {
        OnRemoteFailure = context =>
        {
            // Clear the default authentication cookie set by ASP.NET Core's cookie middleware
            context.Response.Cookies.Delete(CookieAuthenticationDefaults.CookiePrefix + CookieAuthenticationDefaults.AuthenticationScheme);

            // Optionally, clear other related cookies (if any), or loop through all cookies to delete them
            foreach (var cookie in context.Request.Cookies.Keys)
            {
                context.Response.Cookies.Delete(cookie);
            }
            context.Response.Redirect("/login");
            context.HandleResponse(); // Prevents further processing
            return Task.CompletedTask;
        },
        OnSignedOutCallbackRedirect = context =>
        {
            context.Response.Redirect("/login");
            context.HandleResponse(); // Prevents further processing
            return Task.CompletedTask;
        }
    };
});

builder.Services.Configure<CookieAuthenticationOptions>(options =>
{
    
});

builder.Services.AddScoped<ServiceClient>(option =>
{
        string connectionString = builder.Configuration["ConnectionStrings:default"].ToString();
        return new ServiceClient(connectionString);
});


builder.Services.AddScoped<IServiceInterface, ServiceHelper>();

builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

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
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");
app.MapRazorPages();

app.Run();
