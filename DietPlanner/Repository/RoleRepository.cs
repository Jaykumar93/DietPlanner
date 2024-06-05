using Domain.Data;
using Domain.Entities;
using Repository.Interfaces;

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
