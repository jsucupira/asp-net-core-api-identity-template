using System.Collections.Generic;

namespace TemplateProject.Api.Models
{
    public class PocoDto : BaseDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }

        public List<PocoDto> Products { get; set; } = new List<PocoDto>();
    }
}