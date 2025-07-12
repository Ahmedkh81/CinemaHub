using CinemaHub.Models;
using CinemaHub.Utility;
using CinemaHub.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Data;

namespace CinemaHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{Sd.SuperAdmin},{Sd.Admin}")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = _userManager.Users.ToList();

            Dictionary<ApplicationUser, string> userRoles = new ();

            foreach (var item in user)
            {
                userRoles.Add(item, string.Join(", ", await _userManager.GetRolesAsync(item)));
            }

            return View(userRoles.ToDictionary());
        }

        public IActionResult Create()
        {
            var adminCreateUserVM = new AdminCreateUserVM
            {
                Roles = _roleManager.Roles.Select(e => new SelectListItem()
                {
                    Text = e.Name,
                    Value = e.Name
                }).ToList()
            };
            return View(adminCreateUserVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminCreateUserVM adminCreateUserVM, List<string> roles)
        {
            if (!ModelState.IsValid)
            {
                adminCreateUserVM.Roles = _roleManager.Roles.Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Name
                }).ToList();
                return View(adminCreateUserVM);
            }
            var user = new ApplicationUser
            {
                FirstName = adminCreateUserVM.FirstName,
                LastName = adminCreateUserVM.LastName,
                UserName = adminCreateUserVM.UserName,
                Email = adminCreateUserVM.Email,
                Address = adminCreateUserVM.Address
            };
            var result = await _userManager.CreateAsync(user, adminCreateUserVM.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, roles);

                if (adminCreateUserVM.IsEmailConfirmed)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _userManager.ConfirmEmailAsync(user, token);
                }

                TempData["success-notification"] = "User Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            adminCreateUserVM.Roles = _roleManager.Roles.Select(e => new SelectListItem
            {
                Text = e.Name,
                Value = e.Name
            }).ToList();

            return View(adminCreateUserVM);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var regiterVM = user.Adapt<AdminRegisterVM>();
            if (user is not null)
            {
                regiterVM.Roles = _roleManager.Roles.Select(e => new SelectListItem()
                {
                    Text = e.Name,
                    Value = e.Name
                }).ToList();
                return View(regiterVM);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdminRegisterVM adminRegisterVM, List<string> roles)
        {
            if (!ModelState.IsValid)
            {
                return View(adminRegisterVM);
            }

            var user = await _userManager.FindByIdAsync(adminRegisterVM.Id);

            if (user is not null)
            {
                user.FirstName = adminRegisterVM.FirstName;
                user.LastName = adminRegisterVM.LastName;
                user.UserName = adminRegisterVM.UserName;
                user.Email = adminRegisterVM.Email;
                user.Address = adminRegisterVM.Address;

                await _userManager.UpdateAsync(user);

                var userRoles = await _userManager.GetRolesAsync(user);

                await _userManager.RemoveFromRolesAsync(user, userRoles);

                await _userManager.AddToRolesAsync(user, roles);

                TempData["success-notification"] = "Data Updated Successfully";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is not null)
            {
                await _userManager.DeleteAsync(user);

                TempData["success-notification"] = "User Data Deleted Successfully";

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is not null)
            {
                if (user.LockoutEnabled)
                {
                    user.LockoutEnabled = false;
                    user.LockoutEnd = DateTime.UtcNow.AddMonths(1);
                    TempData["success-notification"] = "Block User Successfully";
                }
                else
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = null;
                    TempData["success-notification"] = "UnBlock User Successfully";
                }

                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
    }
}
