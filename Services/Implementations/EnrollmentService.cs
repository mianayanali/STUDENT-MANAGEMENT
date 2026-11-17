using Microsoft.EntityFrameworkCore;
using STUDENT_MANAGEMENT.Data;
using STUDENT_MANAGEMENT.DTOs.Enrollments;
using STUDENT_MANAGEMENT.Models;
using STUDENT_MANAGEMENT.Services.Interfaces;

namespace STUDENT_MANAGEMENT.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EnrollmentResponseDTO>> GetAllEnrollmentsAsync()
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Select(e => new EnrollmentResponseDTO
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentName = e.Student != null ? e.Student.Name : string.Empty,
                    CourseId = e.CourseId,
                    CourseName = e.Course != null ? e.Course.CourseName : string.Empty,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status
                })
                .ToListAsync();
        }

        public async Task<EnrollmentResponseDTO?> GetEnrollmentByIdAsync(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.Id == id)
                .Select(e => new EnrollmentResponseDTO
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentName = e.Student != null ? e.Student.Name : string.Empty,
                    CourseId = e.CourseId,
                    CourseName = e.Course != null ? e.Course.CourseName : string.Empty,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(EnrollmentResponseDTO? Result, string? Error)> CreateEnrollmentAsync(CreateEnrollmentDTO dto)
        {
            var student = await _context.Students.FindAsync(dto.StudentId);
            if (student == null)
            {
                return (null, "Invalid StudentId. Student does not exist.");
            }

            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null)
            {
                return (null, "Invalid CourseId. Course does not exist.");
            }

            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == dto.StudentId && e.CourseId == dto.CourseId);
            if (alreadyEnrolled)
            {
                return (null, "DUPLICATE");
            }

            var enrollment = new Enrollment
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                EnrollmentDate = DateTime.UtcNow,
                Status = "Active"
            };

            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();

            var response = new EnrollmentResponseDTO
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                StudentName = student.Name,
                CourseId = enrollment.CourseId,
                CourseName = course.CourseName,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status
            };

            return (response, null);
        }

        public async Task<bool?> UpdateEnrollmentStatusAsync(int id, UpdateEnrollmentDTO dto)
        {
            var allowedStatuses = new[] { "Active", "Completed", "Cancelled" };
            if (!allowedStatuses.Contains(dto.Status))
            {
                return null; // Invalid status value
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return false; // Not Found
            }

            enrollment.Status = dto.Status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return false;
            }

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

