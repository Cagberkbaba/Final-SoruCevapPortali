using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SoruCevapPortali.Models;
using SoruCevapPortali.Repository.Abstract;
using SoruCevapPortali.ViewModels;

namespace SoruCevapPortali.Repository.Concrete
{
    public class EfQuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext _context;

        public EfQuestionRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Question> Questions => _context.Questions;

        public void CreateQuestion(Question question)
        {
            
            try
            {
                _context.Questions.Add(question);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Veri Tabanına Kayıt Eklenirken Hata Oluştu " + ex);
            }


        }
        public async Task EditQuestion(QuestionModel question )
        {
            var entity = await _context.Questions.FirstOrDefaultAsync(x =>x.QuestionId == question.QuestionId);
            if (entity != null)
            {
                entity.QuestionTitle = question.QuestionTitle;
                entity.QuestionText = question.QuestionText;
                entity.CreatedAt = DateTime.UtcNow;
                entity.CategoryId = question.CategoryId;
                await _context.SaveChangesAsync();
            }
            else
            {
                
            }
        }

    }
}
