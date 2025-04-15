using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using VeteranAnalyticsSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace VeteranAnalyticsSystem.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Users/Index (List of users)
        public async Task<IActionResult> Index()
        {
            // Fetch all users
            var users = await _userManager.Users.ToListAsync();

            // Pass the users to the view
            return View(users);  // This will pass the list of users to the Users/Index view
        }

        // GET: Users/Settings
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            return View(user); // Pass the current user object to the view
        }

        // POST: Users/UpdateAccount
        [HttpPost]
        public async Task<IActionResult> UpdateAccount(string currentPassword, string newPassword, string confirmNewPassword, string email, string phoneNumber)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            // Check if the current password is correct
            var passwordCheckResult = await _userManager.CheckPasswordAsync(user, currentPassword);

            if (!passwordCheckResult)
            {
                ModelState.AddModelError(string.Empty, "The current password is incorrect.");
            }

            // Update phone number and email
            user.PhoneNumber = phoneNumber;
            user.Email = email;

            // Update the password if new password is provided and matches the confirmation
            if (!string.IsNullOrEmpty(newPassword) && newPassword == confirmNewPassword)
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError(string.Empty, "New password and confirmation password do not match.");
            }

            // Only update if the ModelState is valid
            if (!ModelState.IsValid)
            {
                return View("Settings", user);  // If ModelState has errors, return to the view with the errors
            }

            // Save the changes if no validation errors
            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Your account has been updated successfully!";
                return RedirectToAction("Settings");
            }

            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Settings", user);
        }


    }
}