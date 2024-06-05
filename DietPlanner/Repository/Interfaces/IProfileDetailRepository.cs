using Domain.DTO;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IProfileDetailRepository
    {
        public ProfileDetailViewModel GetProfileDetails(TblUserDetail User, string jwtToken);
        
        public Task<bool> UpdateProfileDetails(ProfileDetailViewModel model);
       

    }
}
