using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlatePortal.Domain.DomainModels
{
    public class Product : BaseEntity
    {
        [Required]
        [Display(Name = "Menu Item Name")]
        public string MenuItemName { get; set; }
        [Required]
        [Display(Name = "Menu Item Type")]
        public string MenuItemType { get; set; }
        [Required]
        [Display(Name = "Menu Item Description")]
        public string MenuItemDescription { get; set; }
        [Required]
        [Display(Name = "Menu Item Price")]
        public double MenuItemPrice { get; set; }
        [Display(Name = "Menu Item Image")]
        public string MenuItemImage { get; set; }

        [Required]
        [Display(Name = "Restaurant Name")]
        public string RestaurantName { get; set; }
        [Display(Name = "Restaurant Address")]
        public string RestaurantAddress { get; set; }
        [Display(Name = "Restaurant Opening Hours")]
        public string RestaurantOpeningHours { get; set; }
        [Display(Name = "Restaurant Delivery Time ")]
        public string RestaurantDeliveryTime { get; set; }
        [Display(Name = "Restaurant Cuisine")]
        public string RestaurantCuisine { get; set; }
        [Display(Name = "Restaurant Logo")]
        public string RestaurantLogo { get; set; }

        public virtual ICollection<ProductInShoppingCart> ProductInShoppingCarts { get; set; }
        public virtual ICollection<ProductInOrder> ProductInOrders { get; set; }

    }
}
