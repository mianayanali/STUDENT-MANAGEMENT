using System.ComponentModel.DataAnnotations;

namespace STUDENT_MANAGEMENT.DTOs.Enrollments
{
    public class UpdateEnrollmentDTO
    {
        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = string.Empty;
    }
}

