using PlatePortal.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatePortal.Services.Interface
{
    public interface IShoppingCartService
    {
        ShoppingCartDto getShoppingCartInfo(string userId);
        bool deleteProductFromSoppingCart(string userId, Guid id);
        bool order(string userId);
    }
}
