
using Domain.Data;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Repository.Interfaces;
using Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Repository
{
    public class ProfileDetailRepository : IProfileDetailRepository
    {
        private readonly Domain.Data.DietContext _context;
        private readonly Upload _upload;

        public ProfileDetailRepository(Domain.Data.DietContext context, Upload upload)
        {
            _context = context;
            _upload = upload;
        }

        public ProfileDetailViewModel GetProfileDetails(TblUserDetail User, string jwtToken)
        {

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
            return profileDtoView;
        }

        public async Task<bool> UpdateProfileDetails(ProfileDetailViewModel model)
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
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
