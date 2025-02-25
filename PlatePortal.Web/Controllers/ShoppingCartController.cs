using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatePortal.Domain.DomainModels;
using PlatePortal.Domain.Identity;
using PlatePortal.Services.Interface;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlatePortal.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(this._shoppingCartService.getShoppingCartInfo(userId));
        }

        //DeleteProductFromShoppingCart
        public IActionResult DeleteProductFromShoppingCart(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            this._shoppingCartService.deleteProductFromSoppingCart(userId, id);
            return RedirectToAction("Index", "ShoppingCart");
        }

        //OrderNow
        public void OrderNow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            this._shoppingCartService.order(userId);
            //return RedirectToAction("Index", "ShoppingCart");
        }

        public IActionResult PayOrder(string stripeEmail, string stripeToken)
        {
            var customerService = new CustomerService();
            var chargeService = new ChargeService();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = this._shoppingCartService.getShoppingCartInfo(userId);

            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var charge = chargeService.Create(new ChargeCreateOptions
            {
                Amount = (Convert.ToInt32(order.TotalPrice) * 100),
                Description = "Plate Portal Payment",
                Currency = "mkd",
                Customer = customer.Id
            });

            if (charge.Status == "succeeded")
            {
                this.OrderNow();

                return RedirectToAction("Index", "ShoppingCart");

            }

            return RedirectToAction("Index", "ShoppingCart");
        }

    }
}

