using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//keycloakconfig - start
//builder.Services.AddTransient<IClaimsTransformation, KeycloakRolesClaimTransformer>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(cookie =>
{
    cookie.Cookie.Name = "keycloakcookiemvcsample";
    cookie.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    cookie.SlidingExpiration = true;
    //cookie.LoginPath = "/Home/Index";
    cookie.Cookie.MaxAge = TimeSpan.FromMinutes(60);
})
.AddOpenIdConnect(options =>
{
    //options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //appsettings.json file dan okuyoruz;
    options.Authority = builder.Configuration.GetSection("Keycloak")["Authority"];
    options.ClientId = builder.Configuration.GetSection("Keycloak")["ClientId"];
    options.ClientSecret = builder.Configuration.GetSection("Keycloak")["ClientSecret"];

    //Programmatic olarak da verebiliriz;
    options.Authority = "http://localhost:8080/realms/testrealm/";
    options.ClientId = "KeycloakWebMVCSample";
    options.ClientSecret = "gImTCO9nM3QZIVu75DDi0qsio2FdOaLW";

    options.ResponseType = "code";
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    //options.CallbackPath = "/LoginCallback"; 
    //options.SignedOutCallbackPath = "/Home"; 
    //options.GetClaimsFromUserInfoEndpoint = true;
    options.SaveTokens = true;
    options.RequireHttpsMetadata = false;
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.NonceCookie.SameSite = SameSiteMode.Unspecified;
    //options.NonceCookie.SameSite = SameSiteMode.None;//normalde ustteki gibi , bunu bir dene
    options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;
    //options.CorrelationCookie.SameSite = SameSiteMode.None;//normalde ustteki gibi , bunu bir dene

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "preferred_username",
        RoleClaimType = "roles",
        ValidateIssuer = true
    };

    //Configuration.Bind("<Json Config Filter>", options);
    //options.Events.OnRedirectToIdentityProvider = async context =>
    //{
    //context.ProtocolMessage.RedirectUri = "https://localhost:7209/Home";
    //     await Task.FromResult(0);
    //};
}
);
//keycloakconfig - end

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
