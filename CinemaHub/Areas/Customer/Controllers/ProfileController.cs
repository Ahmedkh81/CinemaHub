using CinemaHub.Models;
using CinemaHub.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace CinemaHub.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            return View(user);
        }


        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            var editProfile = user.Adapt<EditProfileVM>();
            if (user is not null)
            { 
                return View(editProfile);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProfileVM editProfile)
        {
            if (!ModelState.IsValid)
            {
                return View(editProfile);
            }

            var user = await _userManager.FindByIdAsync(editProfile.Id);

            if (user is not null)
            {
                user.FirstName = editProfile.FirstName;
                user.LastName = editProfile.LastName;
                user.UserName = editProfile.UserName;
                user.Email = editProfile.Email;
                user.Address = editProfile.Address;

                await _userManager.UpdateAsync(user);

                TempData["success-notification"] = "Data Updated Successfully";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
                
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["success-notification"] = "Password Changed Successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

    }
}
