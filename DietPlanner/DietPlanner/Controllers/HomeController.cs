using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Services.AuthServices;
using System.Diagnostics;

namespace DietPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserDetailRepository _userDetailRepository;

        public HomeController(ILogger<HomeController> logger, UserDetailRepository userDetailRepository)
        {
            _logger = logger;
            
            _userDetailRepository = userDetailRepository;
        }



        [NoCache]
        [Authorize]
        public IActionResult Index()
        {
           
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
