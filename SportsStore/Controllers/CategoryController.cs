using Microsoft.AspNetCore.Mvc;
using SportsStore.DataAccess.Data;
using SportsStore.Models;
using System.IO;

namespace SportsStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CategoryController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
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
        public IActionResult Create(Category obj, IFormFile file)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Порядковый номер не должен совпадать с названием категории");
            }
            if (file != null && file.Length > 0)
            {

                if (ModelState.IsValid)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    if (file != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string categoryPath = Path.Combine(wwwRootPath, @"images");
                        using (var fileStream = new FileStream(Path.Combine(categoryPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        obj.ImageURL = @"\images\" + fileName;
                    }
                    _db.Categories.Add(obj); // Добавить категорию
                    _db.SaveChanges(); // Сохранить изменения
                    TempData["success"] = "Категория была успешно добавлена!";
                    return RedirectToAction(nameof(Index), "Category");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(obj.Name) && !string.IsNullOrEmpty(obj.DisplayOrder.ToString()) && obj.DisplayOrder != 0)
                {
                    obj.ImageURL = "/images/empty.png";
                    _db.Categories.Add(obj); 
                    _db.SaveChanges();
                    TempData["success"] = "Категория была успешно добавлена!";
                    return RedirectToAction(nameof(Index), "Category");
                }
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
        public IActionResult Edit(Category obj, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                if (ModelState.IsValid)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    if (file != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string categoryPath = Path.Combine(wwwRootPath, @"images");
                        string filePath = Path.Combine(categoryPath, fileName);
                        if (!string.IsNullOrEmpty(obj.ImageURL))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath, obj.ImageURL.TrimStart('\\'));
                            FileInfo fileInfo = new FileInfo(oldImagePath);
                            if (fileInfo.Exists) fileInfo.Delete();
                        }
                        using (var fileStream = new FileStream(Path.Combine(categoryPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        obj.ImageURL = @"\images\" + fileName;
                    }
                    _db.Categories.Update(obj);
                    _db.SaveChanges();
                    TempData["success"] = "Категория была успешно изменена!";
                    return RedirectToAction(nameof(Index), "Category");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(obj.Name) && !string.IsNullOrEmpty(obj.DisplayOrder.ToString()) && obj.DisplayOrder != 0)
                {
                    _db.Categories.Update(obj);
                    _db.SaveChanges();
                    TempData["success"] = "Категория была успешно изменена!";
                    return RedirectToAction(nameof(Index), "Category");
                }
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
            if (!string.IsNullOrEmpty(categoryFromDb.ImageURL))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, categoryFromDb.ImageURL.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
            }
            _db.Categories.Remove(categoryFromDb);
            _db.SaveChanges();
            TempData["success"] = "Категория была успешно удалена!";
            return RedirectToAction(nameof(Index), "Category");
        }
    }
}
