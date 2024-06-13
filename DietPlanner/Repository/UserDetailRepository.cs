using Domain.Data;
using Domain.Entities;
using Repository.Interfaces;

namespace Repository
{
    public class UserDetailRepository: IUserDetailRepository
    {
        private readonly Domain.Data.DietContext _context;

        public UserDetailRepository(Domain.Data.DietContext context)
        {
            _context = context;
        }

        
        public void AddUserDetail(TblUserDetail userDetail)
        {
            _context.TblUserDetails.Add(userDetail);
            _context.SaveChanges();
        }

        public List<TblUserDetail> GetAllUserDetails() => throw new NotImplementedException();

        public TblUserDetail GetUserDetailByEmail(string email)
        {
            return _context.TblUserDetails.FirstOrDefault(TblUserDetail => TblUserDetail.Email == email);
        }
        public TblUserDetail GetUserDetailByUser(string Username)
        {
            return _context.TblUserDetails.FirstOrDefault(TblUserDetail => TblUserDetail.UserName == Username);
        }
    }

    public class Validation
    {
        private readonly IUserDetailRepository _userDetailRepository;

        public Validation(IUserDetailRepository userDetailRepository)
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
