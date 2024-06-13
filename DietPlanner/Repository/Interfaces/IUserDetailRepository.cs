using Domain.Entities;
using Microsoft.EntityFrameworkCore;



namespace Repository.Interfaces
{
    public interface IUserDetailRepository
    {
        public void AddUserDetail(TblUserDetail userDetail);

        public TblUserDetail GetUserDetailByEmail(string email);
        public TblUserDetail GetUserDetailByUser(string Username);
  

    }
}
