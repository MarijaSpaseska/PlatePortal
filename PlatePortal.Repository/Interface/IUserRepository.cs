using PlatePortal.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatePortal.Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<PlatePortalApplicationUser> GetAll();
        PlatePortalApplicationUser Get(string id);
        void Insert(PlatePortalApplicationUser entity);
        void Update(PlatePortalApplicationUser entity);
        void Delete(PlatePortalApplicationUser entity);
    }
}
