using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoruCevapPortali.Repository.Abstract;

namespace SoruCevapPortali.ViewComponents
{
    public class CategoriesMenuViewComponent : ViewComponent
    {
        private ICategoryRepository _categoryRepository;
        public CategoriesMenuViewComponent(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _categoryRepository.Categories.ToListAsync());
        }
    
    }
}
