using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IRoleRepository
    {
        public void AddRole(TblRole RoleDetail);

        public TblRole GetRoleByRoleId(Guid RoleId);

        public TblRole GetRoleIdByRole(string rolename);
        public List<TblRole> GetAllRole();
    }
}
