using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spotcc.Services;
using Spotcc.Services.Models;

namespace SpotCC.Controllers
{
    [Route("api/auth/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepository<Account> _accountRepo;

        public AuthController(IRepository<Account> accountRepo)
        {
            _accountRepo = accountRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                token = token.ToLower();
                
                var account = _accountRepo.FindFirst(a => a.Enable && a.Token.ToLower() == token && a.ExpiredDate >= DateTime.UtcNow);
                if (account != null)
                {
                    account.Artists = null;
                    var claims = new List<Claim>
                        {
                          new Claim(ClaimTypes.Name, account.Name),
                          new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                          new Claim("expired_date", account.ExpiredDate.ToString(), ClaimValueTypes.DateTime)
                        };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    int expiredIn = (account.ExpiredDate - DateTime.UtcNow).Days;
                    return Ok(new { AccountInfo = account, Message = expiredIn <= 7? $"Your token will be expired in {expiredIn} days. Please purchase a new one to continue to use the software." : string.Empty });
                }
            }

            return Unauthorized();
        }
    }
}