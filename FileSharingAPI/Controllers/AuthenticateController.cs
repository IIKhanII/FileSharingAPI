using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FileSharingAPI.Authentication;
using FileSharingAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileSharingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private UsersContext db;
        public AuthenticateController(UsersContext context)
        {
            db = context;
        }
        
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            User user = await db.Users.FirstOrDefaultAsync(u => (u.Email == model.Email || u.Phone == model.Email) && u.PasswordHash == model.Password);
            if (user != null)
            {
                await Authenticate(model.Email);
                return Ok(new Response { Status = "Success", Message = "User login successfully!" });
            }
            return Unauthorized(new Response { Status = "Failed", Message = "Password or login entered incorrectly" });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                db.Users.Add(new User { Id=Guid.NewGuid(), GivenName=null, Surname=null, MiddleName=null, Email = model.Email, Phone=null, PasswordHash = model.Password });
                await db.SaveChangesAsync();
                await Authenticate(model.Email);      
                return Ok(new Response { Status = "Success", Message = "User created successfully!"});
            }
            else
                return Unauthorized(new Response { Status = "Failed", Message = "User exists or incorrect data entered" });
        }
        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}
