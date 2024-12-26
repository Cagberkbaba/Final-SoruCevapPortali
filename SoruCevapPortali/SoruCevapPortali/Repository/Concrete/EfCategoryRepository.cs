using Azure;
using SoruCevapPortali.Models;
using SoruCevapPortali.Repository.Abstract;

namespace SoruCevapPortali.Repository.Concrete
{
    public class EfCategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public EfCategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Category> Categories => _context.Categories;

        public void CreateCategory(Category category)
        {
            try
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Veri Tabanına Kayıt Eklenirken Hata Oluştu " + ex);
            }


        }


    }
}
