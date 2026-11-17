using STUDENT_MANAGEMENT.DTOs.Students;

namespace STUDENT_MANAGEMENT.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentResponseDTO>> GetAllStudentsAsync();
        Task<StudentResponseDTO?> GetStudentByIdAsync(int id);
        Task<StudentResponseDTO?> CreateStudentAsync(CreateStudentDTO dto);
        Task<bool?> UpdateStudentAsync(int id, UpdateStudentDTO dto);
        Task<bool> DeleteStudentAsync(int id);
        Task<IEnumerable<StudentResponseDTO>> SearchByNameAsync(string name);
        Task<IEnumerable<StudentResponseDTO>> FilterByAgeAsync(int minAge, int maxAge);
        Task<int> GetTotalCountAsync();
    }
}

