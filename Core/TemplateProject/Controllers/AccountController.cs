using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TemplateProject.Api.Models;

namespace TemplateProject.Api.Controllers
{
    [Route("accounts")]
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpPost("")]
        public ActionResult Create([FromBody] RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = _userManager.CreateAsync(new IdentityUser
                        {
                            UserName = model.UserName,
                            Email = model.Email
                        },
                        model.Password)
                    .Result;

                if (result.Succeeded)
                    return Ok(result);

                string error = result.Errors.Select(t => t.Description)
                    .Aggregate<string, string>(null, (current, s) => current + s + "<br />");
                return BadRequest(error);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("logout")]
        public async Task<ActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Accepted();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager?.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        [HttpGet("")]
        public ActionResult Get()
        {
            return Ok((from c in User.Claims select new {c.Type, c.Value}).ToList());
        }
    }
}