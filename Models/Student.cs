namespace STUDENT_MANAGEMENT.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = String.Empty;
        public string Phone { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
