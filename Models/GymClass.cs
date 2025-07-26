using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymLinkPro.Models
{
    public class GymClass
    {
        [Key]
        public int GymClassId { get; set; }

        public string Name { get; set; }

        public int TrainerId { get; set; }

        public DateTime ScheduleTime { get; set; }

        public int MaxCapacity { get; set; }
    }

    public class GymClassDto
    {
        public int GymClassId { get; set; }
        public string Name { get; set; }
        public int TrainerId { get; set; }
        public DateTime ScheduleTime { get; set; }
        public int MaxCapacity { get; set; }
    }

    public class GymClassDetailsViewModel
    {
        public GymClassDto GymClass { get; set; }
    }
}
