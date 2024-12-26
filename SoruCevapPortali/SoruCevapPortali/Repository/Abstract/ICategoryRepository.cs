using SoruCevapPortali.Models;

namespace SoruCevapPortali.Repository.Abstract
{
    public interface ICategoryRepository
    {
        IQueryable<Category> Categories { get; }
        void CreateCategory(Category category);
      
    }
}
