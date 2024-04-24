using Domain.Data;
using Domain.Entities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly Domain.Data.DietContext _context;

        public RoleRepository(Domain.Data.DietContext context)
        {
            _context = context;
        }

        public void AddRole(TblRole RoleDetail)
        {
            _context.TblRoles.Add(RoleDetail);
            _context.SaveChanges();
        }

        public List<TblRole> GetAllRole()
        {
            return _context.TblRoles.ToList();
        }

        public TblRole GetRoleByRoleId(Guid RoleId)
        {
            return (TblRole)_context.TblRoles.Where(TblRole => TblRole.RoleId == RoleId);
        }

        public TblRole GetRoleIdByRole(string rolename)
        {
            return (TblRole)_context.TblRoles.Where(TblRole => TblRole.RoleName == rolename);
        }

    }
}
