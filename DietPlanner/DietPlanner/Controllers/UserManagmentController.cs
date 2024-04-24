    using Domain.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.AuthServices;
using Services.ViewModels;


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
        [NoCache]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(LoginModel login)
        {
            try
            {
                var userInfo = _context.TblUserDetails.FirstOrDefault(user => user.Email == login.Email);

                if (userInfo == null)
                {
                    // User not found
                    TempData["UnSuccessful"] = "Invalid username or password.";
                    return RedirectToAction("SignIn", "UserManagment");
                }

                var encryptedPass = Authentication.Checking(login.Password, _config["PasswordKey"], userInfo.PasswordSalt);

                if (encryptedPass == userInfo.PasswordHash)
                {
                    /*var roleName = _context.TblRoles.FirstOrDefault(role => role.RoleId == .RoleId)?.RoleName;*/

                    var roleName = "user";
                    var token = Authorization.GetJWTToken(login, _config, roleName);

                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                    };
                    HttpContext.Response.Cookies.Append("JwtToken", token, cookieOptions);

                    var response = new
                    {
                        Token = token,
                        Message = "Login successful"
                    };

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
        public async Task<IActionResult> SignUp(RegistrationModel model, DateTime dateTime)
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
            TblRole role;
           
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

            TempData["Success"] = "Registered Successfully";
            return RedirectToAction("SignIn", "UserManagment");

        }
    
    
        public IActionResult Logout()
        {
            Response.Cookies.Delete("JwtToken");
            return RedirectToAction("SignIn", "UserManagment");
        }
    
    
    }
}
