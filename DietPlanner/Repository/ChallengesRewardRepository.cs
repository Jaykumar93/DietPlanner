using Domain.Data;
using Domain.DTO;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services;
using System.Security.Claims;


namespace Repository
{
    public class ChallengesRewardRepository : IChallengeRewardRepository
    {
        private readonly DietContext _context;
        private readonly Upload _upload;

        public ChallengesRewardRepository(DietContext context, Upload upload)
        {
            _context = context;
            _upload = upload;
        }

        public IEnumerable<ChallengesRewardViewModel> GetAllChallenges()
        {
            var allChallenges = _context.TblChallenges.ToList();

            var ChallengesList = allChallenges.Select(challenge =>
            {
                return new ChallengesRewardViewModel
                {
                    ChallengeId = challenge.ChallengeId,    
                    ChallengeName = challenge.ChallengeName,
                    ChallengeDescription = challenge.ChallengeDescription,
                    ChallengeGoals = challenge.ChallengeGoals,
                    StartDatetime = challenge.StartDatetime,
                    EndDatetime = challenge.EndDatetime,
                    ChallengeImgLocation = challenge.ChallengeImagePath,
                    RewardImgLocation = _context.TblRewards.Where(reward => reward.ChallengeId == challenge.ChallengeId).Select(reward => reward.RewardImagePath).FirstOrDefault(),
                    RewardDescription = _context.TblRewards.Where(reward => reward.ChallengeId == challenge.ChallengeId).Select(reward => reward.RewardDescription).FirstOrDefault(),

                    AdminStatus = (ChallengesRewardViewModel.ChallengeStatus)Enum.Parse(typeof(ChallengesRewardViewModel.ChallengeStatus), challenge.ChallengeStatus)


                };
            }).ToList();
            return ChallengesList;
        }

        public async Task<bool> AddChallengesReward(ChallengesRewardViewModel model)
        {
            DateTime currentDate = DateTime.Today;

            string challengeimagePath = await _upload.UploadChallengeImage(model.ChallengeImagePath);
            string rewardimagePath = await _upload.UploadRewardImage(model.RewardImagePath);

            TblChallenge challenge = new TblChallenge
            {
                ChallengeName = model.ChallengeName,
                ChallengeDescription = model.ChallengeDescription,
                ChallengeGoals = model.ChallengeGoals,
                StartDatetime = model.StartDatetime,
                EndDatetime = model.EndDatetime,
                ChallengeImagePath = challengeimagePath,

            };

            await _context.TblChallenges.AddAsync(challenge);
            await _context.SaveChangesAsync();


            TblReward reward = new TblReward
            {
                ChallengeId = challenge.ChallengeId,
                RewardDescription = model.RewardDescription,
                RewardImagePath = rewardimagePath
            };

            await _context.TblRewards.AddAsync(reward);
            await _context.SaveChangesAsync();

            return true;
        }


        public IEnumerable<ChallengesRewardViewModel> GetChallengesBasedOnStatus(ChallengesRewardViewModel.ChallengeStatus Status1, ChallengesRewardViewModel.ChallengeStatus Status2)
        {
            var allChallenges = _context.TblChallenges
                                .Where(challenge =>
                                    challenge.ChallengeStatus == Status1.ToString() ||
                                    challenge.ChallengeStatus == Status2.ToString())
                                .ToList();


            var ChallengesList = allChallenges.Select(challenge =>
            {

                return new ChallengesRewardViewModel
                {
                    ChallengeName = challenge.ChallengeName,
                    ChallengeDescription = challenge.ChallengeDescription,
                    ChallengeGoals = challenge.ChallengeGoals,
                    StartDatetime = challenge.StartDatetime,
                    EndDatetime = challenge.EndDatetime,

                    ChallengeImgLocation = challenge.ChallengeImagePath,
                    RewardImgLocation = _context.TblRewards.Where(reward => reward.ChallengeId == challenge.ChallengeId).Select(reward => reward.RewardImagePath).FirstOrDefault(),
                    RewardDescription = _context.TblRewards.Where(reward => reward.ChallengeId == challenge.ChallengeId).Select(reward => reward.RewardDescription).FirstOrDefault()

                };
            }).ToList();
            return ChallengesList;
        }

        public ChallengesRewardViewModel GetChallengeReward(string challengeName)
        {
            var challengedetail = _context.TblChallenges.Where(challenge => challenge.ChallengeName == challengeName).FirstOrDefault();
            var rewarddetail = _context.TblRewards.Where(reward => reward.ChallengeId == challengedetail.ChallengeId).FirstOrDefault();


            ChallengesRewardViewModel ChallengeRewardDetail = new ChallengesRewardViewModel
            {
                ChallengeName = challengedetail.ChallengeName,
                ChallengeDescription = challengedetail.ChallengeDescription,
                ChallengeGoals = challengedetail.ChallengeGoals,
                StartDatetime = challengedetail.StartDatetime,
                EndDatetime = challengedetail.EndDatetime,
                /*ChallengeStatus = (ChallengesRewardViewModel.StatusType)Enum.Parse(typeof(ChallengesRewardViewModel.StatusType), challengedetail.ChallengeStatus),*/
                RewardDescription = rewarddetail.RewardDescription,
            };
            return ChallengeRewardDetail;
        }

        public IEnumerable<ChallengesRewardViewModel> GetOngoingChallenges(IEnumerable<Claim> claims)
        {
            string email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var profileId = (from user in _context.TblUserDetails
                             join profile in _context.TblProfileDetails on user.UserId equals profile.UserId
                             where user.Email == email
                             select profile.ProfileId).FirstOrDefault();

            var challenges = GetAllChallenges().ToList();
            var challengeLogs = _context.TblChallengesRewardsLogs
                                   .Where(log => log.ProfileId == profileId && (log.Status == ChallengesRewardViewModel.UserChallengeStatus.OnGoing.ToString()
                                   || log.Status == ChallengesRewardViewModel.UserChallengeStatus.Registered.ToString()))
                                   .ToList();

            var ongoingChallenges = from log in challengeLogs
                                    join challenge in challenges
                                    on log.ChallengeId equals challenge.ChallengeId
                                    select new ChallengesRewardViewModel
                                    {
                                        ChallengeId = challenge.ChallengeId,
                                        ChallengeName = challenge.ChallengeName,
                                        UserChallengeProgress = log.ChallengeProgress
                                    };
            return ongoingChallenges;
        }
        public async Task<bool> UpdateChallengeReward(ChallengesRewardViewModel UpdatedDetails)
        {
            DateTime currentDate = DateTime.Today;

            var challengedetails = await _context.TblChallenges.FirstOrDefaultAsync(challenge => challenge.ChallengeName == UpdatedDetails.ChallengeName);
            var rewarddetails = await _context.TblRewards.FirstOrDefaultAsync(reward => reward.ChallengeId == challengedetails.ChallengeId);

            string challengeimagePath = null;
            string rewardimagePath = null;

            if (UpdatedDetails.ChallengeImagePath != null)
            {
                challengeimagePath = await _upload.UploadCertificate(UpdatedDetails.ChallengeImagePath);

            }
            else
            {
                challengeimagePath = challengedetails.ChallengeImagePath;
            }
            if (UpdatedDetails.RewardImagePath != null)
            {
                rewardimagePath = await _upload.UploadRewardImage(UpdatedDetails.RewardImagePath);
            }
            else
            {
                rewardimagePath = rewarddetails.RewardImagePath;
            }

            if (challengedetails != null && rewarddetails != null)
            {
                challengedetails.ChallengeName = UpdatedDetails.ChallengeName;
                challengedetails.ChallengeDescription = UpdatedDetails.ChallengeDescription;
                challengedetails.ChallengeGoals = UpdatedDetails.ChallengeGoals;
                challengedetails.StartDatetime = UpdatedDetails.StartDatetime;
                challengedetails.EndDatetime = UpdatedDetails.EndDatetime;
                /*if (UpdatedDetails.ChallengeStatus != null)
                {
                    challengedetails.ChallengeStatus = UpdatedDetails.ChallengeStatus.ToString();
                }*/
                /*else
                {
                    challengedetails.ChallengeStatus = challengedetails.ChallengeStatus;
                }*/
                rewarddetails.RewardDescription = UpdatedDetails.RewardDescription;
                challengedetails.ChallengeImagePath = challengeimagePath;
                rewarddetails.RewardImagePath = rewardimagePath;


                _context.TblChallenges.Update(challengedetails);
                _context.TblRewards.Update(rewarddetails);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }

        }


        public async Task<bool> DeleteChallengeReard(string challengeName)
        {
            var challengeDetail = await _context.TblChallenges.Where(challenge => challenge.ChallengeName == challengeName).FirstOrDefaultAsync();
            var rewardDetail =await  _context.TblRewards.Where(reward => reward.ChallengeId == challengeDetail.ChallengeId).FirstOrDefaultAsync();
            _context.TblRewards.Remove(rewardDetail);
            _context.TblChallenges.Remove(challengeDetail);
            _context.SaveChangesAsync();
            return true;
        }

    }
}
