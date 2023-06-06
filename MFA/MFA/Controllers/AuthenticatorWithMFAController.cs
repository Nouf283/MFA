using MFA.Dtos;
using MFA.Entities;
using MFA.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using System.Net;

namespace MFA.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticatorWithMFAController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenServices _tokenServices;
        private readonly IEmailService _emailService;
        public EmailMFA EmailMFA { get; set; }

        public AuthenticatorWithMFAController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, ITokenServices tokenServices, IEmailService emailService
            )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._tokenServices = tokenServices;
            this._emailService = emailService;
            this.EmailMFA = new EmailMFA();
        }

        [HttpPost]
        [Route("get-code")]
        public async Task<ActionResult<string>> getCode(LoginDto loginDto)
        {
            var user = await _userManager.GetUserAsync(base.User);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            return key;
           
        }

        [HttpPost]
        [Route("code-check")]
        public async Task<ActionResult<bool>> codeCheck(string key)
        {
            var user = await _userManager.GetUserAsync(base.User);
            if (await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                key))
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
            }
            else
            {
                return Unauthorized();
            }

            return true;
        }

       
    }
}

