using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GymLinkPro.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        public int CreatorId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        // Navigation property
        [ForeignKey("CreatorId")]
        [ValidateNever] // This ensures the navigation property is never validated or bound
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
