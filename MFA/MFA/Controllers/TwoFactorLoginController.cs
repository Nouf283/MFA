using MFA.Entities;
using MFA.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebApp.Services;

namespace MFA.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TwoFactorLoginController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenServices _tokenServices;
        private readonly IEmailService _emailService;
        public EmailMFA EmailMFA { get; set; }

        public TwoFactorLoginController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, ITokenServices tokenServices, IEmailService emailService
            )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._tokenServices = tokenServices;
            this._emailService = emailService;
            this.EmailMFA = new EmailMFA();
        }

        [HttpGet]
        [Route("OnGetOTP")]
        public async Task OnGetOTP([FromQuery] string email, [FromQuery] bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);

            this.EmailMFA.SecurityCode = string.Empty;
            this.EmailMFA.RememberMe = rememberMe;

            // Generate the code
            var securityCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            // Send to the user
            await _emailService.SendAsync("noufawal0311@gmail.com",
                email,
                "My Web App's OTP",
                $"Please use this code as the OTP: {securityCode}");
        }
        //[HttpPost]
        //public async Task<IActionResult> OnPostAsync()
        //{
        //  //  if (!ModelState.IsValid) return Page();

        //    var result = await _signInManager.TwoFactorSignInAsync("Email",
        //        this.EmailMFA.SecurityCode,
        //        this.EmailMFA.RememberMe,
        //        false);

        //    if (result.Succeeded)
        //    {
        //       // return RedirectToPage("/Index");
        //       //login with token
        //    }
        //    else
        //    {
        //        //if (result.IsLockedOut)
        //        //{
        //        //    ModelState.AddModelError("Login2FA", "You are locked out.");
        //        //}
        //        //else
        //        //{
        //        //    ModelState.AddModelError("Login2FA", "Failed to login.");
        //        //}

        //       // return Page();

        //        //return error
        //        //return null;
        //    }
        //}
    }
}
public class EmailMFA
{
    [Required]
    [Display(Name = "Security Code")]
    public string SecurityCode { get; set; }
    public bool RememberMe { get; set; }
}
