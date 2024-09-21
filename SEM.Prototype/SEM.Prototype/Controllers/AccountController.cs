using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SEM.Prototype.Models;

namespace SEM.Prototype.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["SuccessMessage"] = "Sign-up successful! You are now logged in.";
                    return RedirectToAction("Index", "Home");
                }

                // Handle specific errors
                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName")
                    {
                        ModelState.AddModelError("", "A user with this email already exists.");
                    }
                    else if (error.Code == "PasswordTooShort")
                    {
                        ModelState.AddModelError("", "The password is too short.");
                    }
                    else if (error.Code == "PasswordRequiresNonAlphanumeric")
                    {
                        ModelState.AddModelError("", "The password must contain at least one non-alphanumeric character.");
                    }
                    else
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "No account found with this email.";
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Sign-in successful! Welcome back.";
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    TempData["ErrorMessage"] = "Your account is locked out. Please try again later.";
                }
                else if (result.IsNotAllowed)
                {
                    TempData["ErrorMessage"] = "You are not allowed to sign in at this time.";
                }
                else if (result.RequiresTwoFactor)
                {
                    TempData["ErrorMessage"] = "Two-factor authentication is required.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid password. Please try again.";
                }
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            // Clear TempData after logging out to prevent messages from persisting
            TempData.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
