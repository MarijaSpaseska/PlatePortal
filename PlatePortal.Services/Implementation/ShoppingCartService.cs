using PlatePortal.Domain.DomainModels;
using PlatePortal.Domain.DTO;
using PlatePortal.Repository.Interface;
using PlatePortal.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlatePortal.Services.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<EmailMessage> _mailRepository;
        private readonly IRepository<ProductInOrder> _productInOrderRepository;
        private readonly IUserRepository _userRepository;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, IRepository<Order> orderRepository,
            IRepository<EmailMessage> mailRepository, IRepository<ProductInOrder> productInOrderRepository, IUserRepository userRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _orderRepository = orderRepository;
            _mailRepository = mailRepository;
            _productInOrderRepository = productInOrderRepository;
            _userRepository = userRepository;
        }

        public ShoppingCartDto getShoppingCartInfo(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);
                var userShoppingCart = loggedInUser.UserCart;
                var productPrice = userShoppingCart.ProductInShoppingCarts.Select(z => new
                {
                    ProductPrice = z.Product.MenuItemPrice,
                    Quantity = z.Quantity
                }).ToList();
                double totalPrice = 0;
                foreach (var item in productPrice)
                {
                    totalPrice += item.ProductPrice * item.Quantity;
                }
                ShoppingCartDto model = new ShoppingCartDto
                {
                    ProductInShoppingCarts = userShoppingCart.ProductInShoppingCarts.ToList(),
                    TotalPrice = totalPrice
                };
                // var allProducts = userShoppingCart.ProductInShoppingCarts.Select(z => z.Product).ToList();
                return model;
            }
            return new ShoppingCartDto();
        }

        public bool deleteProductFromSoppingCart(string userId, Guid id)
        {
            if (!string.IsNullOrEmpty(userId) && id != null)
            {
                var loggedInUser = this._userRepository.Get(userId);
                var userShoppingCart = loggedInUser.UserCart;
                var itemToDelete = userShoppingCart.ProductInShoppingCarts.Where(z => z.ProductId.Equals(id)).FirstOrDefault();
                userShoppingCart.ProductInShoppingCarts.Remove(itemToDelete);
                this._shoppingCartRepository.Update(userShoppingCart);
                return true;
            }
            return false;
        }


        public bool order(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);
                var userShoppingCart = loggedInUser.UserCart;

                EmailMessage mail = new EmailMessage();
                mail.MailTo = loggedInUser.Email;
                mail.Subject = "Successfully created order!";
                mail.Status = false;

                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    User = loggedInUser,

                };
                this._orderRepository.Insert(order);

                List<ProductInOrder> productInOrder = new List<ProductInOrder>();
                var result = userShoppingCart.ProductInShoppingCarts
                    .Select(z => new ProductInOrder
                    {
                        Id = Guid.NewGuid(),
                        ProductId = z.Product.Id,
                        SelectedProduct = z.Product,
                        OrderId = order.Id,
                        UserOrder = order,
                        Quantity = z.Quantity
                    }).ToList();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Your order is completed. The order conatins: ");
                var totalPrice = 0.0;
                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];
                    totalPrice += item.Quantity * item.SelectedProduct.MenuItemPrice;
                    sb.AppendLine(i.ToString() + ". " + item.SelectedProduct.MenuItemName + " with quantity of: " + item.Quantity + " and price of: " + item.SelectedProduct.MenuItemPrice+"den.");
                }
                sb.AppendLine("Total price for your order: " + totalPrice.ToString()+"den.");
                mail.Content = sb.ToString();

                productInOrder.AddRange(result);
                foreach (var item in productInOrder)
                {
                    this._productInOrderRepository.Insert(item);
                }

                loggedInUser.UserCart.ProductInShoppingCarts.Clear();

                this._mailRepository.Insert(mail);
                this._userRepository.Update(loggedInUser);

                //List<EmailMessage> testList = new List<EmailMessage>();
                //testList.Add(mail);
                //await _emailService.SendEmailAsync(testList);

                return true;
            }
            return false;
        }
    }

}
