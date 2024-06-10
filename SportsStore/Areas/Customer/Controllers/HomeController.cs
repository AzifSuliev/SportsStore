using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsStore.DataAccess.Repository.IRepository;
using SportsStore.Models;
using SportsStore.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace SportsStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == claim.Value).Count());
            }
            IEnumerable<Product> allProducts = _unitOfWork.Product.GetAll(includeProperties: "ProductImages").ToList();
            return View(allProducts);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category,ProductImages"),
                Count = 0,
                ProductId = productId
            };
            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            // Определение пользователя
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            // Определение корзины пользователя по его Id
            shoppingCart.AppUserId = userId;

            // извлечение корзины из базы данных (если она есть)
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.AppUserId == userId && u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                // Корзина существует 
                cartFromDb.Count += shoppingCart.Count; //  изменение количества товара
                _unitOfWork.ShoppingCart.Update(cartFromDb); // обновление корзины 
                _unitOfWork.Save();
            }
            else
            {
                // Корзина не существует
                // Добавление корзины
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId).Count());
            }
            TempData["success"] = "Корзина успешно обновлена";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult GetCategories()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            return View(categories);
        }

        public IActionResult ShowItemsOfCategoryForCustomer(int? id)
        {
            Category category = _unitOfWork.Category.Get(x => x.Id == id);
            List<Product> productlist = _unitOfWork.Product.GetAll(x => x.CategoryId == category.Id, includeProperties: "ProductImages,Category").ToList();
            return View(productlist);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
