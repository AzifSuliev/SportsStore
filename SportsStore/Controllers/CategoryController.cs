using Microsoft.AspNetCore.Mvc;
using SportsStore.DataAccess.Data;
using SportsStore.Models;

namespace SportsStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> categoriesFromDb = _db.Categories.ToList();
            return View(categoriesFromDb);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
