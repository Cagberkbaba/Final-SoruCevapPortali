using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoruCevapPortali.Models;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using SoruCevapPortali.Repository.Concrete;
using SoruCevapPortali.Repository.Abstract;
using Microsoft.Extensions.FileProviders;
using SoruCevapPortali.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });



builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("sqlCon"));


});

builder.Services.AddScoped<ICategoryRepository, EfCategoryRepository>();
builder.Services.AddScoped<IQuestionRepository, EfQuestionRepository>();

builder.Services.AddSignalR();

builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); // add default token providers ile gerekli token bilgisini üretme Özelliðini ekliyoruz.


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    // email'i benzersiz yapma
    options.User.RequireUniqueEmail = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(4);
    options.Lockout.MaxFailedAccessAttempts = 5;
   
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Users/Login";
});

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/User/Login";
    options.AccessDeniedPath = "/Home/AccesDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true; // uygulamaya her giriþ yapýldýðýnda 30 günlük süreyi yeniler
    
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(3);
});

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
app.UseNotyf();
app.UseAuthentication();

app.MapHub<TodoHub>("/todoHub");

app.UseAuthorization();




app.MapControllerRoute(
    name: "my_questions_by_category",
    pattern: "Admin/Category/{category}",
    defaults: new { controller = "Admin", action = "Questions" }
    );
app.MapControllerRoute(
    name: "my_questions_by_category",
    pattern: "MyQuestions/Category/{category}",
    defaults: new { controller = "Question", action = "MyQuestions" }
    );
app.MapControllerRoute(
    name: "questions_by_category",
    pattern: "Questions/Category/{category}",
    defaults: new { controller = "Question", action = "Index" }
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Question}/{action=Index}/{id?}"
    );

await SeedData.IdentitySeeddata(app);
app.Run();
