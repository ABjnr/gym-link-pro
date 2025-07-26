using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymLinkPro.Models
{
    public class ProjectLink
    {
        [Key]
        public int ProjectLinkId { get; set; }

        public int ProjectId { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }
    }

    public class ProjectLinkDto
    {
        public int ProjectLinkId { get; set; }
        public int ProjectId { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }

    public class ProjectLinkDetailsViewModel
    {
        public ProjectLinkDto ProjectLink { get; set; }
    }
}
