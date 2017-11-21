using System.ComponentModel.DataAnnotations;

namespace TemplateProject.Api.Models
{
    public class RegistrationViewModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }
    }
}