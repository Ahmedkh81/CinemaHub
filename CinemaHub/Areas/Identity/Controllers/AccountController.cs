using CinemaHub.Models;
using CinemaHub.Repositories;
using CinemaHub.Repositories.IRepositories;
using CinemaHub.Utility;
using CinemaHub.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Common;

namespace CinemaHub.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IApplicationUserOTPRepository _applicationUserOTPRepository;

        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, IApplicationUserOTPRepository applicationUserOTPRepository)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _applicationUserOTPRepository = applicationUserOTPRepository;
        }

        public IActionResult Register() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            ApplicationUser user = new()
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                Address = registerVM.Address
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Sd.Customer);

                TempData["success-notification"] = "Registration successful! Please confirm your email address.";
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(ConfirmEmail), "Account"
                    , new 
                    {
                        userId = user.Id,
                        Token = token,
                        area = "Identity"
                    }, Request.Scheme);

                await _emailSender.SendEmailAsync(user!.Email ?? "", "Confirm Your Account"
                    , $"<h1>Our Dear Customer Please, Confirm Your Email By Clicking" +
                    $"<a href= '{link}'> Here</a></h1>");

                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }
            return View(registerVM);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);

            if (user is null)
            {
                user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);
            }

            if (user is not null)
            {
                var result = await _userManager.CheckPasswordAsync(user, loginVM.Password);

                if (result)
                {
                    if (!user.EmailConfirmed)
                    {
                        TempData["error-notification"] = "Please confirm your email address before logging in. " +
                            "<a href='/Identity/Account/ResendEmailConfirmation' class='text-info'>Resend it here</a>";
                        return View(loginVM);
                    }

                    if (!user.LockoutEnabled)
                    {
                        TempData["error-notification"] = $"You have a block till {user.LockoutEnd}";
                        return View(loginVM);
                    }

                    await _signInManager.SignInAsync(user, loginVM.RememberMe);
                    TempData["success-notification"] = "Login Successfully";
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
            }
            ModelState.AddModelError("UserNameOrEmail", "Invalid UserName Or Email");
            ModelState.AddModelError("Password", "Invalid Password");
            return View(loginVM);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is not null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    TempData["success-notification"] = "Email confirmed successfully! You can now log in.";
                }
                else
                {
                    TempData["error-notification"] = $"{String.Join(",", result.Errors)}";
                }
                return RedirectToAction("index", "Home", new { area = "Customer" });
            }
            return NotFound();
        }

        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            TempData["success-notification"] = "Logout Successfully";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resendEmailConfirmationVM);
            }

            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.EmailOrUserName);

            if (user is null)
            {
                user = await _userManager.FindByNameAsync(resendEmailConfirmationVM.EmailOrUserName);
            }

            if (user is not null)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, token = token, area = "Identity" }, Request.Scheme);

                await _emailSender.SendEmailAsync(user!.Email ?? "", "Reconfirm Your Account!", $"<h1>Confirm Your Account By Clicking <a href='{link}'>here</a></h1>");

                TempData["success-notification"] = "Email Sent Successfully";

                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
            return View(resendEmailConfirmationVM);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(forgotPasswordVM);
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordVM.EmailOrUserName);

            if (user is null)
            {
                user = await _userManager.FindByNameAsync(forgotPasswordVM.EmailOrUserName);
            }

            if (user is not null)
            {
                var otpNumber = new Random().Next(1000, 9999);

                var totalNumberOfOTPs = (await _applicationUserOTPRepository.GetAsync(e => e.ApplicationUserId == user.Id && DateTime.UtcNow.Day == e.SendDate.Day));

                if (totalNumberOfOTPs.Count() > 5)
                {
                    TempData["error-notification"] = "Many Requests of OTPs";
                    return View(forgotPasswordVM);
                }

                await _applicationUserOTPRepository.CreateAsync(new()
                {
                    ApplicationUserId = user.Id,
                    OTPNumber = otpNumber,
                    SendDate = DateTime.UtcNow,
                    Status = false,
                    ValidTo = DateTime.UtcNow.AddMinutes(30)
                });

                await _emailSender.SendEmailAsync(user!.Email ?? "", "OTP Your Account", $"<h1>Reset Password using OTP: {otpNumber}</h1>");

                TempData["success-notification"] = "OTP Sent to your Email Successfully";

                //TempData["Redirection"] = Guid.NewGuid();

                return RedirectToAction("ResetPassword", "Account", new { area = "Identity", userId = user.Id });
            }

            return View(forgotPasswordVM);
        }

        public async Task<IActionResult> ResetPassword(string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);

            if (user is not null)
            {
                return View(new ResetPasswordVM()
                {
                    UserId = userId
                });
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordVM);
            }

            var user = await _userManager.FindByIdAsync(resetPasswordVM.UserId);

            if (user is not null)
            {
                var lastOTP = (await _applicationUserOTPRepository.GetAsync(e => e.ApplicationUserId == resetPasswordVM.UserId)).OrderBy(e => e.Id).LastOrDefault();

                if (lastOTP is not null)
                {
                    if (lastOTP.OTPNumber == resetPasswordVM.OTP && (lastOTP.ValidTo - DateTime.UtcNow).TotalMinutes < 30 && !lastOTP.Status)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordVM.Password);

                        if (result.Succeeded)
                        {
                            TempData["success-notification"] = "Reset Password Successfully";
                        }
                        else
                        {
                            TempData["error-notification"] = $"{String.Join(",", result.Errors)}";
                        }

                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                    }
                }

                TempData["error-notification"] = "Invalid OR Expired OTP";
                return View(resetPasswordVM);
            }

            return NotFound();
        }

    }
}
