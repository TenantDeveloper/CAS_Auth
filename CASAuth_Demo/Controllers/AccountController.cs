using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CASAuth_Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        [AllowAnonymous]
        [Route("Login")]
        public async Task Login(string returnUrl)
        {
            var props = new AuthenticationProperties { RedirectUri = returnUrl };
            await HttpContext.ChallengeAsync("CAS", props);
        }

        [HttpGet("GreetingUser")]
        public  IActionResult GreetingUser()
        {
            return Ok("Welcome");
            
        }
        [Route("Logout")]
        public IActionResult Logout()
        {
            return SignOut();
        } 

    }
}