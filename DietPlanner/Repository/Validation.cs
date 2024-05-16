using Domain.Entities;
using System;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AuthServices
{
    public class Validation
    {
        private readonly UserDetailRepository _userDetailRepository;

        public Validation(UserDetailRepository userDetailRepository) 
        {
            _userDetailRepository = userDetailRepository;
        }

        public bool IsUsernameUnique(string username)
        {
            TblUserDetail user = _userDetailRepository.GetUserDetailByUser(username);
            if (user == null)
                return true;
            return false;
        }
        public bool IsEmailUnique(string email)
        {
            TblUserDetail user = _userDetailRepository.GetUserDetailByEmail(email);
            if (user == null)
                return true;
            return false;
        }
    }
}
