using Domain.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services.AuthServices;
using Domain.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using AspNetCoreHero.ToastNotification.Abstractions;


namespace Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly Domain.Data.DietContext _context;
        private readonly IConfiguration _config;
        private readonly Validation _validation;
        private readonly INotyfService _notyf;

        public AuthController(Domain.Data.DietContext context, IConfiguration config, Validation validation, INotyfService notyf)
        {
            _context = context;
            _config = config;
            _validation = validation;
            _notyf = notyf;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(LoginModel login)
        {
            try
            {
                var userInfo = _context.TblUserDetails.FirstOrDefault(user => user.Email == login.Email);
                var profileInfo = _context.TblProfileDetails.FirstOrDefault(profile => profile.UserId == userInfo.UserId);
                if (userInfo == null)
                {
                    // User not found
                    _notyf.Warning("Invalid username or password.");
                    return RedirectToAction("SignIn", "Auth");
                }

                var encryptedPass = Authentication.Checking(login.Password, _config["PasswordKey"], userInfo.PasswordSalt);

                if (encryptedPass == userInfo.PasswordHash)
                {
                    var roleName = _context.TblRoles.Where(role => role.RoleId == profileInfo.RoleId).Select(role => role.RoleName).FirstOrDefault();
                    var token = Authorization.GetJWTToken(login, _config, roleName);


                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                    };
                    HttpContext.Response.Cookies.Append("JwtToken", token, cookieOptions);

                    _notyf.Success("You have successfully logged In");

                    return RedirectToAction("DashboardRedirection", "RoleBasedRedirection");
                }
                else
                {
                    // Incorrect password
                    _notyf.Warning("Invalid username or password.");
                    return RedirectToAction("SignIn", "Auth");
                }
            }
            catch (Exception ex)
            {
                var x = StatusCode(500, $"An error occurred: {ex.Message}");
                _notyf.Error($"{x}");
                return RedirectToAction("SignIn", "Auth");

            }
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(RegistrationModel model)
        {
            if (!_validation.IsUsernameUnique(model.UserName))
            {
                _notyf.Warning("UserName : Username is already present.");
                ModelState.AddModelError("UserName", "Username is already present.");
            }
            if (!_validation.IsEmailUnique(model.Email))
            {
                _notyf.Warning("Email : Email is already present.");

                ModelState.AddModelError("Email", "Email is already present.");
            }
            if (!ModelState.IsValid)
            {
                return View("SignUp", model);
            }
            DateTime currentDate = DateTime.Today;

            string PassKey = _config["PasswordKey"];
            string PasswordSalt = null;
            var PasswordHash = Authentication.Encrypt(model.Password, PassKey, out PasswordSalt);
            TblUserDetail user = new TblUserDetail
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                ContactNumber = model.ContactNumber,
                PasswordHash = PasswordHash,
                PasswordSalt = PasswordSalt,
                CreatedBy = model.UserName,
                CreatedDate = DateOnly.Parse(currentDate.ToString("yyyy-MM-dd")),
                ModifiedBy = model.UserName,
                ModifiedDate = DateOnly.Parse(currentDate.ToString("yyyy-MM-dd"))
            };
            await _context.TblUserDetails.AddAsync(user);
            await _context.SaveChangesAsync();

            TblProfileDetail profile = new TblProfileDetail
            {
                UserId = _context.TblUserDetails.Where(user => user.UserName == model.UserName).Select(user => user.UserId).FirstOrDefault(),
                RoleId = _context.TblRoles.Where(role => role.RoleName == "User").Select(role => role.RoleId).FirstOrDefault(),
                UserGender = "Not Specified"

            };

            await _context.TblProfileDetails.AddAsync(profile);
            await _context.SaveChangesAsync();

            _notyf.Success("Registered Successfully");
            return RedirectToAction("SignIn", "Auth");

        }

        public async Task<IActionResult> Logout()
        {
            //HttpContext.Response.Cookies.Delete("JWTToken");
            Response.Cookies.Delete("JwtToken");
            return RedirectToAction("SignIn", "Auth");
        }


       

        public async Task<IActionResult> LayoutData()
        {
            {
                string jwtToken = HttpContext.Request.Cookies["JWTToken"];


                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);


                string email = parsedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;

                TblUserDetail userdetail = await _context.TblUserDetails.FirstOrDefaultAsync(user => user.Email == email);

                string imagePath = await _context.TblProfileDetails.Where(profile=>profile.UserId == userdetail.UserId).Select(profile=>profile.ImagePath).FirstOrDefaultAsync();

                var loggedUser = new { Username = userdetail.UserName, ImagePath = imagePath };

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,

                };
                var json = JsonSerializer.Serialize(loggedUser, options);
                return Ok(json);
            }
        }


        
    }
}
