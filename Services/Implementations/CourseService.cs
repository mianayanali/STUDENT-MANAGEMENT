using Microsoft.EntityFrameworkCore;
using STUDENT_MANAGEMENT.Data;
using STUDENT_MANAGEMENT.DTOs.Courses;
using STUDENT_MANAGEMENT.Models;
using STUDENT_MANAGEMENT.Services.Interfaces;

namespace STUDENT_MANAGEMENT.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CourseResponseDTO>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Select(c => new CourseResponseDTO
                {
                    Id = c.Id,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    DurationInMonths = c.DurationInMonths,
                    Fee = c.Fee
                })
                .ToListAsync();
        }

        public async Task<CourseResponseDTO?> GetCourseByIdAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return null;

            return new CourseResponseDTO
            {
                Id = course.Id,
                CourseName = course.CourseName,
                Description = course.Description,
                DurationInMonths = course.DurationInMonths,
                Fee = course.Fee
            };
        }

        public async Task<CourseResponseDTO?> CreateCourseAsync(CreateCourseDTO dto)
        {
            var courseExists = await _context.Courses
                .AnyAsync(c => c.CourseName.ToLower() == dto.CourseName.ToLower());
            if (courseExists) return null; // Conflict

            var course = new Course
            {
                CourseName = dto.CourseName,
                Description = dto.Description,
                DurationInMonths = dto.DurationInMonths,
                Fee = dto.Fee
            };

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            return new CourseResponseDTO
            {
                Id = course.Id,
                CourseName = course.CourseName,
                Description = course.Description,
                DurationInMonths = course.DurationInMonths,
                Fee = course.Fee
            };
        }

        public async Task<bool?> UpdateCourseAsync(int id, UpdateCourseDTO dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return false; // Not Found

            var courseExists = await _context.Courses
                .AnyAsync(c => c.CourseName.ToLower() == dto.CourseName.ToLower() && c.Id != id);
            if (courseExists) return null; // Conflict

            course.CourseName = dto.CourseName;
            course.Description = dto.Description;
            course.DurationInMonths = dto.DurationInMonths;
            course.Fee = dto.Fee;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return false;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

