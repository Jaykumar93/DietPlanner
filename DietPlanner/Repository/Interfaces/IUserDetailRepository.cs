using Domain.Entities;



namespace Repository.Interfaces
{
    public interface IUserDetailRepository
    {
        public void AddUserDetail(TblUserDetail userDetail);

        public TblUserDetail GetUserDetailByEmail(string email);

        public List<TblUserDetail> GetAllUserDetails();
    }
}
