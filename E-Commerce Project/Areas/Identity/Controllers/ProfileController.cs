using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Commerce_Project.Areas.Identity.Controllers
{
    [Area(CD.IDENTITY_AREA)]
    [Authorize]
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
            if (user == null)
            {
                return NotFound();
            }
            //var userVM = new ApplicationUserVM()
            //{
            //    FirstName = user.FirstName,
            //    LastName = user.LastName,
            //    Address = user.Address,
            //    PhoneNumber = user.PhoneNumber,
            //    Email = user.Email
            //};
            var userVM = user.Adapt<ApplicationUserVM>();
            return View(userVM);
        }


        public async Task<IActionResult> UpdateProfile(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            user.FirstName = applicationUserVM.FirstName;
            user.LastName = applicationUserVM.LastName;
            user.Address = applicationUserVM.Address;
            user.PhoneNumber = applicationUserVM.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                TempData["Error_Notification"] = string.Join(", ", result.Errors.Select(e => e.Description));
                return View(nameof(Index), applicationUserVM);
            }
            TempData["Success_Notification"] = "Profile updated successfully";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> UpdatePassword(UpdatePasswordVM updatePasswordVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, updatePasswordVM.CurrentPassword, updatePasswordVM.NewPassword);

            if (!result.Succeeded)
            {
                TempData["Error_Notification"] = string.Join(", ", result.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Index));
            }
            TempData["Success_Notification"] = "Password updated successfully";
            return RedirectToAction(nameof(Index));
        }

    }
}
