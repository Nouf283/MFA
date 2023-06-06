using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace MFA.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        [HttpGet("testauth")]
        [Authorize]
        public ActionResult<string> GetSecretText()
        {
            return "secret key";
        }

        //[HttpPost]
        //public IActionResult Authenticate([FromBody] Credential credential)
        //{
        //    if (credential.UserName == "admin" && credential.Password == "password")
        //    {
        //        var claims = new List<Claim>
        //            {
        //                new Claim(ClaimTypes.Name,"admin"),
        //                new Claim(ClaimTypes.Email,"admin@gmail.com"),
        //                new Claim("Department", "HR"),
        //                new Claim("Admin", "true"),
        //                new Claim("Manager", "true"),
        //                new Claim("EmploymentDate", "2020-02-01")
        //            };
        //        var expiresAt = DateTime.UtcNow.AddMinutes(10);
        //        return Ok(new
        //        {
        //            access_token = CreateToken(claims, expiresAt),
        //            expires_at = expiresAt
        //        });
        //    }

        //    ModelState.AddModelError("UnAuthorized", "You are not allowed to access the endpoint");
        //    return Unauthorized(ModelState);
        //}

        //private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
        //{
        //    var secretKey = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("secretKey"));
        //    var jwt = new JwtSecurityToken(
        //        claims: claims,
        //        notBefore: DateTime.UtcNow,
        //        expires: expireAt,
        //        signingCredentials: new SigningCredentials(
        //            new SymmetricSecurityKey(secretKey),
        //        SecurityAlgorithms.HmacSha256Signature));
        //    return new JwtSecurityTokenHandler().WriteToken(jwt);
        //}

    }

    //public class Credential
    //{
    //    public string UserName { get; set; }
    //    public string Password { get; set; }
    //}

}

