using System.ComponentModel.DataAnnotations.Schema;

namespace STUDENT_MANAGEMENT.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationInMonths { get; set; }
       
        [Column(TypeName = "decimal(10,2)")]
        public decimal Fee { get; set; }

    }
}
