using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FCS_Analysis.Models;
using FCS_Analysis.Models.Entities;
using FCS_Analysis.Models.ViewModel;
using FCS_Analysis.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCS_Analysis.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public UsersController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Login(string returnUrl = "/")
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User model, string returnUrl = "/")
        {
            User loginUser = null;
            loginUser = _dbContext.Users.Where(user => user.user_email == model.user_email && user.user_password == model.user_password).FirstOrDefault();
            
            if (loginUser == null)
            {
                ViewData["ErrorMessage"] = "Your credential is incorrect.";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
                
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, loginUser.user_role),
                new Claim("user_name", loginUser.user_name == null ? "" : loginUser.user_name),
                new Claim("user_id", "" + loginUser.user_id)
            };

            var userIdentity = new ClaimsIdentity(claims, "user");

            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(principal);

            return Redirect(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");

            if (!ModelState.IsValid)
            {
                return View();
            }

            // check there is same user email with this model.
            int nCnt = _dbContext.Users.Where(user => user.user_email == model.user_email).Count();
            if (nCnt > 0)
            {
                ViewData["ErrorMessage"] = "There exists an user with this email. please try with other one.";
                return View();
            }

            string filePath = "/uploads/avatars/";
            if (model.user_avatar_image != null && model.user_avatar_image.Length > 0)
            {
                string fileName = model.user_avatar_image.FileName;
                int nIdx = fileName.LastIndexOf('\\');
                nIdx = nIdx > 0 ? nIdx + 1 : 0;
                fileName = fileName.Substring(nIdx);

                filePath = "/uploads/avatars/" + Path.GetRandomFileName() + "_" + fileName;
                string fullPath = Path.GetFullPath("./wwwroot") + filePath;

                using (var stream = System.IO.File.Create(fullPath))
                {
                    await model.user_avatar_image.CopyToAsync(stream);
                }
            }
            model.user_avatar = filePath;
            _dbContext.Users.Add(model);
            _dbContext.SaveChanges();

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, model.user_role),
                new Claim("user_name", model.user_name == null ? "" : model.user_name),
                new Claim("user_id", "" + model.user_id),
            };

            var userIdentity = new ClaimsIdentity(claims, "user");

            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(principal);

            return Redirect("/");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Account/Login");
        }

        [Authorize]
        public IActionResult MyAccount()
        {
            User currentUser = _dbContext.Users.Find(Convert.ToInt32(User.FindFirstValue("user_id")));
            return View(currentUser);
        }

        [Authorize(Roles = Constants.ROLE_ADMIN)]
        public IActionResult Customers(string message = "")
        {
            List<User> arrUsers = _dbContext.Users.Where(user => user.user_role != Constants.ROLE_ADMIN).ToList();
            ViewData["message"] = message;
            return View(arrUsers);
        }

        [Authorize(Roles = Constants.ROLE_ADMIN)]
        [HttpPost]
        public IActionResult UpdateUser(User model)
        {
            string message = "";
            int nCnt = _dbContext.Users.Where(user => user.user_email == model.user_email).Count();
            if (nCnt > 0 && model.user_id == 0)
            {
                message = "There exists an user with this email. please try with other one.";
                return RedirectToAction("Customers", message);
            }

            _dbContext.Users.Update(model);
            _dbContext.SaveChanges();

            if (model.user_id == 0)
            {
                message = "Created new user successfully.";
            }
            else
            {
                message = "Updated the user successfully.";
            }

            return RedirectToAction("Customers", message);
        }
    }
}