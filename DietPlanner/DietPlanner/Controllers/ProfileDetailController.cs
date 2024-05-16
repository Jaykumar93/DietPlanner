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

namespace Web.Controllers.User
{
    [Authorize]

    public class ProfileDetailController : Controller
    {
        private readonly Domain.Data.DietContext _context;
        private readonly Upload _upload;
        private readonly INotyfService _notyf;

        public ProfileDetailController(Domain.Data.DietContext context, Upload upload, INotyfService notyf)
        {
            _context = context;
            _upload = upload;
            _notyf = notyf;
        }


        [NoCache]
        [HttpGet]
        public async Task<IActionResult> ViewProfile(TblUserDetail User)
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];


            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);


            Claim emailClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);


            var userdetail = _context.TblUserDetails.FirstOrDefault(u => u.Email == emailClaim.Value);

            var userProfile = _context.TblProfileDetails.Where(profile => profile.UserId == userdetail.UserId).FirstOrDefault();


            ProfileDetailViewModel profileDtoView = new ProfileDetailViewModel
            {
                UserName = userdetail.UserName,
                RoleName = _context.TblRoles.Where(r => r.RoleId == userProfile.RoleId).Select(r => r.RoleName).FirstOrDefault(),
                UserGender = userProfile.UserGender ?? "Not Specified",
                UserWeight = userProfile.UserWeight ?? "Not Specified",
                UserHeight = userProfile.UserHeight ?? "Not Specified",
                UserGoals = userProfile.UserGoals ?? "Not Specified",
                UserCalorieLimit = userProfile.UserCalorieLimit ?? "Not Specified",
                UserSpeciality = userProfile.UserSpeciality ?? "Not Specified",
            };

            return View("ViewProfile", profileDtoView);
        }

        [NoCache]
        public async Task<IActionResult> UpdateProfile(TblUserDetail User)
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];


            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);


            Claim emailClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);


            var userdetail = _context.TblUserDetails.FirstOrDefault(u => u.Email == emailClaim.Value);

            var userProfile = _context.TblProfileDetails.Where(profile => profile.UserId == userdetail.UserId).FirstOrDefault();


            ProfileDetailViewModel profileDtoView = new ProfileDetailViewModel
            {
                UserName = userdetail.UserName,
                RoleName = _context.TblRoles.Where(r => r.RoleId == userProfile.RoleId).Select(r => r.RoleName).FirstOrDefault(),
                UserGender = userProfile.UserGender ?? "Not Specified",
                UserWeight = userProfile.UserWeight ?? "Not Specified",
                UserHeight = userProfile.UserHeight ?? "Not Specified",
                UserGoals = userProfile.UserGoals ?? "Not Specified",
                UserCalorieLimit = userProfile.UserCalorieLimit ?? "Not Specified",
                UserSpeciality = userProfile.UserSpeciality ?? "Not Specified",

            };

            return View("UpdateProfile", profileDtoView);
        }

        [NoCache]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileDetailViewModel model)
        {

            try
            {
                var userdetail = await _context.TblUserDetails.FirstOrDefaultAsync(u => u.UserName == model.UserName);
                TblProfileDetail profileDetail = await _context.TblProfileDetails.FirstOrDefaultAsync(p => p.UserId == userdetail.UserId);
                string imagePath = null;
                if (model.ImagePath != null)
                {
                    imagePath = await _upload.UploadProfileImg(model.ImagePath);
                }
                else
                {
                     imagePath = profileDetail.ImagePath;
                }
                string certificatePath = await _upload.UploadCertificate(model.UserCertification);

                if (profileDetail != null)
                {
                    profileDetail.UserGender = model.UserGender;
                    profileDetail.UserWeight = model.UserWeight;
                    profileDetail.UserHeight = model.UserHeight;
                    profileDetail.UserCalorieLimit = model.UserCalorieLimit;
                    profileDetail.UserGoals = model.UserGoals;
                    profileDetail.UserSpeciality = model.UserSpeciality;
                    profileDetail.UserCertification = certificatePath;
                    profileDetail.ImagePath = imagePath;

                    _context.TblProfileDetails.Update(profileDetail);
                    await _context.SaveChangesAsync();
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
