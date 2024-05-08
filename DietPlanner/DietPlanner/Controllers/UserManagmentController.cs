using Domain.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.AuthServices;
using Services.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;


namespace Web.Controllers
{
    public class UserManagmentController : Controller
    {
        private readonly Domain.Data.DietContext _context;
        private readonly IConfiguration _config;
        private readonly Validation _validation;

        public UserManagmentController(Domain.Data.DietContext context,IConfiguration config,Validation validation)
        {
            _context = context;
            _config = config;
            _validation = validation;
        }


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
                    TempData["UnSuccessful"] = "Invalid username or password.";
                    return RedirectToAction("SignIn", "UserManagment");
                }

                var encryptedPass = Authentication.Checking(login.Password, _config["PasswordKey"], userInfo.PasswordSalt);

                if (encryptedPass == userInfo.PasswordHash)
                {
                  

                    var roleName = _context.TblRoles.Where(role=>role.RoleId == profileInfo.RoleId).Select(role=>role.RoleName).FirstOrDefault();
                    var token = Authorization.GetJWTToken(login, _config, roleName);

                    
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                    };
                    HttpContext.Response.Cookies.Append("JwtToken", token, cookieOptions);

                    TempData["Successful"] = "You have successfully logged in";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Incorrect password
                    TempData["UnSuccessful"] = "Invalid username or password.";
                    return RedirectToAction("SignIn", "UserManagment");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


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
                ModelState.AddModelError("UserName", "Username is already present.");
            }
            if (!_validation.IsEmailUnique(model.Email))
            {   
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
	

            TempData["Success"] = "Registered Successfully"; 
            return RedirectToAction("SignIn", "UserManagment");

        }

        public async Task<IActionResult> Logout()
        {
            //HttpContext.Response.Cookies.Delete("JWTToken");
            Response.Cookies.Delete("JwtToken");
            return RedirectToAction("SignIn", "UserManagment");
        }


        public async Task<IActionResult> ForgotPassword()
        {
            
            return View();
        
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(LoginModel model)
        {
            
            return View();
        
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
