using Microsoft.AspNet.Identity.EntityFramework;
using TemplateProject.Api.Models;

namespace TemplateProject.Api.Provider
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}