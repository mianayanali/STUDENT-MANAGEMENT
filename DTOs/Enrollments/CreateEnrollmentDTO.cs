using System.ComponentModel.DataAnnotations;

namespace STUDENT_MANAGEMENT.DTOs.Enrollments
{
    public class CreateEnrollmentDTO
    {
        [Required(ErrorMessage = "StudentId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid StudentId.")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "CourseId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid CourseId.")]
        public int CourseId { get; set; }
    }
}