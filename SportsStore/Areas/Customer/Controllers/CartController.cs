using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsStore.DataAccess.Data;
using SportsStore.DataAccess.Repository.IRepository;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using SportsStore.Utility;
using SQLitePCL;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;


namespace SportsStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        private readonly IUnitOfWork _unitOfWork;
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
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId, includeProperties: "Product.ProductImages"),
                OrderHeader = new()
            };

            foreach (ShoppingCart cart in ShoppingCartVM.ShoppingCartList)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += GetTotalPrice(cart);
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
            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);
            if (shoppingCart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(shoppingCart);
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.
                    GetAll(u => u.AppUserId == shoppingCart.AppUserId).Count() - 1);
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
            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);
            HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.
                   GetAll(u => u.AppUserId == shoppingCart.AppUserId).Count() - 1);
            _unitOfWork.ShoppingCart.Remove(shoppingCart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary()
        {
            // Определение пользователя
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId, includeProperties: "Product,AppUser"),
                OrderHeader = new()
            };
            ShoppingCartVM.OrderHeader.AppUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            foreach (ShoppingCart shoppingCart in ShoppingCartVM.ShoppingCartList)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += GetTotalPrice(shoppingCart);
            }

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.AppUser.Name;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.AppUser.City;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.AppUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.AppUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.AppUser.StreetAddres;
            return View(ShoppingCartVM);
        }


        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            // Определение пользователя
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId, includeProperties: "Product.ProductImages");
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.AppUserId = userId;

            foreach (ShoppingCart shoppingCart in ShoppingCartVM.ShoppingCartList)
            {
                ShoppingCartVM.OrderHeader.OrderTotal = GetTotalPrice(shoppingCart);
            }

            ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusPending; // статус заказа

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (ShoppingCart shoppingCart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = shoppingCart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Count = shoppingCart.Count,
                    Price = shoppingCart.Product.Price * shoppingCart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            #region Stripe funcionality
            StripeConfiguration.ApiKey = "sk_test_51OoQctFmqGqHHR4uCCp7XjDux9y0Qm55nzaI4q91OEW4yJuPha6jsL2HLZY0uy4GLyYkXzCFuVdvigYKzNb6l90Y00882tKj9e";
            var domain = "https://localhost:7204/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?Id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + "customer/cart/index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                PaymentMethodTypes = new List<string>
                    {
                        "card"
                    }
            };

            foreach (var item in ShoppingCartVM.ShoppingCartList)
            {
                decimal exchangeRate = 92.05M;
                SessionLineItemOptions sessionLineItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)((item.Count * item.Product.Price * 100) / exchangeRate),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = item.Product.Name
                        }
                    },
                    Quantity = 1
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
            #endregion
            //return RedirectToAction(nameof(OrderConfirmation), new { Id = ShoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "AppUser");
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
                _unitOfWork.Save();
            }
            HttpContext.Session.Clear();
            // Удаление содержимого корзины
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == orderHeader.AppUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
        }
    }
}
