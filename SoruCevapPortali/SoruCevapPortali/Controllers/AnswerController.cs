using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoruCevapPortali.Models;
using SoruCevapPortali.Repository.Abstract;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace SoruCevapPortali.Controllers
{
    public class AnswerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly INotyfService _notify;

        public AnswerController(ICategoryRepository categoryRepository, INotyfService notify, IQuestionRepository questionRepository, AppDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _categoryRepository = categoryRepository;
            _questionRepository = questionRepository;
            _context = context;
            _notify = notify;

        }
        [HttpPost]
        public async Task<JsonResult> AddAnswer(int QuestionId, string AnswerText)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var photoUrl = User.FindFirstValue(ClaimTypes.UserData) ?? "NoUser.jpg";
            if (userId == null || user == null) return Json(new { success = false, message = "Giriş yapmalısınız." }); ;
            var entity = new Answer
            {
                AnswerText = AnswerText,
                CreatedAt = DateTime.Now,
                QuestionId = QuestionId,
                AppUserId = userId,

            };
            var createdAt = entity.CreatedAt;
            try
            {
                await _context.Answers.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Cevap eklenmedi." + ex.Message }); ;

            }

            return Json(new
            {
                userName,
                QuestionId,
                createdAt,
                photoUrl,
                AnswerText,

            });
        }
        [Authorize]
        public async Task<IActionResult> MyAnswers()
        {
            var userId = _userManager.GetUserId(User);

            var answers = await _context.Answers.Where(x => x.AppUserId == userId).Include(y => y.Question).ThenInclude(c => c.AppUser).ToListAsync();
            return View(answers);
        }
        [Authorize]

        public async Task<IActionResult> Delete(int id)
        {
            var answer = await _context.Answers.FirstOrDefaultAsync(x => x.AnswerId == id);
            if (answer == null) return NotFound();

            try
            {
                _context.Answers.Remove(answer);
                await _context.SaveChangesAsync();
                _notify.Success("Cevap başarılı şekilde silindi.");
               
            }
            catch (Exception)
            {

                _notify.Error("Cevabı silerken bir hata oluştu.");

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


            return RedirectToAction("Index", "Question");
        }
    }
}
