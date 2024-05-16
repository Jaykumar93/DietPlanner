using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Domain.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Services.AuthServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace DietPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserDetailRepository _userDetailRepository;
        private readonly IDistributedCache _cache;
        private readonly DietContext _context;

        public HomeController(ILogger<HomeController> logger, UserDetailRepository userDetailRepository, IDistributedCache cache,DietContext context)
        {
            _logger = logger;
            _userDetailRepository = userDetailRepository;
            _cache = cache;
            _context = context;
        }


        [NoCache]
        [Authorize]
        public async Task<IActionResult> IndexAsync()
        {
            
            return View();
        }

        public async Task<IActionResult> User([FromServices] IDistributedCache cache)
        {
            var usersinfo = new List<TblUserDetail>();
            if (string.IsNullOrEmpty(cache.GetString("users")))
            {
                usersinfo = _context.TblUserDetails.ToList();
                
                    
                var userstring = JsonConvert.SerializeObject(usersinfo);
                cache.SetString("users", userstring);
            }
            else
            {
                var userFromCache = cache.GetString("users");
                usersinfo = JsonConvert.DeserializeObject<List<TblUserDetail>>(userFromCache);
            }
            return View(usersinfo);
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
