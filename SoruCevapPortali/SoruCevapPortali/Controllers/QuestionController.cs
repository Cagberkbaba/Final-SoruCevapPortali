using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SoruCevapPortali.Models;
using SoruCevapPortali.Repository.Abstract;
using SoruCevapPortali.ViewComponents;
using SoruCevapPortali.ViewModels;

namespace SoruCevapPortali.Controllers
{
    public class QuestionController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INotyfService _notify;
        private readonly IQuestionRepository _questionRepository;
        private readonly AppDbContext _context;

        public QuestionController(ICategoryRepository categoryRepository, INotyfService notify, IQuestionRepository questionRepository, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, AppDbContext context)
        {
            _notify = notify;
            _userManager = userManager;
            _roleManager = roleManager;
            _categoryRepository = categoryRepository;
            _questionRepository = questionRepository;
            _context = context;
        }
        public async Task<IActionResult> Index(string category)
        {
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

        [Authorize]

        public async Task<IActionResult> MyQuestions(string category)
        {
            var userId = _userManager.GetUserId(User);

            // Kullanıcının sorularını filtreleme
            var myQuestions = _context.Questions
                .Include(a => a.AppUser)
                .Include(y => y.Answers)
                .ThenInclude(z => z.AppUser)
                .AsQueryable(); // IQueryable olarak başlat

            // Kategoriye göre filtreleme
            if (!string.IsNullOrEmpty(category))
            {
                var categoryObj = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryName == category);
                if (categoryObj == null) return NotFound();

                myQuestions = myQuestions.Where(x => x.CategoryId == categoryObj.CategoryId && x.AppUserId == userId);
            }
            else
            {
                myQuestions = myQuestions.Where(x => x.AppUserId == userId);
            }

            // Veriyi liste olarak çek ve ViewBag'e kategorileri aktar
            ViewBag.Categories = await _context.Categories.ToListAsync();
            var questionList = await myQuestions.ToListAsync();

            return View(questionList); // Doğru türde model döndür
        }
        [Authorize]

        public async Task<IActionResult> MyAnswers()
        {
            var userId = _userManager.GetUserId(User);

            var myQuestions = await _context.Answers.Where(x => x.AppUserId == userId).Include(a => a.Question).ToListAsync();

            return View(myQuestions);
        }
        public async Task<IActionResult> Details(int id)
        {
            var question = await _context.Questions.Where(x => x.QuestionId == id).Include(p => p.AppUser).Include(a => a.Answers).ThenInclude(y => y.AppUser).FirstOrDefaultAsync();
            return View(question);
        }
        [Authorize]

        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Answers) // Cevapları da dahil et
                .FirstOrDefaultAsync(x => x.QuestionId == id);

            if (question == null)
            {
                _notify.Error("Böyle bir soru bulunamamakta");
                return RedirectToAction("Index", "Question");
            }

            try
            {
                // Önce cevaplardaki ilgili kayıtları sil
                _context.Answers.RemoveRange(question.Answers);

                // Ardından soruyu sil
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();

                _notify.Success("Soru başarılı şekilde silindi.");
            }
            catch (Exception ex)
            {
                _notify.Error("Soru silinemedi: " + ex.Message);
                return RedirectToAction("Index", "Question");
            }
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);  // Roles'ları async şekilde alıyoruz
                if (userRoles.Contains("Admin"))  // Admin rolüne sahip mi diye kontrol ediyoruz
                {
                    return RedirectToAction("Questions", "Admin");
                }
            }

            return RedirectToAction("MyQuestions");
        }
        [Authorize]

        public async Task<IActionResult> Create()
        {
            
            
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }
        [Authorize]

        [HttpPost]
        public async Task<IActionResult> Create(QuestionModel model)
        {
            if (ModelState.IsValid)
            {
                

                var userId = _userManager.GetUserId(User);
                if (userId == null) return RedirectToAction("Login","User");
                var question = new Question()
                {
                    QuestionTitle = model.QuestionTitle,
                    QuestionText = model.QuestionText,
                    CreatedAt = DateTime.Now,
                    AppUserId = userId,
                    CategoryId = model.CategoryId,


                };
                await _context.Questions.AddAsync(question);
                await _context.SaveChangesAsync();
                _notify.Success("Soru başarılı şekilde kaydedildi.");
                return RedirectToAction("MyQuestions", "Question");
            }
            return View();
        }
        [Authorize]

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();

            var userId =  _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login","User");

            var question = await _questionRepository.Questions.Where(x => x.QuestionId == id && x.AppUserId == userId).Select(x => new QuestionModel() { 
               QuestionId = x.QuestionId,
                CategoryId = x.CategoryId, 
                QuestionText= x.QuestionText ,
                QuestionTitle= x.QuestionTitle ,
               

            }).FirstOrDefaultAsync();

            if (question == null) return NotFound();

            
            return View(question);
        }
        [Authorize]

        [HttpPost]
        public async Task<IActionResult> Edit(QuestionModel model)
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
                    _notify.Success("Soru kaydedilirken hata oluştu. "+ex.Message);
                    return RedirectToAction("MyQuestions", "Question");

                }
                _notify.Success("Soru başarılı şekilde kaydedildi.");

                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);  // Roles'ları async şekilde alıyoruz
                    if (userRoles.Contains("Admin"))  // Admin rolüne sahip mi diye kontrol ediyoruz
                    {
                        return RedirectToAction("Questions", "Admin");
                    }
                }

                return RedirectToAction("MyQuestions", "Question");

            }
            return View();
        }

    }
}
