using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsStore.DataAccess.Data;
using SportsStore.DataAccess.Repository.IRepository;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System.Security.Claims;


namespace SportsStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            // Определение пользователя
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product.ProductImages")
            };

            foreach (ShoppingCart cart in ShoppingCartVM.ShoppingCartList)
            {
                ShoppingCartVM.OrderTotal += GetTotalPrice(cart);
            }

            return View(ShoppingCartVM);
        }

        public decimal GetTotalPrice(ShoppingCart shoppingCart)
        {
            decimal totalPrice;
            totalPrice = shoppingCart.Product.Price * shoppingCart.Count;
            return totalPrice;
        }

        public IActionResult AddUnit(int cartId)
        {
            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            shoppingCart.Count++;
            _unitOfWork.ShoppingCart.Update(shoppingCart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult MinusUnit(int cartId)
        {
            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (shoppingCart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(shoppingCart);
            }
            else
            {
                shoppingCart.Count--;
                _unitOfWork.ShoppingCart.Update(shoppingCart);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteItem(int cartId)
        {
            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(shoppingCart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
