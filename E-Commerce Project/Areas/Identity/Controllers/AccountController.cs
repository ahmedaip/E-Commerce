using E_Commerce_Project.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace E_Commerce_Project.Areas.Identity.Controllers
{
    [Area(CD.IDENTITY_AREA)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRepository<ApplicationUserOTP> applicationUserOTPRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _applicationUserOTPRepository = applicationUserOTPRepository;
        }

        [HttpGet]
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
            var user = new ApplicationUser()
            {
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                Address = registerVM.Address,
                Email = registerVM.Email,
                UserName = registerVM.UserName,
            };
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(user, CD.ROLE_CUSTOMER);

            //Send Email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = CD.IDENTITY_AREA, userId = user.Id, token = token }, Request.Scheme);
            await _emailSender.SendEmailAsync(
                registerVM.Email,
                "Ecommerce confirmation email",
                $"<h1>Please confirm your email by clicking on the following link: <a href={link}>Confirm Email</a></h1>"
            );


            TempData["Success_Notification"] = "Confirmation email sent successfully";

            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult Login()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = CD.CUSTOMER_AREA });
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }
            var user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail) ??
                await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "Invalid Username or Password");
                return View(loginVM);
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Your account is locked. Please try again later.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Please confirm your email before logging in.");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Username or Password");
                }
                return View(loginVM);
            }
            return RedirectToAction("Index", "Home", new { area = CD.CUSTOMER_AREA });
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();


            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                string errors = "";
                foreach (var error in result.Errors)
                {
                    errors += error.Description + "\n";
                }
                TempData["Error_Notification"] = errors;
                return RedirectToAction(nameof(Login), "Account", new { area = CD.IDENTITY_AREA });
            }
            TempData["Success_Notification"] = "Email confirmed successfully";
            return RedirectToAction(nameof(Login), "Account", new { area = CD.IDENTITY_AREA });
        }



        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {

            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.UserNameOrEmail) ??
                await _userManager.FindByNameAsync(resendEmailConfirmationVM.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "Invalid Username or Password");
                return View(resendEmailConfirmationVM);
            }

            //Send Email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = CD.IDENTITY_AREA, userId = user.Id, token = token }, Request.Scheme);
            await _emailSender.SendEmailAsync(
                user.Email,
                "Ecommerce confirmation email",
                $"<h1>Please confirm your email by clicking on the following link: <a href={link}>Confirm Email</a></h1>"
            );
            return RedirectToAction(nameof(Login));
        }




        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail) ??
                await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError("", "Invalid Username or Email");
                return View(forgetPasswordVM);
            }

            var otp = new Random().Next(1000, 9999).ToString();
            var applicationUserOTP = new ApplicationUserOTP(user.Id, otp);

            await _applicationUserOTPRepository.CreateAsync(applicationUserOTP);
            await _applicationUserOTPRepository.CommitAsync();

            //send otp Email
            await _emailSender.SendEmailAsync(
               user.Email,
               "Ecommerce Reset Password",
               $"<h1>Use this OTP to reset your password: <span style='color: red; font-weight: bold;'>{otp}</span></h1>"
           );


            TempData["Success_Notification"] = "OTP sent successfully";

            return RedirectToAction(nameof(VerifyOTP), new { userId = user.Id });
        }


        [HttpGet]
        public IActionResult VerifyOTP(string userId)
        {
            return View(new VerifyOTPVM() { UserId = userId });
        }
        [HttpPost]
        public async Task<IActionResult> VerifyOTP(VerifyOTPVM verifyOTPVM)
        {
            var user = await _userManager.FindByIdAsync(verifyOTPVM.UserId);
            if (user is null)
            {
                ModelState.AddModelError("", "Invalid User");
                return View(verifyOTPVM);
            }
            var opts = await _applicationUserOTPRepository.GetAllAsync(o => o.ApplicationUserId == user.Id && o.IsValid && o.ValidTo > DateTime.UtcNow);
            var otp = opts.OrderBy(o => o.CreatedAt).LastOrDefault();

            if (otp is null || otp.OTP != verifyOTPVM.OTP)
            {
                ModelState.AddModelError("", "Invalid or expired OTP");
                return View(verifyOTPVM);
            }
            otp.IsValid = false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _applicationUserOTPRepository.CommitAsync();

            return RedirectToAction(nameof(ResetPassword), new { userId = user.Id , token = token});
        }



        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            return View(new ResetPasswordVM() { UserId = userId, Token = token });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {

            var user = await _userManager.FindByIdAsync(resetPasswordVM.UserId);
            if (user is null)
            {
                ModelState.AddModelError("", "Invalid User");
                return View(resetPasswordVM);
            }


            var result = await _userManager.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(resetPasswordVM);
            }
            return RedirectToAction(nameof(Login));
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }




    }
}
