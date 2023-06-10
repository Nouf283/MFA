using MFA.Dtos;
using MFA.Entities;
using MFA.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
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


        [HttpGet("qrcode-setup")]
        public async Task<IActionResult> GetQrCodeSetup(string email)
        {
            var ViewModel = new SetupMFAViewModel();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("User does not exist");
            var isTfaEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (key == null)
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            ViewModel.Key = key;
            ViewModel.QRCodeBytes = GenerateQRCodeBytes("my web app", key, user.Email);

            // Convert the QR code image to base64 string
            var base64Image = Convert.ToBase64String(ViewModel.QRCodeBytes);

            // Return the base64 image string in the API response
            return Ok(new { QrCodeImage = base64Image });

            //var formattedKey = GenerateQrCode("", authenticatorKey);
            //return Ok(new TfaSetupDto
            //{ IsTfaEnabled = isTfaEnabled, AuthenticatorKey = authenticatorKey, FormattedKey = formattedKey });
        }

        [HttpPost("tfa-setup")] // make object
        public async Task<IActionResult> PostTfaSetup([FromBody] SetupMFAViewModel setupMFAViewModel)
        {
            var user = await _userManager.FindByNameAsync(setupMFAViewModel.Email);
            var isValidCode = await _userManager
                .VerifyTwoFactorTokenAsync(user,
                  _userManager.Options.Tokens.AuthenticatorTokenProvider,
                  setupMFAViewModel.SecurityCode);
            if (isValidCode)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                return Ok(new SetupMFAViewModel { IsTfaEnabled = true });
            }
            else
            {
                return BadRequest("Invalid code");
            }
        }


        private Byte[] GenerateQRCodeBytes(string provider, string key, string userEmail)
        {
            var qrCodeGenerater = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerater.CreateQrCode(
                $"otpauth://totp/{provider}:{userEmail}?secret={key}&issuer={provider}",
                QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);
            return BitmapToByteArray(qrCodeImage);
        }

        private Byte[] BitmapToByteArray(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }

}

public class SetupMFAViewModel
{
    public string Key { get; set; }
    public string Email { get; set; }

    [Required]
    [Display(Name = "Code")]
    public string SecurityCode { get; set; }

    public Byte[] QRCodeBytes { get; set; }
    public bool IsTfaEnabled { get; set; }
}