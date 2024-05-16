using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.AuthServices;
using Domain.DTO;
using Services;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminChallengesController : Controller
    {
        private readonly Domain.Data.DietContext _context;
        private readonly Upload _upload;
        private readonly INotyfService _notyf;

        public AdminChallengesController(Domain.Data.DietContext context, Upload upload, INotyfService notyf)
        {
            _context = context;
            _upload = upload;
            _notyf = notyf;
        }



        public IActionResult ViewChallenges()
        {
            var allChallenges = _context.TblChallenges.ToList();

            var ChallengesList = allChallenges.Select(challenge =>
            {
                return new ChallengesRewardViewModel
                {
                    ChallengeName = challenge.ChallengeName,
                    ChallengeDescription = challenge.ChallengeDescription,
                    ChallengeGoals = challenge.ChallengeGoals,
                    StartDatetime = challenge.StartDatetime,
                    EndDatetime = challenge.EndDatetime,
                    ChallengeStatus = challenge.ChallengeStatus,
                    RewardDescription = _context.TblRewards.Where(reward => reward.ChallengeId == challenge.ChallengeId).Select(reward => reward.RewardDescription).FirstOrDefault()
                };
            }).ToList();

            return View(ChallengesList);
        }



        public async Task<IActionResult> CreateChallenge()
        {
            return View();
        }


        [NoCache]
        [HttpPost]
        public async Task<IActionResult> CreateChallenge(ChallengesRewardViewModel model)
        {
            try
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
                    ChallengeStatus = "NotRegistered",
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

                _notyf.Success("Challenge Added Successfully");
                return RedirectToAction("ViewChallenges");
            }
            catch (Exception ex)
            {
                
                _notyf.Warning("An error occurred while creating the challenge.");
                return RedirectToAction("ViewChallenges");
            }

        }


        public IActionResult UpdateChallenge(string challengeName)
        {
            var challengedetail = _context.TblChallenges.Where(challenge => challenge.ChallengeName == challengeName).FirstOrDefault();
            var rewarddetail = _context.TblRewards.Where(reward => reward.ChallengeId == challengedetail.ChallengeId).FirstOrDefault();


            ChallengesRewardViewModel updateChallenge = new ChallengesRewardViewModel
            {
                ChallengeName = challengedetail.ChallengeName,
                ChallengeDescription = challengedetail.ChallengeDescription,
                ChallengeGoals = challengedetail.ChallengeGoals,
                StartDatetime = challengedetail.StartDatetime,
                EndDatetime = challengedetail.EndDatetime,
                ChallengeStatus = challengedetail.ChallengeStatus,
                RewardDescription = rewarddetail.RewardDescription,
            };
            return View(updateChallenge);
        }


        [NoCache]
        [HttpPost]
        public async Task<IActionResult> UpdateChallenge(ChallengesRewardViewModel UpdatedDetails)
        {
            try
            {
                DateTime currentDate = DateTime.Today;



                var challengedetails = await _context.TblChallenges.FirstOrDefaultAsync(challenge => challenge.ChallengeName == UpdatedDetails.ChallengeName);
                var rewarddetails = await _context.TblRewards.FirstOrDefaultAsync(reward => reward.ChallengeId == challengedetails.ChallengeId);

                string challengeimagePath = null;
                string rewardimagePath= null;

                if (UpdatedDetails.ChallengeImagePath != null)
                {
                     challengeimagePath = await _upload.UploadCertificate(UpdatedDetails.ChallengeImagePath);
                }
                else if (UpdatedDetails.RewardImagePath != null)
                {
                     rewardimagePath = await _upload.UploadRewardImage(UpdatedDetails.RewardImagePath);
                }
                else
                {
                    challengeimagePath = challengedetails.ChallengeImagePath;
                    rewardimagePath = rewarddetails.RewardImagePath;
                }

                if (challengedetails != null && rewarddetails != null)
                {
                    challengedetails.ChallengeName = UpdatedDetails.ChallengeName;
                    challengedetails.ChallengeDescription = UpdatedDetails.ChallengeDescription;
                    challengedetails.ChallengeGoals = UpdatedDetails.ChallengeGoals;
                    challengedetails.StartDatetime = UpdatedDetails.StartDatetime;
                    challengedetails.EndDatetime = UpdatedDetails.EndDatetime;
                    challengedetails.ChallengeStatus = UpdatedDetails.ChallengeStatus;
                    rewarddetails.RewardDescription = UpdatedDetails.RewardDescription;
                    challengedetails.ChallengeImagePath = challengeimagePath;
                    rewarddetails.RewardImagePath = rewardimagePath;


                    _context.TblChallenges.Update(challengedetails);
                    _context.TblRewards.Update(rewarddetails);

                    await _context.SaveChangesAsync();
                    _notyf.Success("Challenge Updated Successfully");
                }

                else
                {
                    _notyf.Warning("challege not found.");
                }

                return RedirectToAction("ViewChallenges", "AdminChallenges");
            }
            catch (Exception ex)
            {


                _notyf.Error("An unexpected error occurred while updating the Challenge. Please contact support.");

                return RedirectToAction("ViewChallenges", "AdminChallenges");
            }
        }

        [NoCache]
        [HttpPost]
        public IActionResult DeleteChallenge(string challengeName)
        {
            var challengeDetail = _context.TblChallenges.Where(challenge => challenge.ChallengeName == challengeName).FirstOrDefault();
            var rewardDetail = _context.TblRewards.Where(reward => reward.ChallengeId == challengeDetail.ChallengeId).FirstOrDefault();
            _context.TblRewards.Remove(rewardDetail);
            _context.TblChallenges.Remove(challengeDetail);
            _context.SaveChanges();
            return RedirectToAction("ViewChallenges");
        }
    }
}
