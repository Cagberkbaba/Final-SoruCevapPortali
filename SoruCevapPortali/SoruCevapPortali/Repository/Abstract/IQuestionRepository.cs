using SoruCevapPortali.Models;
using SoruCevapPortali.ViewModels;

namespace SoruCevapPortali.Repository.Abstract
{
    public interface IQuestionRepository
    {
        IQueryable<Question> Questions { get; }
        void CreateQuestion(Question question);
        Task EditQuestion(QuestionModel question);
    }
}
