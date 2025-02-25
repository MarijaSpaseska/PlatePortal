using PlatePortal.Domain.DomainModels;
using PlatePortal.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatePortal.Domain.DTO
{
    public class OrderDto
    {
        //public string UserId { get; set; }
        //public PlatePortalApplicationUser User { get; set; }
        //public virtual ICollection<ProductInOrder> ProductInOrders { get; set; }
        public double TotalPrice { get; set; }
        public ICollection<ProductInOrder> ProductInOrders { get; set; }
        public PlatePortalApplicationUser User { get; set; }


    }
}
