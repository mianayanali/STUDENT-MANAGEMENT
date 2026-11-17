using STUDENT_MANAGEMENT.DTOs.Courses;

namespace STUDENT_MANAGEMENT.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseResponseDTO>> GetAllCoursesAsync();
        Task<CourseResponseDTO?> GetCourseByIdAsync(int id);
        Task<CourseResponseDTO?> CreateCourseAsync(CreateCourseDTO dto);
        Task<bool?> UpdateCourseAsync(int id, UpdateCourseDTO dto);
        Task<bool> DeleteCourseAsync(int id);
    }
}

