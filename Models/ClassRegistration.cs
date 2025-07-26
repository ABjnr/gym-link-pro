using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymLinkPro.Models
{
    public class ClassRegistration
    {
        [Key]
        public int ClassRegistrationId { get; set; }

        public int MemberId { get; set; }

        public int ClassId { get; set; }

        public string Status { get; set; }

        public DateTime RegistrationDate { get; set; }
    }

    public class ClassRegistrationDto
    {
        public int ClassRegistrationId { get; set; }
        public int MemberId { get; set; }
        public int ClassId { get; set; }
        public string Status { get; set; }
        public DateTime RegistrationDate { get; set; }
    }

    public class ClassRegistrationDetailsViewModel
    {
        public ClassRegistrationDto ClassRegistration { get; set; }
    }
}
