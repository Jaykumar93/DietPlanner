using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.AuthServices;
using Domain.DTO;
using Services;
using AspNetCoreHero.ToastNotification.Abstractions;
using Domain.Data;
using Repository;

namespace Web.Controllers
{
    [NoCache]
    [Authorize(Roles = "Admin")]
    public class AdminChallengesController : Controller
    {
        private readonly Domain.Data.DietContext _context;
        private readonly ChallengesRewardRepository _challengesRewardController;
        private readonly Upload _upload;
        private readonly INotyfService _notyf;

        public AdminChallengesController(Domain.Data.DietContext context,ChallengesRewardRepository challengesRewardController, Upload upload, INotyfService notyf)
        {
            _context = context;
            _challengesRewardController = challengesRewardController;
            _upload = upload;
            _notyf = notyf;
        }



        public IActionResult ViewChallenges()
        {
            return View(_challengesRewardController.GetAllChallenges());
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
                bool isAdded = await _challengesRewardController.AddChallengesReward(model);

                if (isAdded)
                {
                    _notyf.Success("Challenge Added Successfully");
                    return RedirectToAction("ViewChallenges");
                }
                else
                {
                    _notyf.Error("Challenge Not Added Successfully");
                    return RedirectToAction("ViewChallenges");
                }
                
            }
            catch (Exception ex)
            {
                
                _notyf.Warning("An error occurred while creating the challenge.");
                return RedirectToAction("ViewChallenges");
            }

        }


        public IActionResult UpdateChallenge(string challengeName)
        {
           
            return View(_challengesRewardController.GetChallengeReward(challengeName));
        }


        [NoCache]
        [HttpPost]
        public async Task<IActionResult> UpdateChallenge(ChallengesRewardViewModel UpdatedDetails)
        {
            try
            {
                bool ChallengeisUpdated = await _challengesRewardController.UpdateChallengeReward(UpdatedDetails);
                if (ChallengeisUpdated)
                { 
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
        public async Task<IActionResult> DeleteChallengeAsync(string challengeName)
        {
            if(await _challengesRewardController.DeleteChallengeReard(challengeName))
            {
                _notyf.Success("Challenge Deleted Successfully");
            }
            else
            {
                _notyf.Error("Error While Deleting Challenge");
            }
            return RedirectToAction("ViewChallenges");
        }
    }
}
