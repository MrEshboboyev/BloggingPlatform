using BloggingPlatform.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloggingPlatform.Controllers
{
    public class AdministrationController : Controller
    {
        // DI : RoleManager
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        #region Create Role

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool roleExists = await _roleManager.RoleExistsAsync(model.RoleName);

                if (roleExists)
                {
                    ModelState.AddModelError(string.Empty, $"Role {model.RoleName} already exists!");
                }
                else
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole { Name = model.RoleName });

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        #endregion

        #region List Roles

        [HttpGet]
        public async Task<IActionResult> ListRoles()
        {
            List<IdentityRole> roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        #endregion

        #region Edit Role

        [HttpGet]
        public async Task<IActionResult> EditRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if(role == null)
            {
                ModelState.AddModelError(string.Empty, $"Role with Id = {roleId} not found!");
            }
            else
            {
                // instance EditRoleViewModel
                var model = new EditRoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                return View(model);
            }

            return RedirectToAction("ListRoles", "Administration");
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);

                if (role == null)
                {
                    ModelState.AddModelError(string.Empty, $"Role with Id = {model.RoleId} is not found!");
                    return View("Error");
                }
                else
                {
                    role.Name = model.RoleName;
                 
                    var result = await _roleManager.UpdateAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(model);
        }

        #endregion
    }
}
