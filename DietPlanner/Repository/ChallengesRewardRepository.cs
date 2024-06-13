using Domain.Data;
using Domain.DTO;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repository.Interfaces;
using Services;
using static Domain.DTO.ChallengesRewardViewModel;


namespace Repository
{
    public class ChallengesRewardRepository : IChallengeRewardRepository
    {
        private readonly Domain.Data.DietContext _context;
        private readonly Upload _upload;

        public ChallengesRewardRepository(Domain.Data.DietContext context, Upload upload)
        {
            _context = context;
            _upload = upload;
        }

        public IEnumerable<ChallengesRewardViewModel> GetAllChallenges()
        {
            var allChallenges = _context.TblChallenges.ToList();

            var ChallengesList = allChallenges.Select(challenge =>
            {
                Dictionary<string, string> ChallengeGoals = JsonConvert.DeserializeObject<Dictionary<string, string>>(challenge.ChallengeGoals);

                return new ChallengesRewardViewModel
                {
                    ChallengeName = challenge.ChallengeName,
                    ChallengeDescription = challenge.ChallengeDescription,
                    CalorieCountGoal = int.Parse(ChallengeGoals.GetValueOrDefault("CalorieCountGoal", "0")),
                    VitaminGoals = ChallengeGoals.GetValueOrDefault("VitaminGoals", "None"),
                    MineralsGoals = ChallengeGoals.GetValueOrDefault("MineralsGoals", "None"),
                    ProteinGoals = int.Parse(ChallengeGoals.GetValueOrDefault("ProteinGoals", "0")),
                    FatGoals = int.Parse(ChallengeGoals.GetValueOrDefault("MealFat", "0")),
                    CarbohydratesGoals = int.Parse(ChallengeGoals.GetValueOrDefault("CarbohydratesGoals", "0")),
                    WaterGoals = int.Parse(ChallengeGoals.GetValueOrDefault("WaterGoals", "0")),
                    StartDatetime = challenge.StartDatetime,
                    EndDatetime = challenge.EndDatetime,
                    StatusOfChallenge = (ChallengesRewardViewModel.ChallengeStatus)Enum.Parse(typeof(ChallengesRewardViewModel.ChallengeStatus), challenge.ChallengeStatus),
                    RewardDescription = _context.TblRewards.Where(reward => reward.ChallengeId == challenge.ChallengeId).Select(reward => reward.RewardDescription).FirstOrDefault()
                };
            });
            return ChallengesList;
        }

        public async Task<bool> AddChallengesReward(ChallengesRewardViewModel model)
        {
            DateTime currentDate = DateTime.Today;

            string challengeimagePath = await _upload.UploadChallengeImage(model.ChallengeImagePath);
            string rewardimagePath = await _upload.UploadRewardImage(model. RewardImagePath);

            var ChallengeGoals = new
            {
                model.CalorieCountGoal,
                model.VitaminGoals,
                model.MineralsGoals,
                model.ProteinGoals,
                model.FatGoals,
                model.CarbohydratesGoals,
                model.WaterGoals

            };
            string SerilizedInfo = JsonConvert.SerializeObject(ChallengeGoals);


            TblChallenge challenge = new TblChallenge
            {
                ChallengeName = model.ChallengeName,
                ChallengeDescription = model.ChallengeDescription,
                ChallengeGoals = SerilizedInfo,
                StartDatetime = model.StartDatetime,
                EndDatetime = model.EndDatetime,
                ChallengeStatus = ChallengesRewardViewModel.ChallengeStatus.NotStarted.ToString(),
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
                Dictionary<string, string> ChallengeGoals = JsonConvert.DeserializeObject<Dictionary<string, string>>(challenge.ChallengeGoals);

                return new ChallengesRewardViewModel
                {
                    ChallengeName = challenge.ChallengeName,
                    ChallengeDescription = challenge.ChallengeDescription,
                    CalorieCountGoal = int.Parse(ChallengeGoals.GetValueOrDefault("CalorieCountGoal", "0")),
                    VitaminGoals = ChallengeGoals.GetValueOrDefault("VitaminGoals", "None"),
                    MineralsGoals = ChallengeGoals.GetValueOrDefault("MineralsGoals", "None"),
                    ProteinGoals = int.Parse(ChallengeGoals.GetValueOrDefault("ProteinGoals", "0")),
                    FatGoals = int.Parse(ChallengeGoals.GetValueOrDefault("MealFat", "0")),
                    CarbohydratesGoals = int.Parse(ChallengeGoals.GetValueOrDefault("CarbohydratesGoals", "0")),
                    WaterGoals = int.Parse(ChallengeGoals.GetValueOrDefault("WaterGoals", "0")),
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

            Dictionary<string, string> ChallengeGoals = JsonConvert.DeserializeObject<Dictionary<string, string>>(challengedetail.ChallengeGoals);

            ChallengesRewardViewModel ChallengeRewardDetail = new ChallengesRewardViewModel
            {
                ChallengeName = challengedetail.ChallengeName,
                ChallengeDescription = challengedetail.ChallengeDescription,
                CalorieCountGoal = int.Parse(ChallengeGoals.GetValueOrDefault("CalorieCountGoal", "0")),
                VitaminGoals = ChallengeGoals.GetValueOrDefault("VitaminGoals", "None"),
                MineralsGoals = ChallengeGoals.GetValueOrDefault("MineralsGoals", "None"),
                ProteinGoals = int.Parse(ChallengeGoals.GetValueOrDefault("ProteinGoals", "0")),
                FatGoals = int.Parse(ChallengeGoals.GetValueOrDefault("MealFat", "0")),
                CarbohydratesGoals = int.Parse(ChallengeGoals.GetValueOrDefault("CarbohydratesGoals", "0")),
                WaterGoals = int.Parse(ChallengeGoals.GetValueOrDefault("WaterGoals", "0")),
                StartDatetime = challengedetail.StartDatetime,
                EndDatetime = challengedetail.EndDatetime,
                /*ChallengeStatus = (ChallengesRewardViewModel.StatusType)Enum.Parse(typeof(ChallengesRewardViewModel.StatusType), challengedetail.ChallengeStatus),*/
                RewardDescription = rewarddetail.RewardDescription,
            };
            return ChallengeRewardDetail;
        }

        public async Task<bool> UpdateChallengeReward(ChallengesRewardViewModel UpdatedDetails)
        {
            DateTime currentDate = DateTime.Today;

            var challengedetails = await _context.TblChallenges.FirstOrDefaultAsync(challenge => challenge.ChallengeName == UpdatedDetails.ChallengeName);
            var rewarddetails = await _context.TblRewards.FirstOrDefaultAsync(reward => reward.ChallengeId == challengedetails.ChallengeId);

            string challengeimagePath= null;
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

            var ChallengeGoals = new
            {
                UpdatedDetails.CalorieCountGoal,
                UpdatedDetails.VitaminGoals,
                UpdatedDetails.MineralsGoals,
                UpdatedDetails.ProteinGoals,
                UpdatedDetails.FatGoals,
                UpdatedDetails.CarbohydratesGoals,
                UpdatedDetails.WaterGoals

            };
            string SerilizedInfo = JsonConvert.SerializeObject(ChallengeGoals);

            if (challengedetails != null && rewarddetails != null)
            {
                challengedetails.ChallengeName = UpdatedDetails.ChallengeName;
                challengedetails.ChallengeDescription = UpdatedDetails.ChallengeDescription;
                challengedetails.ChallengeGoals = SerilizedInfo;
                challengedetails.StartDatetime = UpdatedDetails.StartDatetime;
                challengedetails.EndDatetime = UpdatedDetails.EndDatetime;
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
            var challengeDetail = _context.TblChallenges.Where(challenge => challenge.ChallengeName == challengeName).FirstOrDefault();
            var rewardDetail = _context.TblRewards.Where(reward => reward.ChallengeId == challengeDetail.ChallengeId).FirstOrDefault();
            _context.TblRewards.Remove(rewardDetail);
            _context.TblChallenges.Remove(challengeDetail);
            _context.SaveChanges();
            return true;
        }

    }
}
