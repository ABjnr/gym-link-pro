using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymLinkPro.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        public int CreatorId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        // Navigation property
        [ForeignKey("CreatorId")]
        public virtual User Creator { get; set; }
    }

    public class ProjectDto
    {
        public int ProjectId { get; set; }
        public int Creator { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ProjectDetailsViewModel
    {
        public ProjectDto Project { get; set; }
    }
}
