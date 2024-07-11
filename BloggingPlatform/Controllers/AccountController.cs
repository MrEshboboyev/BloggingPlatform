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
        public IActionResult Login()
        {
            return View();
        }

        #endregion
    }
}
