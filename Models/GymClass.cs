using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymLinkPro.Models
{
    public class GymClass
    {
        [Key]
        public int GymClassId { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public string? Instructor { get; set; }

        public int TrainerId { get; set; }
        public DateTime ScheduleTime { get; set; }
        public int MaxCapacity { get; set; }

        public string? ImagePath { get; set; }
    }

    public class GymClassDto
    {
        public int GymClassId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public string? Instructor { get; set; }
        public int TrainerId { get; set; }
        public DateTime ScheduleTime { get; set; }
        public int MaxCapacity { get; set; }
        public string? ImagePath { get; set; } 
    }

    public class GymClassDetailsViewModel
    {
        public required GymClassDto GymClass { get; set; }
    }
}
