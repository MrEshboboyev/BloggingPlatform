using BloggingPlatform.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BloggingPlatform.Controllers
{
    public class AccountController : Controller
    {
        // DI for IdentityClasses 

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                // initialize IdentityUser
                IdentityUser user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,                    
                };

                // create a new user 
                var result = await _userManager.CreateAsync(user, model.Password);

                // user create success
                if (result.Succeeded)
                {
                      // signing in user
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                // user create failed
                foreach (var error in result.Errors)
                {
                    // errors added in ModelState
                    ModelState.AddModelError(string.Empty, error.Description);  
                }
            }

            return View(model);
        }
        #endregion

        #region Login

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, 
                    model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                // display errors
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt!");
                    return View(model);
                }
            }

            TempData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        #endregion

        #region Logout

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
