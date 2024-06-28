using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;



namespace Repository.Interfaces
{
    public interface IUserDetailRepository
    {
        public void AddUserDetail(TblUserDetail userDetail);

        public TblUserDetail GetUserDetailByEmail(string email);
        public TblUserDetail GetUserDetailByUser(string Username);

        public string GetUserActivityDetails(IEnumerable<Claim> claims);

    }
}
