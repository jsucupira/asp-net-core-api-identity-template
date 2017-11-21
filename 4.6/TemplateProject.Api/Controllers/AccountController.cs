using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using TemplateProject.Api.Models;
using TemplateProject.Api.Provider;

namespace TemplateProject.Api.Controllers
{
    [RoutePrefix("accounts")]
    public class AccountController : ApiController
    {
        private ApplicationUserManager _userManager;

        private IAuthenticationManager AuthenticationManager
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private ApplicationUserManager UserManager
        {
            get { return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        [Route("")]
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult Create([FromBody] RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = UserManager.Create(new ApplicationUser
                    {
                        UserName = model.UserName,
                        Email = model.Email
                    },
                    model.Password);

                if (result.Succeeded)
                    return Ok(result);
                string error = result.Errors.Aggregate<string, string>(null, (current, s) => current + s + "<br />");
                return BadRequest(error);
            }
            return BadRequest(ModelState);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            ClaimsPrincipal principal = (ClaimsPrincipal) Thread.CurrentPrincipal;
            return Ok((from c in principal.Claims select new {c.Type, c.Value}).ToList());
        }
    }
}