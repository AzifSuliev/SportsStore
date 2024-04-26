using Microsoft.AspNetCore.Mvc;
using SportsStore.DataAccess.Repository.IRepository;
using SportsStore.Models;
using System.Diagnostics;

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
            IEnumerable<Product> allProducts = _unitOfWork.Product.GetAll(includeProperties: "ProductImages").ToList();
            return View(allProducts);
        }

        public IActionResult Details(int id)
        {
            Product product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
            return View(product);
        }

        public IActionResult GetCategories()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            return View(categories);
        }

        public IActionResult ShowItemsOfCategoryForCustomer(int? id)
        {
            Category category = _unitOfWork.Category.Get(x => x.Id == id);
            List<Product> productlist = _unitOfWork.Product.GetAll(x => x.CategoryId == category.Id, includeProperties: "ProductImages").ToList();
            return View(productlist);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
