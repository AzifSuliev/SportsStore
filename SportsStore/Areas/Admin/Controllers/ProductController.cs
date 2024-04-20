using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using SportsStore.DataAccess.Repository.IRepository;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> productsFromDb = _unitOfWork.Product.GetAll(includeProperties: "ProductImages").ToList();
            return View(productsFromDb);
        }

        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Create(ProductVM productVm, List<IFormFile>? files)
        {

            if (ModelState.IsValid)
            {
                // WebRootPath предназначен для предоставления физического пути к папке wwwroot
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null && files.Count > 0)
                {
                    foreach (IFormFile file in files)
                    {
                        // Генерируется уникальное имя файла с использованием Guid.NewGuid()
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\";
                        string finalPath = Path.Combine(wwwRootPath, productPath);
                        // создание папки по указанному пути finalPath, если такой папки не существyет
                        if (!Directory.Exists(finalPath)) Directory.CreateDirectory(finalPath);
                        // в этой части кода файл загружается на сервер и сохраняется в указанной директории,
                        // а затем путь к этому файлу присваивается свойству ImageURL объекта Product      
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        ProductImage productImage = new ProductImage
                        {
                            ImageURL = @"\" + productPath + @"\" + fileName,
                            ProductId = productVm.Product.Id
                        };
                        if (productVm.Product.ProductImages == null) productVm.Product.ProductImages = new List<ProductImage>();
                        productVm.Product.ProductImages.Add(productImage);
                        _unitOfWork.ProductImage.Add(productImage);
                    }
                }
                _unitOfWork.Product.Add(productVm.Product);
                _unitOfWork.Save();
                TempData["success"] = "Товар был успешно добавлен!";
                return RedirectToAction(nameof(Index), "Product");
            }
            else
            {
                productVm.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVm);
            }
        }

        public IActionResult Edit(int? id)
        {
            ProductVM productVM = new ProductVM
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            if (id == null || id == 0) NotFound();
            productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
            if (productVM == null) NotFound();
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Edit(ProductVM productVM, List<IFormFile>? files)
        {
            if (ModelState.IsValid)
            {
                // WebRootPath предназначен для предоставления физического пути к папке wwwroot
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (files != null && files.Count > 0)
                {
                    foreach (IFormFile file in files)
                    {
                        // Генерируется уникальное имя файла с использованием Guid.NewGuid()
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\";
                        string finalPath = Path.Combine(webRootPath, productPath);
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        ProductImage productImage = new()
                        {
                            ImageURL = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id
                        };
                        if (productVM.Product.ProductImages == null) productVM.Product.ProductImages = new List<ProductImage>();
                        productVM.Product.ProductImages.Add(productImage);
                        _unitOfWork.ProductImage.Add(productImage);
                    }
                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();
                }
                TempData["success"] = "Товар был успешно изменён!";
                return RedirectToAction(nameof(Index), "Product");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) NotFound();
            Product productFromDb = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
            ProductVM productVM = new ProductVM
            {
                Product = productFromDb
            };
            if (productVM == null) NotFound();
            return View(productVM);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product productFromDb = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
            if (productFromDb == null) NotFound();
            if (productFromDb.ProductImages != null && productFromDb.ProductImages.Count > 0)
            {
                foreach (var image in productFromDb.ProductImages)
                {
                    string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageURL.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
                }
            }
            _unitOfWork.Product.Remove(productFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Товар был успешно удалён!";
            return RedirectToAction(nameof(Index), "Product");
        }
    }
}
