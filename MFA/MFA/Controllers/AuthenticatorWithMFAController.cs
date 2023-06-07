using System.Net.Http;
using System.Net.Http.Headers;
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
using Newtonsoft.Json;

namespace MFA.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
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
            var client = new HttpClient();
            //var user = await _userManager.GetUserAsync(base.User);
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            var securityCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");
            await _userManager.ResetAuthenticatorKeyAsync(user);
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            string API_Url = "https://api.sms.net.bd/sendsms";
            string API_Key = "tP7Fk8U4GQq5JY2q2Bmf95zuZ92UFK7Z7t4pY37X";
            string Text_Massage = key+" "+securityCode;
            string To_Phone_Number = "8801885831037";

            client.BaseAddress = new Uri(API_Url);// //
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("?api_key=" + API_Key + "&msg=" + Text_Massage + "&to=" + To_Phone_Number).Result;
            using (HttpContent content = response.Content)
            {
                var bkresult = content.ReadAsStringAsync().Result;
                dynamic stuff = JsonConvert.DeserializeObject(bkresult);
                if (stuff.error == "0")
                {
                    Console.WriteLine(stuff.msg);
                }
                else
                {
                    Console.WriteLine("Sms Not Send, " + stuff.msg);
                }

            }
            return key;

        }

        [HttpPost]
        [Route("code-check")]
        public async Task<ActionResult<bool>> codeCheck(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                loginDto.Key))
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

