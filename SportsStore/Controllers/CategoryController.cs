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

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if(obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Порядковый номер не должен совпадать с названием категории");
            }
            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj); // Добавить категорию
                _db.SaveChanges(); // Сохранить изменения
                return RedirectToAction(nameof(Index), "Category");
            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            if (id == null || id == 0) NotFound();
            Category? categoryFromDb = _db.Categories.Find(id);
            if (categoryFromDb == null) NotFound();
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index), "Category");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) NotFound();
            Category? categoryFromDb = _db.Categories.FirstOrDefault(u => u.Id.Equals(id));
            if (categoryFromDb == null) NotFound();
            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int id)
        {
            Category categoryFromDb = _db.Categories.FirstOrDefault(u => u.Id == id);
            if (categoryFromDb == null) NotFound();
            _db.Categories.Remove(categoryFromDb);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index), "Category");
        }
    }
}
