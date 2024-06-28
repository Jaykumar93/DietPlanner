using AspNetCoreHero.ToastNotification.Abstractions;
using Domain.Data;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using Repository;
using Repository.Interfaces;

namespace Web.Controllers
{
    public class UserChallengesController : Controller
    {
        private readonly Domain.Data.DietContext _context;
        private readonly INotyfService _notyf;
        private readonly IChallengeRewardRepository _challengeRewardRepository;

        public UserChallengesController(Domain.Data.DietContext context, INotyfService notyf, IChallengeRewardRepository challengeRewardRepository) 
        {
            _context = context;
            _notyf = notyf;
            _challengeRewardRepository = challengeRewardRepository;
        } 
        public IActionResult ChallengeDashboard(List<ChallengesRewardViewModel.ChallengeStatus> selectedCategories,DateTime? startDate, DateTime? endDate, string term = "", string orderBy = "")
        {
            term = term.ToLower();
            var ChallengesList = _challengeRewardRepository.GetAllChallenges();

            
            var claims = HttpContext.User.Claims;
            string Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var profileId = (from user in _context.TblUserDetails
                             join profile in _context.TblProfileDetails on user.UserId equals profile.UserId
                             where user.Email == Email
                             select profile.ProfileId).FirstOrDefault();
            var challengesLog = _context.TblChallengesRewardsLogs.Where(log=> log.ProfileId == profileId).ToList();

            
            /* if (selectedCategories?.Count > 0)
             {
                 ChallengesList = ChallengesList.Where(p => selectedCategories.Contains(p.ChallengeStatus));
             }*/

            if (startDate.HasValue)
            {
                ChallengesList = ChallengesList.Where(challenges => challenges.StartDatetime >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                ChallengesList = ChallengesList.Where(challenges => challenges.StartDatetime <= endDate.Value);
            }
            switch (orderBy)
            {
                case "start_date_desc":
                    ChallengesList = ChallengesList.OrderByDescending(challenges => challenges.StartDatetime);
                    break;

                case "end_date_desc":
                    ChallengesList = ChallengesList.OrderByDescending(challenges => challenges.EndDatetime);
                    break;
                case "end_date":
                    ChallengesList = ChallengesList.OrderBy(challenges => challenges.EndDatetime);
                    break;
                case "start_date":
                    ChallengesList = ChallengesList.OrderBy(challenges => challenges.StartDatetime);
                    break;
                default:
                    ChallengesList = ChallengesList.OrderBy(challenges => challenges.StartDatetime);
                    break;
            }

            if (!string.IsNullOrEmpty(term))
            {
                ChallengesList = ChallengesList.Where(c => c.ChallengeName.ToLower().Contains(term) ||
                                                c.ChallengeDescription.ToLower().Contains(term) ||
                                                /*c.ChallengeGoals.ToString().Contains(term) ||*/
                                                c.RewardDescription.ToLower().Contains(term));
            }
            int registeredChallengesCount = challengesLog.Count(log =>
                 log.Status == ChallengesRewardViewModel.UserChallengeStatus.Registered.ToString() ||
                 log.Status == ChallengesRewardViewModel.UserChallengeStatus.Completed.ToString() ||
                 log.Status == ChallengesRewardViewModel.UserChallengeStatus.OnGoing.ToString());


            int completedChallengesCount = challengesLog.Count(log => log.Status == ChallengesRewardViewModel.UserChallengeStatus.Completed.ToString());
            int rewardChallengesCount = challengesLog.Count(log =>log.Status == ChallengesRewardViewModel.UserChallengeStatus.Registered.ToString() && log.Status == ChallengesRewardViewModel.UserChallengeStatus.Completed.ToString() && log.RewardId != null);

            int totalChallengeCount = ChallengesList.Count();
            ViewData["TotalChallengeCount"] = totalChallengeCount;

            ViewData["RegisteredChallengesCount"] = registeredChallengesCount;
            ViewData["CompletedChallengesCount"] = completedChallengesCount;
            ViewData["RewardChallengesCount"] = rewardChallengesCount;

            return View(ChallengesList);
        }

        public IActionResult ChallengeInfoDetails(string challengeName)
        {
            var challengeDetails = _context.TblChallenges.Where(challenge => challenge.ChallengeName == challengeName).FirstOrDefault();
            var RewardsDetails = _context.TblRewards.Where(reward => reward.ChallengeId == challengeDetails.ChallengeId).FirstOrDefault();
            var claims = HttpContext.User.Claims;
            string Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var profileId = (from user in _context.TblUserDetails
                             join profile in _context.TblProfileDetails on user.UserId equals profile.UserId
                             where user.Email == Email
                             select profile.ProfileId).FirstOrDefault();
            var challengeRewardLog = _context.TblChallengesRewardsLogs
           .Where(challenge => challenge.ChallengeId == challengeDetails.ChallengeId && challenge.ProfileId == profileId)
           .FirstOrDefault();

       

            ChallengesRewardViewModel challengeModel = new ChallengesRewardViewModel
            {
                ChallengeId = challengeDetails.ChallengeId,
                ChallengeName = challengeName,
                ChallengeDescription = challengeDetails.ChallengeDescription,
                ChallengeGoals = challengeDetails.ChallengeGoals,
                StartDatetime = challengeDetails.StartDatetime,
                EndDatetime = challengeDetails.EndDatetime,
                ChallengeImgLocation = challengeDetails.ChallengeImagePath,
                RewardDescription = RewardsDetails.RewardDescription,
                RewardImgLocation = RewardsDetails.RewardImagePath,
                UserStatus = challengeRewardLog != null
                            ? (ChallengesRewardViewModel.UserChallengeStatus)Enum.Parse(typeof(ChallengesRewardViewModel.UserChallengeStatus), challengeRewardLog.Status)
                            : ChallengesRewardViewModel.UserChallengeStatus.NotRegistered,
                UserChallengeProgress = challengeRewardLog != null? challengeRewardLog.ChallengeProgress: 0
        };
            return PartialView("ChallengeInfoDetails", challengeModel);
        }

        public async Task<IActionResult> AddChallengeToUser(Guid ChallengeId, string UserName)
        {


            var user = await _context.TblUserDetails.Where(user=>user.UserName == UserName).FirstOrDefaultAsync();
            var profile = await _context.TblProfileDetails.Where(profile => profile.UserId == user.UserId).FirstOrDefaultAsync();
            var challenge = await _context.TblChallenges.Where(challenge => challenge.ChallengeId == ChallengeId).FirstOrDefaultAsync();

            bool isProfileInChallengeLog = await _context.TblChallengesRewardsLogs.AnyAsync(log => log.ProfileId == profile.ProfileId && log.ChallengeId == challenge.ChallengeId);

            if (isProfileInChallengeLog == false)
            {
                TblChallengesRewardsLog ChallengeRewardLog = new TblChallengesRewardsLog
                {
                    ProfileId = profile.ProfileId,
                    ChallengeId = ChallengeId,
                    Status = ChallengesRewardViewModel.UserChallengeStatus.Registered.ToString(),
                    StatusDatetime = DateTime.Now,
                    RewardId = null,
                };
                await _context.TblChallengesRewardsLogs.AddAsync(ChallengeRewardLog);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"{challenge.ChallengeName} is Added to Your Profile {UserName}" });
            }
            else
            {
                return Json(new { success = true, message = $"{challenge.ChallengeName} is Already Added to Your Profile {UserName}" });

            }
        }
    }
}
