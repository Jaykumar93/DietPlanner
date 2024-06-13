using Domain.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.AuthServices;
using Domain.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Repository;
using Microsoft.AspNetCore.Components.Web;
using Repository.Interfaces;

namespace Web.Controllers.User
{
    [NoCache]
    [Authorize]

    public class ProfileDetailController : Controller
    {
        private readonly Domain.Data.DietContext _context;
        private readonly IProfileDetailRepository _profileDetailRepository;
        private readonly Upload _upload;
        private readonly INotyfService _notyf;

        public ProfileDetailController(Domain.Data.DietContext context, IProfileDetailRepository profileDetailRepository, Upload upload, INotyfService notyf)
        {
            _context = context;
            _profileDetailRepository = profileDetailRepository;
            _upload = upload;
            _notyf = notyf;
        }


        [HttpGet]
        public async Task<IActionResult> ViewProfile(TblUserDetail User)
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];

            ProfileDetailViewModel UserProfileDetails = _profileDetailRepository.GetProfileDetails(User, jwtToken);

            return View("ViewProfile", UserProfileDetails);
        }


        [HttpGet]
        public async Task<IActionResult> UpdateProfile(TblUserDetail User)
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];

            ProfileDetailViewModel UserProfileDetails = _profileDetailRepository.GetProfileDetails(User, jwtToken);

            return View("UpdateProfile", UserProfileDetails);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileDetailViewModel model)
        {

            try
            {
                bool isProfileUpdated = await _profileDetailRepository.UpdateProfileDetails(model);
                if (isProfileUpdated)
                {
                    _notyf.Success("Profile Updated Successfully");
                }
                else
                {
                    _notyf.Warning("Profile not found.");
                }

                return RedirectToAction("ViewProfile", "ProfileDetail");
            }
            catch (Exception)
            {
                _notyf.Error("An unexpected error occurred while updating the Profile. Please contact support.");
                return RedirectToAction("ViewProfile", "ProfileDetail");
            }


        }
    }
}
