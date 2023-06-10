using MFA.Dtos;
using MFA.Entities;
using MFA.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApp.Services;

namespace MFA.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class QRCodeAuthMFAController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenServices _tokenServices;
        private readonly IEmailService _emailService;
        public EmailMFA EmailMFA { get; set; }

        public QRCodeAuthMFAController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, ITokenServices tokenServices, IEmailService emailService
            )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._tokenServices = tokenServices;
            this._emailService = emailService;
            this.EmailMFA = new EmailMFA();
        }

    
}

public class Credential
{
    public string Email { get; set; }
    public string Securitycode { get; set; }

}