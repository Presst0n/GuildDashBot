using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DiscordBot.Server.EmailSender;
using DiscordBot.Server.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Server.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, ILogger<AccountController> logger, 
            IEmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager; 
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user is null)
                {
                    return RedirectToAction("Login");
                }

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View();
                }

                await _signInManager.RefreshSignInAsync(user);

                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
                return View("ResetPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account", 
                        new { email = model.Email, token = token }, Request.Scheme);

                    string message = $"<center><h4>Click link below to reset your password.<br/><a href=\"{passwordResetLink}\" style=\"color: red;\">{passwordResetLink}</a></h4></center>";

                    try
                    {
                        await _emailService.SendEmailAsync(user.Email, message, "Reset Password Link - DO_NOT_REPLY");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorTitle = $"Problem with sending email to the user.";
                        ViewBag.ErrorMessage = $"Message: {ex.Message} StackTrace: {ex.StackTrace}";

                        return View("Error");
                    }

                    _logger.Log(LogLevel.Warning, passwordResetLink);

                    return View("ForgotPasswordConfirmation");
                }

                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            await CreateSuperAdmin();

            return View();
        }

        public async Task CreateSuperAdmin()
        {
            var userEmail = _configuration.GetSection("UserSettings").GetValue<string>("UserEmail");

            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user is null)
            {
                var superUser = new IdentityUser()
                {
                    Email = userEmail,
                    UserName = userEmail,
                    EmailConfirmed = true
                };

                var tempPassword = _configuration.GetSection("UserSettings").GetValue<string>("UserPassword");

                var result = await _userManager.CreateAsync(superUser, tempPassword);

                if (result.Succeeded)
                {
                    var role = await _roleManager.FindByNameAsync("Super Admin");

                    if (role is null)
                    {
                        var newRole = new IdentityRole()
                        {
                            Name = "Super Admin"
                        };

                        var roleResult = await _roleManager.CreateAsync(newRole);

                        if (roleResult.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(superUser, "Super Admin");
                        }
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(superUser, "Super Admin");
                    }

                    var existingUserClaims = await _userManager.GetClaimsAsync(superUser);

                    if (existingUserClaims.Count < 1)
                    {
                        List<Claim> AllClaims = new List<Claim>()
                        {
                            new Claim("Create Role", "true"),
                            new Claim("Edit Role", "true"),
                            new Claim("Delete Role", "true"),
                            new Claim("Manage Claims", "true")
                        };


                        var claimResult = await _userManager.AddClaimsAsync(superUser, AllClaims);
                    }
                    else
                    {

                        await _userManager.AddClaimsAsync(superUser, existingUserClaims);
                    }
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && !user.EmailConfirmed && (await _userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token = token }, Request.Scheme);

                    string message = $"<center><h4>Click link below to activate your account.<br/><a href=\"{confirmationLink}\" style=\"color: red;\">{confirmationLink}</a></h4></center>";

                    try
                    {
                        await _emailService.SendEmailAsync(user.Email, message);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorTitle = $"Problem with sending email to the user.";
                        ViewBag.ErrorMessage = $"Message: {ex.Message} StackTrace: {ex.StackTrace}";

                        return View("Error");
                    }

                    _logger.Log(LogLevel.Warning, confirmationLink);

                    if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin") || _signInManager.IsSignedIn(User) && User.IsInRole("Super Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    ViewBag.SuccessTitle = "Registration successful";
                    ViewBag.SuccessMessage = "Before you can LogIn, please confirm your " +
                            "email, by clicking on the confirmation link we have emailed you";

                    return View("Success");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }
    }
}