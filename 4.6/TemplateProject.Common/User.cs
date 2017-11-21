using System.Collections.Generic;
using System.Security.Claims;

namespace TemplateProject.Common
{
    public class User
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int AccessFailedCount { get; set; }
        public List<Claim> Claims { get; set; } = new List<Claim>();
        public string Name { get; set; }
        public int UserType { get; set; }
    }
}