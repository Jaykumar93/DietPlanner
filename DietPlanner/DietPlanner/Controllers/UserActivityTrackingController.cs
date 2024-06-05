using AspNetCoreHero.ToastNotification.Abstractions;
using Domain.Data;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Security.Claims;
using static Domain.DTO.ActivityTrackingViewModel;

namespace Web.Controllers
{
    [Authorize]
    public class UserActivityTrackingController : Controller
    {
        private readonly Domain.Data.DietContext _context;
        private readonly INotyfService _notyf;

        public UserActivityTrackingController(Domain.Data.DietContext context,INotyfService notyf) 
        {
            _context = context;
            _notyf = notyf;
        }

        [HttpGet]
        public IActionResult UserActivity()
        {
            var claims = HttpContext.User.Claims;
            string Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var profileId = (from user in _context.TblUserDetails
                             join profile in _context.TblProfileDetails on user.UserId equals profile.UserId
                             where user.Email == Email
                             select profile.ProfileId).FirstOrDefault();

            var allActivity = _context.TblActivityTrackings.Where(activity=> activity.ProfileId == profileId).ToList();

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

            var intensityData = activityViewModel
                    .GroupBy(a => a.ActivityIntensity)
                    .Select(g => new
                    {
                        Intensity = g.Key.ToString(),
                        TotalCalories = g.Sum(a => a.CalorieBurned)
                    }).OrderBy(data => data.TotalCalories).ToList();

            var activityData = activityViewModel
                     .GroupBy(a => a.ActivityType)
                     .Select(g => new
                     {
                         ActivityType = g.Key.ToString(),
                         TotalCalories = g.Sum(a => a.CalorieBurned)
                     }).ToList();

            ViewBag.IntensityData =JsonConvert.SerializeObject(intensityData);
            ViewBag.Data = JsonConvert.SerializeObject(chartData);
            ViewBag.ActivityData = JsonConvert.SerializeObject(activityData);

            var ActivityData = activityViewModel.OrderBy(data=>data.ActivityEndDatetime).Take(20).ToList();

            return View(ActivityData);
        }

        [HttpGet]
        public IActionResult ViewActivityDetails()
        {
            var claims = HttpContext.User.Claims;
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
                    ActivityId = activity.ActivityId,
                    Email = Email,
                    ActivityType = activity.ActivityType,
                    ActivityStartDatetime = activity.ActivityStartDatetime,
                    ActivityEndDatetime = activity.ActivityEndDatetime,
                    ActivityIntensity = (ActivityIntensityType)Enum.Parse(typeof(ActivityIntensityType), activity.ActivityIntensity),
                    CalorieBurned = activity.CalorieBurned,
                };
            });
            return View(activityViewModel);
        }

        [HttpGet]
        public IActionResult AddActivity()
        {
            var claims = HttpContext.User.Claims;
            string Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var viewModel = new ActivityTrackingViewModel
            {
                Email = Email
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddActivity(ActivityTrackingViewModel model)
        {
            try
            {
                var profileId = await (from user in _context.TblUserDetails
                                       join profile in _context.TblProfileDetails on user.UserId equals profile.UserId
                                       where user.Email == model.Email
                                       select profile.ProfileId).FirstOrDefaultAsync();

                if (profileId!= null)
                {
                    TblActivityTracking tblActivityTracking = new TblActivityTracking
                    {
                        ProfileId = profileId,
                        ActivityType = model.ActivityType,
                        ActivityStartDatetime = model.ActivityStartDatetime,
                        ActivityEndDatetime = model.ActivityEndDatetime,
                        ActivityIntensity = model.ActivityIntensity.ToString(),
                        CalorieBurned = model.CalorieBurned,
                    };
                    _context.TblActivityTrackings.Add(tblActivityTracking);
                    _context.SaveChangesAsync();
                    _notyf.Success("Activity Detail Added Successfully");
                }
                else
                {
                    _notyf.Warning("Activity Detail Added");
                }
            }
            catch (Exception ex)
            {
                _notyf.Error("An error occurred while Adding the Activity Detail.");
            }
            return RedirectToAction("ViewActivityDetails", "UserActivityTracking");
        }

        [HttpGet]
        public IActionResult UpdateActivity(Guid ActivityId)
        {
            TblActivityTracking activityDetail = _context.TblActivityTrackings.FirstOrDefault(activity => activity.ActivityId == ActivityId);
            var claims = HttpContext.User.Claims;
            string Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var activityViewModel = new ActivityTrackingViewModel
                {
                    Email = Email,
                    ActivityType = activityDetail.ActivityType,
                    ActivityStartDatetime = activityDetail.ActivityStartDatetime,
                    ActivityEndDatetime = activityDetail.ActivityEndDatetime,
                    ActivityIntensity = (ActivityIntensityType)Enum.Parse(typeof(ActivityIntensityType), activityDetail.ActivityIntensity),
                    CalorieBurned = activityDetail.CalorieBurned,
                };
            
            return View(activityViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateActivity(ActivityTrackingViewModel model)
        {

            
            try
            {
                var profileId = await (from user in _context.TblUserDetails
                                       join profile in _context.TblProfileDetails on user.UserId equals profile.UserId
                                       where user.Email == model.Email
                                       select profile.ProfileId).FirstOrDefaultAsync();


                if (profileId != null)
                {
                    TblActivityTracking tblActivityTracking = new TblActivityTracking
                    {
                        ProfileId = profileId,
                        ActivityType = model.ActivityType,
                        ActivityStartDatetime = model.ActivityStartDatetime,
                        ActivityEndDatetime = model.ActivityEndDatetime,
                        ActivityIntensity = model.ActivityIntensity.ToString(),
                        CalorieBurned = model.CalorieBurned,
                    };
                    _context.TblActivityTrackings.Update(tblActivityTracking);
                    _context.SaveChangesAsync();
                    _notyf.Success("Activty Detail Updated Successfully");
                }
                else
                {
                    _notyf.Warning("Activty Detail not Found");

                }
                
            }
            catch (Exception ex)
            {

                _notyf.Error("An error occurred while Updating the Activity Detail.");
                
            }
            return RedirectToAction("ViewActivityDetails", "UserActivityTracking");
        }

        public async Task<IActionResult> DeleteActivity(Guid ActivityId)
        {
           

            if (ActivityId != null)
            {
                var activityDetails = _context.TblActivityTrackings.Where(activity => activity.ActivityId == ActivityId).FirstOrDefault();
                _context.TblActivityTrackings.Remove(activityDetails);
                _context.SaveChanges();
                _notyf.Success("Activity Detail Deleted Successfully");
            }
            else
            {
                _notyf.Error("Error While Deleting Activity Detail");
            }
            return RedirectToAction("ViewActivityDetails", "UserActivityTracking");
        }
    }
}
