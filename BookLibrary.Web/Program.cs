using BookLibrary.Web.Configuration;
using BookLibrary.Web.Middleware;
using BookLibrary.Web.Services;
using BookLibrary.Web.Services.Implementations;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    // Require authentication for ALL pages by default across MeroLibrary
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Register");
    options.Conventions.AllowAnonymousToPage("/Account/Logout");
    options.Conventions.AllowAnonymousToPage("/Error");
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AuthHeaderHandler>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// Configure ApiSettings from configuration
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection(ApiSettings.SectionName));

// Configure typed HttpClients via IHttpClientFactory using ApiSettings:BaseUrl
void ConfigureHttpClient(HttpClient client)
{
    var apiSettings = builder.Configuration.GetSection(ApiSettings.SectionName).Get<ApiSettings>()
        ?? throw new InvalidOperationException("ApiSettings section is missing in configuration.");

    client.BaseAddress = new Uri(apiSettings.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}

builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(ConfigureHttpClient);
builder.Services.AddHttpClient<IBookApiClient, BookApiClient>(ConfigureHttpClient)
    .AddHttpMessageHandler<AuthHeaderHandler>();
builder.Services.AddHttpClient<IAuthorApiClient, AuthorApiClient>(ConfigureHttpClient)
    .AddHttpMessageHandler<AuthHeaderHandler>();
builder.Services.AddHttpClient<ICategoryApiClient, CategoryApiClient>(ConfigureHttpClient)
    .AddHttpMessageHandler<AuthHeaderHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMiddleware<FrontendExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
