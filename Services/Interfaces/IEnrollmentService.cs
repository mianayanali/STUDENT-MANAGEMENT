using STUDENT_MANAGEMENT.DTOs.Enrollments;

namespace STUDENT_MANAGEMENT.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<EnrollmentResponseDTO>> GetAllEnrollmentsAsync();
        Task<EnrollmentResponseDTO?> GetEnrollmentByIdAsync(int id);
        Task<(EnrollmentResponseDTO? Result, string? Error)> CreateEnrollmentAsync(CreateEnrollmentDTO dto);
        Task<bool?> UpdateEnrollmentStatusAsync(int id, UpdateEnrollmentDTO dto);
        Task<bool> DeleteEnrollmentAsync(int id);
    }
}

