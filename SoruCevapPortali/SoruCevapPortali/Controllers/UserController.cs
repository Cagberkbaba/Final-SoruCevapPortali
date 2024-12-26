using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using SoruCevapPortali.Models;
using SoruCevapPortali.ViewModels;
using System.Security.Claims;
using System.Security.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace SoruCevapPortali.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly INotyfService _notify;
        private readonly IConfiguration _config;
        private readonly IFileProvider _fileProvider;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;


        public UserController(ILogger<HomeController> logger, IConfiguration config, IFileProvider fileProvider, AppDbContext context, INotyfService notify, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
        {

            _context = context;
            _config = config;
            _notify = notify;
            _logger = logger;
           _fileProvider = fileProvider; 
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public  IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new AppUser
                {

                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    CreatedAt = DateTime.Now,
                };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {

                    if(user.UserName == "Admin")
                    {
                        var role = new AppRole { Name = "Admin" };
                        await _roleManager.CreateAsync(role);
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    var roleExist = await _roleManager.RoleExistsAsync("Uye");

                    if (!roleExist)
                    {
                        var role = new AppRole { Name = "Uye" };
                        await _roleManager.CreateAsync(role);
                       
                    }

                    await _userManager.AddToRoleAsync(user, "Uye");


                    return RedirectToAction("Login", "User");
                }

                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(model);
        }




        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByEmailAsync(model.EmailOrName);

                if(user == null)
                {
                    user = await _userManager.FindByNameAsync(model.EmailOrName);
                }

                if (user != null)
                {

                    await _signInManager.SignOutAsync();

                   

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
                    if (result.Succeeded)
                    {
                        var userRole = (await _userManager.GetRolesAsync(user)).Contains("Admin") ? "Admin" : "Uye";

                        var userClaims = new List<Claim>();
                        userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        userClaims.Add(new Claim(ClaimTypes.Name, user.UserName ?? ""));
                        userClaims.Add(new Claim(ClaimTypes.GivenName, user.FullName ?? ""));
                        userClaims.Add(new Claim(ClaimTypes.UserData, user.PhotoUrl ?? ""));
                        userClaims.Add(new Claim(ClaimTypes.Role, userRole));

                        if (user.Email == "yucufer@gmail.com")
                        {
                            userClaims.Add(new Claim(ClaimTypes.Role, "Admin"));
                        }

                        var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true
                        };
                        // Eğer varsa kayıtlı cookie'yi siler
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        //Yeni cookie eklemek için
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties
                            );



                        await _userManager.ResetAccessFailedCountAsync(user);
                        await _userManager.SetLockoutEndDateAsync(user, null);
                        _notify.Success("Başarılı şekilde giriş yapıldı.");
                        if(userRole == "Admin")
                        {
                        return RedirectToAction("Index", "Admin");

                        }
                        return RedirectToAction("Index", "Question");
                    }
                    else if (result.IsLockedOut)
                    {
                        var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                        var timeLeft = lockoutDate.Value - DateTime.UtcNow;
                        ModelState.AddModelError("", $"Hesabınız kilitlendi {timeLeft.Minutes.ToString()} dakika {timeLeft.Seconds} saniye sonra giriş yapabilirsiniz.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hatalı email ya da parola!!");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Hatalı email ya da parola!!");
                }
            }


            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _notify.Success("Başarıyla çıkış yapıldı.");
            return RedirectToAction("Index","Question");
        }

        public async Task<IActionResult> UserPage()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login");
            var user= await _userManager.FindByIdAsync(userId);
            if (user == null) return RedirectToAction("Login");
            var userRoles = await _userManager.GetRolesAsync(user);

            var currentUser = await _userManager.FindByIdAsync(userId);



            var userModel = new UserModel()
            {

                UserName = currentUser.UserName ?? "",
                CreatedAt = currentUser.CreatedAt,
                FullName = currentUser.FullName,
                Email = currentUser.Email,
                PhotoUrl = currentUser.PhotoUrl,
                Role = userRoles.Contains("Admin") ? "Admin" : "Student"

            };


            return View(userModel);
        }
        [Authorize]

        [HttpPost]
        public async Task<IActionResult> UserPage(UserModel model)
        {
            var rootFolder = _fileProvider.GetDirectoryContents("wwwroot");
            var photoUrl = "-";
            if (model.PhotoFile != null && model.PhotoFile.Length > 0 )
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.PhotoFile.FileName);
                var photoPath = Path.Combine(rootFolder.First(x => x.Name == "Photos").PhysicalPath, filename);
                using var stream = new FileStream(photoPath, FileMode.Create);
                model.PhotoFile.CopyTo(stream);
                photoUrl = filename;

                var userId = _userManager.GetUserId(User);

                if (userId == null) return RedirectToAction("Login");
                // Kullanıcıyı ID'siyle veri tabanından çek
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    // Kullanıcının PhotoUrl özelliğini güncelle
                    user.PhotoUrl = photoUrl;

                    // Kullanıcıyı veri tabanında güncelle
                    var updateResult = await _userManager.UpdateAsync(user);

                    if (updateResult.Succeeded)
                    {

                        _notify.Success("Fotoğrafınız güncellendi");

                        return RedirectToAction("UserPage");
                    }


                }
            }
            return RedirectToAction("Userpage");
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(string id)
        {
            // id'nin boş veya null olup olmadığını kontrol edin
            if (string.IsNullOrEmpty(id))
                return BadRequest("Geçersiz kullanıcı ID'si.");

            // Kullanıcıyı bul
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            // Kullanıcıyı sil
            var result = await _userManager.DeleteAsync(user);

            // Silme işleminin sonucunu kontrol et
            if (!result.Succeeded)
            {
                // Hata mesajlarını toplayıp göstermek isterseniz
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest($"Kullanıcı silinemedi: {errors}");
            }

            // İşlem başarılıysa yönlendir
            return RedirectToAction("Users", "Admin");
        }


    }
}
