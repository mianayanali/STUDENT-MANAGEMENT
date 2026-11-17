namespace STUDENT_MANAGEMENT.DTOs.Courses
{
    public class CourseResponseDTO
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationInMonths { get; set; }
        public decimal Fee { get; set; }
    }
}

