using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyWebApp1.Models.EntityManager;
using MyWebApp1.Models.ViewModel;
using MyWebApp1.Security;
using System.Security.Claims;

namespace MyWebApp1.Controllers
{
 
    public class AccountController : Controller
    {
        public ActionResult SignUp()
        {
            return View();
        }
        //public SignInManager<string> _signInManager;
        public ActionResult Login()
        {
 
            return View();
        }

        public ActionResult MyProfile()
                {
                    UserManager useMan = new UserManager();
                    string inCurrent = User.Identity.Name;

                    UsersModel user = useMan.GetCurrUse(inCurrent); 

                    if (user == null)
                    {
                        return NotFound(); 
                    }

                    return View(user);
                }
 
        [AuthorizeRoles("Admin")]
        public ActionResult Users()
        {
 
            UserManager um = new UserManager();
            UsersModel user = um.GetAllUsers();
 
            return View(user);
        }
 
        [HttpPost]
        public ActionResult SignUp(UserModel user)
        {
            ModelState.Remove("AccountImage");
            ModelState.Remove("RoleName");
 
            if (ModelState.IsValid)
            {
                UserManager um = new UserManager();
                if (!um.IsLoginNameExist(user.LoginName))
                {
                    um.AddUserAccount(user);
                    // FormsAuthentication.SetAuthCookie(user.FirstName, false);
                    return RedirectToAction("", "Home");
                }
                else
                    ModelState.AddModelError("", "Login Name already taken.");
            }
            return View();
        }
 
        [HttpPut]
        public async Task<ActionResult> Update([FromBody] UserModel userData)
        {
            ModelState.Remove("Password");
            UserManager um = new UserManager();
            if (um.IsLoginNameExist(userData.LoginName))
            {
                um.UpdateUserAccount(userData);
                return RedirectToAction("Index"); // Redirect to a relevant action after successful update.
            }
            // Handle the case when the login name doesn't exist, e.g., return a relevant error view.
            return RedirectToAction("LoginNameNotFound");
        }
 
        [HttpPost]
        public ActionResult LogIn(UserLoginModel ulm)
        {
 
            if (ModelState.IsValid)
            {
                UserManager um = new UserManager();
 
                if (string.IsNullOrEmpty(ulm.Password))
                {
                    ModelState.AddModelError("", "The user login or password provided is incorrect.");
                }
                else
                {
                    if (um.GetUserPassword(ulm.LoginName).Equals(ulm.Password))
                    {
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, ulm.LoginName)
                    };
 
                        var userIdentity = new ClaimsIdentity(claims, "login");
 
                        ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
 
                        // Sign in the user using Cookie Authentication
                        HttpContext.SignInAsync(principal);
 
                        // Redirect to the desired action (e.g., "Users")
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The password provided is incorrect.");
                    }
                }
            }
 
            // If authentication fails or ModelState is invalid, redisplay the login form
            return View();
        }
 
        [HttpPost]
        public ActionResult LogOut()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}