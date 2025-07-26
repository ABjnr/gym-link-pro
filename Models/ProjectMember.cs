using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymLinkPro.Models
{
    public class ProjectMember
    {
        [Key]
        public int ProjectMemberId { get; set; }

        public int MemberId { get; set; }

        public int ProjectId { get; set; }
    }

    public class ProjectMemberDto
    {
        public int ProjectMemberId { get; set; }
        public int MemberId { get; set; }
        public int ProjectId { get; set; }
    }

    public class ProjectMemberDetailsViewModel
    {
        public ProjectMemberDto ProjectMember { get; set; }
    }
}
