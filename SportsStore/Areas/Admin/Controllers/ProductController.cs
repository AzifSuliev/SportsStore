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
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> productsFromDb = _unitOfWork.Product.GetAll().ToList();
            IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
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
        public IActionResult Create(ProductVM productVm)
        {

            if (ModelState.IsValid)
            {
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
            productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
            if (productVM == null) NotFound();
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Edit(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();
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
            Product productFromDb = _unitOfWork.Product.Get(u => u.Id == 0);
            if (productFromDb == null) NotFound();
            return View(productFromDb);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            if (productFromDb == null) NotFound();
            _unitOfWork.Product.Remove(productFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Товар был успешно удалён!";
            return RedirectToAction(nameof(Index), "Product");
        }
    }
}
