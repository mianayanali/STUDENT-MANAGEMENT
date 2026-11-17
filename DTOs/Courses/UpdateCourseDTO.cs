using System.ComponentModel.DataAnnotations;

namespace STUDENT_MANAGEMENT.DTOs.Courses
{
    public class UpdateCourseDTO
    {
        [Required(ErrorMessage = "Course name is required.")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "Course name must be between 3 and 120 characters.")]
        public string CourseName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, 48, ErrorMessage = "Duration must be between 1 and 48 months.")]
        public int DurationInMonths { get; set; }

        [Required(ErrorMessage = "Fee is required.")]
        [Range(0, 1000000, ErrorMessage = "Fee must be a positive amount.")]
        public decimal Fee { get; set; }
    }
}

