using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Domain.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Services.AuthServices;

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
        [NoCache]

        public IActionResult Privacy()
        {
            return View();
        }
        [NoCache]

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
