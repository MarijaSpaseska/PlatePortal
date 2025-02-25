using System;
using System.Collections.Generic;
using System.Text;

namespace PlatePortal.Domain.DTO
{
    public class ProductDto
    {

        public string MenuItemName { get; set; }

        public string MenuItemType { get; set; }
        public string MenuItemDescription { get; set; }

        public double MenuItemPrice { get; set; }
        public string MenuItemImage { get; set; }


        public string RestaurantName { get; set; }

        public string RestaurantAddress { get; set; }

        public string RestaurantOpeningHours { get; set; }

        public string RestaurantDeliveryTime { get; set; }

        public string RestaurantCuisine { get; set; }

        public string RestaurantLogo { get; set; }
    }
}
