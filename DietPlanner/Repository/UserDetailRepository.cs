using Domain.Data;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Repository.Interfaces;
using static Domain.DTO.ActivityTrackingViewModel;
using System.Security.Claims;
using Newtonsoft.Json;

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

        public string GetUserActivityDetails(IEnumerable<Claim> claims)
        {
            string Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var profileId = (from user in _context.TblUserDetails
                             join profile in _context.TblProfileDetails on user.UserId equals profile.UserId
                             where user.Email == Email
                             select profile.ProfileId).FirstOrDefault();

            var allActivity = _context.TblActivityTrackings.Where(activity => activity.ProfileId == profileId).ToList();
            var activityViewModel = allActivity.Select(activity =>
            {
                return new ActivityTrackingViewModel
                {
                    Email = Email,
                    ActivityType = activity.ActivityType,
                    ActivityStartDatetime = activity.ActivityStartDatetime,
                    ActivityEndDatetime = activity.ActivityEndDatetime,
                    ActivityIntensity = (ActivityIntensityType)Enum.Parse(typeof(ActivityIntensityType), activity.ActivityIntensity),
                    CalorieBurned = activity.CalorieBurned,
                };
            });
            var chartData = activityViewModel
                    .Select(activity => new
                    {
                        x = activity.ActivityEndDatetime.ToString("o"), // ISO 8601 format
                        y = activity.CalorieBurned
                    }).OrderBy(data => DateTime.Parse(data.x)).ToList();
            var chartDatastring = JsonConvert.SerializeObject(chartData);
            return chartDatastring;
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
