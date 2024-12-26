using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SoruCevapPortali.Models;
using SoruCevapPortali.Repository.Abstract;
using SoruCevapPortali.ViewModels;

namespace SoruCevapPortali.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly IFileProvider _fileProvider;

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotyfService _notify;
        private readonly IQuestionRepository _questionRepository;
        private readonly AppDbContext _context;

        public AdminController(ICategoryRepository categoryRepository, INotyfService notify, IQuestionRepository questionRepository, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, AppDbContext context, IFileProvider fileProvider)
        {
            _notify = notify;
            _userManager = userManager;
            _roleManager = roleManager;
            _categoryRepository = categoryRepository;
            _questionRepository = questionRepository;
            _context = context;
            _fileProvider = fileProvider;
        }



        public IActionResult Index()
        {
            var userCount = _userManager.Users.Count();
            var questionCount = _questionRepository.Questions.Count();
            var answerCount = _context.Answers.Count();
            var todoCount = _context.Todos.Count();
            var model = new DashboardModel
            {
                AnswerCount = answerCount,
                QuestionCount = questionCount,
                UserCount = userCount,
                TodoCount = todoCount
            };
            return View(model);
        }

        public async Task<IActionResult> Todo()
        {
            var todos = await _context.Todos.ToListAsync();

            return View(todos);
        }
        public async Task<IActionResult> Users(string searchString)
        {
            var users = await _userManager.Users.Select(x => new UserModel()
            {
                Id = x.Id,
                FullName = x.FullName,
                UserName = x.UserName,
                Email = x.Email,
                QuestionCount = x.Questions.Count(),
                CreatedAt = x.CreatedAt

            }).ToListAsync();
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(p => p.UserName.ToLower().Contains(searchString.ToLower())).ToList();

            }

            return View(users);
        }
        public async Task<IActionResult> Questions(string category)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();

            var claims = User.Claims;
            var questions = _questionRepository.Questions;
            if (!string.IsNullOrEmpty(category))
            {
                var category2 = _context.Categories.FirstOrDefault(c => c.CategoryName == category);
                if (category2 == null) return NotFound();
                questions = questions.Where(x => x.CategoryId == category2.CategoryId);
            }
            var questionList = await questions.Include(y => y.AppUser).Include(x => x.Answers).ThenInclude(a => a.AppUser).ToListAsync();
            return View(questionList);

        }
        public async Task<IActionResult> EditQuestion(int id)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();

            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "User");

            var question = await _questionRepository.Questions.Where(x => x.QuestionId == id).Select(x => new QuestionModel()
            {
                QuestionId = x.QuestionId,
                CategoryId = x.CategoryId,
                QuestionText = x.QuestionText,
                QuestionTitle = x.QuestionTitle,


            }).FirstOrDefaultAsync();

            if (question == null) return NotFound();


            return View(question);
        }
        [HttpPost]
        public async Task<IActionResult> EditQuestion(QuestionModel model)
        {
            if (ModelState.IsValid)
            {

                try
                {

                    var userId = _userManager.GetUserId(User);
                    if (userId == null) return RedirectToAction("Login", "User");
                    model.AppUserId = userId;

                    await _questionRepository.EditQuestion(model);

                }
                catch (Exception ex)
                {
                    _notify.Success("Soru kaydedilirken hata oluştu. " + ex.Message);
                    return RedirectToAction("Questions", "Admin");

                }
                _notify.Success("Soru başarılı şekilde kaydedildi.");



                return RedirectToAction("Questions", "Admin");

            }
            return View();
        }

        public async Task<IActionResult> UserPage()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login","User");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return RedirectToAction("Login","User");
            var userRoles = await _userManager.GetRolesAsync(user);

            var currentUser = await _userManager.FindByIdAsync(userId);



            var userModel = new UserModel()
            {

                UserName = currentUser.UserName ?? "",
                CreatedAt = currentUser.CreatedAt,
                FullName = currentUser.FullName,
                Email = currentUser.Email,
                PhotoUrl = currentUser.PhotoUrl,
                Role = userRoles.Contains("Admin") ? "Admin" : "Uye"

            };


            return View(userModel);
        }
        [HttpPost]
        public async Task<IActionResult> UserPage(UserModel model)
        {
            var rootFolder = _fileProvider.GetDirectoryContents("wwwroot");
            var photoUrl = "-";
            if (model.PhotoFile != null && model.PhotoFile.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.PhotoFile.FileName);
                var photoPath = Path.Combine(rootFolder.First(x => x.Name == "Photos").PhysicalPath, filename);
                using var stream = new FileStream(photoPath, FileMode.Create);
                model.PhotoFile.CopyTo(stream);
                photoUrl = filename;

                var userId = _userManager.GetUserId(User);

                if (userId == null) return RedirectToAction("Login", "User");
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

                        _notify.Success("Fotoğraf güncellendi");

                        return RedirectToAction("UserPage");
                    }


                }
            }
            return RedirectToAction("Userpage");
        }
    }
}
