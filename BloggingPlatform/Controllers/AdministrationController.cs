using BloggingPlatform.Models;
using BloggingPlatform.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloggingPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        // DI : RoleManager
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AdministrationController(RoleManager<ApplicationRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
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
                    var role = new ApplicationRole
                    {
                        Name = model.RoleName,
                        Description = model.Description
                    };


                    var result = await _roleManager.CreateAsync(role);

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
            List<ApplicationRole> roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        #endregion

        #region Edit Role

        [HttpGet]
        public async Task<IActionResult> EditRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ModelState.AddModelError(string.Empty, $"Role with Id = {roleId} not found!");
            }
            else
            {
                // instance EditRoleViewModel
                var model = new EditRoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Description = role.Description,
                    Users = new List<string?>()
                };

                // adding is in role user to Users list 
                foreach (var user in await _userManager.Users.ToListAsync())
                {
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        model.Users.Add(user.UserName);
                    }
                }

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
                    role.Description = model.Description;

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

        #region Delete Role

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ModelState.AddModelError(string.Empty, $"Role with Id = {roleId} is not found!");
                return View("Error");
            }
            else
            {
                try
                {
                    var result = await _roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return RedirectToAction("ListRoles", await _roleManager.Roles.ToListAsync());
                }
                catch (DbUpdateException ex)
                {
                    // Log the exception to a file. 
                    ViewBag.Error = ex.Message;
                    // Pass the ErrorTitle and ErrorMessage that you want to show to the user using ViewBag.
                    // The Error view retrieves this data from the ViewBag and displays to the user.
                    ViewBag.ErrorTitle = $"{role.Name} Role is in Use";
                    ViewBag.ErrorMessage = $"{role.Name} Role cannot be deleted as there are users in this role. If you want to delete this role, please remove the users from the role and then try to delete";

                    TempData["ErrorTitle"] = $"{role.Name} Role is in Use";
                    TempData["ErrorMessage"] = $"{role.Name} Role cannot be deleted as there are users in this role. If you want to delete this role, please remove the users from the role and then try to delete";
                    TempData["ErrorDetails"] = ex.Message;
                    return RedirectToAction("Error", "Error");
                    throw;
                }
            }
        }
        #endregion

        #region Edit Users In Role

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ModelState.AddModelError(string.Empty, $"Role with Id = {roleId} is not found!");
                return View("Error");
            }
            List<UserRoleViewModel> model = new List<UserRoleViewModel>();

            // get all users
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                // create new user Role View model
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                // adding prepared userRoleViewModel to model 
                model.Add(userRoleViewModel);

            }

            ViewBag.RoleId = roleId;
            ViewBag.RoleName = role.Name;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            // check role exists
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ModelState.AddModelError(string.Empty, $"Role with Id = {roleId} is not found!");
                return View("Error");
            }
            else
            {
                for (int i = 0; i < model.Count; i++)
                {
                    var user = await _userManager.FindByIdAsync(model[i].UserId);

                    if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        await _userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                    else if (model[i].IsSelected && !await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                    else
                        continue;

                    if (i < model.Count - 1)
                        continue;
                    else
                        return RedirectToAction("EditRole", new { roleId = roleId });
                }

                return RedirectToAction("EditRole", new { roleId = roleId });
            }
        }

        #endregion

        #region List Users

        public async Task<IActionResult> ListUsers()
        {
            IEnumerable<IdentityUser> model = await _userManager.Users.ToListAsync();
            return View(model);
        }

        #endregion

        #region Edit User

        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            // checking a user in database
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {userId} cannot be found";
                return View("NotFound");
            }

            // if user was found
            var claims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles,
                Claims = claims.Select(c => c.Value).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            // checking user in database
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {model.Id} cannot be found";
                return View("NotFound");
            }

            // update fields 
            user.Email = model.Email;
            user.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(user);

            if(result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }
            else
            {
                // adding errors to ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }


            return View(model);
        }

        #endregion

        #region Delete User

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            // checking user in database
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {userId} cannot be found";
                return View("NotFound");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers", "Administration");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Error");
        }

        #endregion

        #region Manager User Roles

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            // checking user in db
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with ID = {userId} cannot be found";
                return View("NotFound");
            }

            // added ViewBag for displaying view
            ViewBag.UserId = userId;
            ViewBag.UserName = user.UserName;

            // create model type UserRolesViewModel
            List<UserRolesViewModel> model = new List<UserRolesViewModel>();

            var roles = await _roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Description = role.Description
                };

                if(await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);
            }

            return View(model);
        }

        #endregion
    }
}
