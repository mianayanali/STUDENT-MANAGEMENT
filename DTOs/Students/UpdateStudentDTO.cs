using System.ComponentModel.DataAnnotations;

namespace STUDENT_MANAGEMENT.DTOs.Students
{
    public class UpdateStudentDTO
    {
        [Required(ErrorMessage = "Student name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Age is required.")]
        [Range(16, 100, ErrorMessage = "Age must be between 16 and 100.")]
        public int Age { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; } = string.Empty;
    }
}
