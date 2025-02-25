using PlatePortal.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlatePortal.Domain.DomainModels
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public PlatePortalApplicationUser User { get; set; }
        public virtual ICollection<ProductInOrder> ProductInOrders { get; set; }
    }
}
