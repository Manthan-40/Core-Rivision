using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.PowerPlatform.Dataverse.Client;

var builder = WebApplication.CreateBuilder(args);

//var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ') ?? builder.Configuration["MicrosoftGraph:Scopes"]?.Split(' ');

//builder.Services.AddAuthentication(option => { option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; option.DefaultScheme = OpenIdConnectDefaults.AuthenticationScheme; }).AddCookie(x => { x.LoginPath = "/Account/Login"; });

//builder.Services.AddAuthentication()
//    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"), OpenIdConnectDefaults.AuthenticationScheme, "ADCookies");

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

//builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();


builder.Services.AddScoped<ServiceClient>(option =>
{
    string connectionString = builder.Configuration["ConnectionStrings:default"].ToString();
    return new ServiceClient(connectionString);
    //return new ServiceClient(connectionString);
});

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");
app.MapRazorPages();

app.Run();
