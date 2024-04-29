using Domain.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.ViewModels;

namespace Web.Controllers
{
    public class ProfileDetailController : Controller
    {
		private readonly DietContext _context;

		public ProfileDetailController(DietContext context) 
        {
			_context = context;
		}

        public IActionResult AddProfile()
        {
            return View();
        }

        public IActionResult AddProfile(ProfileDetailViewModel profileModel)
        {
            return View();
        }

        public IActionResult ViewProfile(TblUserDetail User)
        {
            var userProfile = _context.TblProfileDetails.Where(profile=> profile.UserId == User.UserId).FirstOrDefault();
            return View(userProfile);
		}

        public IActionResult UpdateProfile(TblUserDetail User)
        {
            var userProfile = _context.TblProfileDetails.Where(profile => profile.UserId == User.UserId).FirstOrDefault();
            return View(userProfile);
        }
    }
}
