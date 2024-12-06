using BudgetTracker.Service;
using BudgetTRacker.Data;
using BudgetTRacker.Service;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Add Security 
builder.Services.AddAuthentication(
    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults
    .AuthenticationScheme
    ).AddCookie(s =>
    {
        s.LoginPath = "/SignIn";
        s.LogoutPath = "/SignOut";
    });

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthorization();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation(); 

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<BankTransactionDataService>();
builder.Services.AddHttpClient<AccountDetailsDataService>();
builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<CashTransactionDataService>();
builder.Services.AddScoped<AddCashDateService>();
builder.Services.AddScoped<CategoryDataService>();
builder.Services.AddScoped<UserDataService>();
builder.Services.AddScoped<IAuthService, AuthService>();
var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

//app.MapGet("/", context =>
//{
//    context.Response.Redirect("/SignIn");
//    return Task.CompletedTask;
//});

app.MapRazorPages();

app.Run();
