using MFA.Dtos;
using MFA.Entities;
using MFA.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApp.Services;
using System.Net.Http.Json;
using System.Net.Http;

namespace MFA.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenServices _tokenServices;
        private readonly IEmailService _emailService;
        private readonly HttpClient _httpClient;
        private readonly string apiUrl = "https://api.linkedin.com/v2/jobs";

        public EmailMFA EmailMFA { get; set; }

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, ITokenServices tokenServices, IEmailService emailService
            )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._tokenServices = tokenServices;
            this._emailService = emailService;
            this.EmailMFA = new EmailMFA();
            _httpClient = new HttpClient();
        }

        //[HttpPost]
        //[Route("login")]
        //public async Task<ActionResult<string>> Login(LoginDto loginDto)
        //{
        //    var user = await _userManager.FindByEmailAsync(loginDto.Email);
        //    if (user == null)
        //    {
        //        return Unauthorized();
        //    }
        //   // var isTwoFactorClientRemembered = await IsTwoFactorClientRememberedAsync(user);
        //    var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password,true,false);

        //    //  var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        //    if (result.Succeeded)
        //    {
        //        return Unauthorized();
        //    }
        //    else
        //    {
        //        //problem istwologin na hole auth error khay
        //        if (result.RequiresTwoFactor)
        //        {
        //            var user1 = await _userManager.FindByEmailAsync(loginDto.Email);

        //            this.EmailMFA.SecurityCode = string.Empty;
        //            this.EmailMFA.RememberMe = true;

        //            // Generate the code
        //            var securityCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

        //            // Send to the user
        //            await _emailService.SendAsync("noufawal0311@gmail.com",
        //                loginDto.Email,
        //                "My Web App's OTP",
        //                $"Please use this code as the OTP: {securityCode}");

        //            // or get-code phase from AuthenticatorWithMFAController

        //            return "ok";
        //            //new UserDto
        //            //{
        //            //    Email = loginDto.Email,
        //            //    UserName = user.UserName,
        //            //    Token = _tokenServices.CreateToken(user),
        //            //    IsTwoFactor = true
        //            //};

        //        }
        //        else
        //        {
        //            return "two factor page";
        //            //return new UserDto
        //            //{
        //            //    Email = loginDto.Email,
        //            //    UserName = user.UserName,
        //            //    Token = _tokenServices.CreateToken(user),
        //            //    IsTwoFactor = false
        //            //};

        //        }

        //    }


        //}

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized();
            }
           
            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, true, false);

            if (result.Succeeded)
            {
                return new UserDto
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Token = _tokenServices.CreateToken(user)
                };
            }
            else
            {
                return NotFound();
            }
        }

        // one type of two way verification
        [HttpPost]
        [Route("twoFactorVerificationByEmail")]
        public async Task<ActionResult<UserDto>> TwoFactorVerificationByEmail(Credential credential)
        {
            var user = await _userManager.FindByEmailAsync(credential.Email);
            var result = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", credential.Securitycode);
            if (result == true)
            {
                return new UserDto
                {
                    Email = credential.Email,
                    UserName = user.UserName,
                    Token = _tokenServices.CreateToken(user),
                    Id = user.Id
                };
                //return null;
            }
            else
            {
                return Unauthorized();
            }
        }
        // this way of verification
        [HttpPost]
        [Route("twoFactorVerificationByOTP")]
        public async Task<ActionResult<UserDto>> twoFactorVerificationByOTP(Credential credential)
        {
            var user = await _userManager.FindByEmailAsync(credential.Email);
            var result = await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", credential.Securitycode);
            user.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);
            if (result == true)
            {
                //return new UserDto
                //{
                //    Email = credential.Email,
                //    UserName = user.UserName,
                //    Token = _tokenServices.CreateToken(user),
                //    Id = user.Id
                //};
                return null;
            }
            else
            {
                return Unauthorized();
            }
        }

        // or googlr auth service type verification
        [HttpPost]
        [Route("twoFactorVerificationByAuthCode")]
        public async Task<ActionResult<UserDto>> twoFactorVerificationByAuthCode(Credential credential)
        {
            var user = await _userManager.FindByEmailAsync(credential.Email);
           // var result = await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", credential.Securitycode);
            if (await this._userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                 credential.Securitycode))
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                
                return new UserDto
                {
                    Email = credential.Email,
                    UserName = user.UserName,
                    Token = _tokenServices.CreateToken(user),
                    Id = user.Id
                };
                //this.Succeeded = true;
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<string>> Register(RegisterDto registerDto)
        {
            string confirmationToken = null;
            var user = new AppUser
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                FirstName = registerDto.UserName,
                PhoneNumber="008801885831037"
               // TwoFactorEnabled = false
            };
            try
            {
                var clientURI = "https://localhost:44305/api/Account/ConfirmEmail";
                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    return Unauthorized();
                }
              //  await _userManager.SetTwoFactorEnabledAsync(user, true);
                confirmationToken = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var param = new Dictionary<string, string?>
                        {
                            {"token", confirmationToken },
                            {"userId", user.Id }
                        };
                var callback = QueryHelpers.AddQueryString(clientURI, param);

                await _emailService.SendAsync("cleinttest123@gmail.com",
                    user.Email,
                    "Please confirm your email",
                    $"Please click on this link to confirm your email address: {callback}");
            }
            catch (Exception ex)
            {
                new Exception(ex.Message);
            }

            //return new UserDto
            //{
            //    Email = registerDto.Email,
            //    UserName = user.UserName,
            //    Token = confirmationToken,
            //    Id = user.Id
            //};
            return "Please confirm email";
        }

        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<ActionResult<string>> ConfirmEmail([FromQuery] string token, [FromQuery] string userId)
        {
            var userEntity = await _userManager.FindByIdAsync(userId);
            var confirmResult = await _userManager.ConfirmEmailAsync(userEntity, token);

            return "please login";
        }

        [HttpGet]
        [Route("linkedInLogin")]
        public async Task<ActionResult<string>> linkedInLogin(string code)
        {
            //var userEntity = await _userManager.FindByIdAsync(userId);
            //var confirmResult = await _userManager.ConfirmEmailAsync(userEntity, token);

            var clientId = "78vqzhsn5aeie4";
            var clientSecret = "dlX3bATIxp6hGHjK";
            var redirectUri = "https://localhost:44305/api/account/linkedInLogin";

            var accessTokenEndpoint = "https://www.linkedin.com/oauth/v2/accessToken";
            var accessTokenRequest = new HttpRequestMessage(HttpMethod.Post, accessTokenEndpoint);

            var requestContent = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("redirect_uri", redirectUri)
        });

            accessTokenRequest.Content = requestContent;

            var response = await _httpClient.SendAsync(accessTokenRequest);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // Extract the access token from the response
                var accessToken = ExtractAccessToken(responseContent);
                string url = "http://api.linkedin.com/v1/people/~:(id,first-name,last-name,email-address,picture-url)?format=json";
                var test = GetJobDescriptions("Hikvision Bangladesh", accessToken);
              //  string json = oAuthLi.oAuthWebRequest(WebUtility.Method.GET, url, "");
               //return accessToken;
               return test.ToString();
            }

            throw new Exception("Failed to retrieve access token from LinkedIn.");

            //return "please login";
        }

        private string ExtractAccessToken(string responseContent)
        {
            // Parse the JSON response to extract the access token
            var responseObject = JObject.Parse(responseContent);
            var accessToken = responseObject.GetValue("access_token")?.ToString();

            if (string.IsNullOrEmpty(accessToken))
            {
                throw new Exception("Failed to extract access token from LinkedIn response.");
            }

            return accessToken;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            //  var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            return new UserDto
            {
                Email = user.Email,
                UserName = user.UserName,
                Token = _tokenServices.CreateToken(user)
            };

        }

        public async Task<string> GetJobDescriptions(string companyName,string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                // Set up headers and parameters
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var parameters = $"?keywords=software engineer&company={companyName}";

                // Make a request to the LinkedIn API
                var response = await httpClient.GetAsync($"{apiUrl}{parameters}");

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    var jobsData = await response.Content.ReadAsAsync<LinkedInJobsResponse>();
                    var jobDescriptions = string.Join("\n\n", jobsData.Elements.Select(job => $"{job.Title}\n{job.Description}"));
                    return jobDescriptions;
                }
                else
                {
                    return $"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}";
                }
            }
        }
    }
}

public class Credential
{
    public string Email { get; set; }
    public string Securitycode { get; set; }

}

public class LinkedInJobsResponse
{
    public List<LinkedInJob> Elements { get; set; }
}

public class LinkedInJob
{
    public string Title { get; set; }
    public string Description { get; set; }
}