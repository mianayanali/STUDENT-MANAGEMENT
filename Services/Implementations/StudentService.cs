using Microsoft.EntityFrameworkCore;
using STUDENT_MANAGEMENT.Data;
using STUDENT_MANAGEMENT.DTOs.Students;
using STUDENT_MANAGEMENT.Models;
using STUDENT_MANAGEMENT.Services.Interfaces;

namespace STUDENT_MANAGEMENT.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Tamam students fetch karna
        public async Task<IEnumerable<StudentResponseDTO>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Select(s => new StudentResponseDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Age = s.Age,
                    Email = s.Email,
                    Phone = s.Phone,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();
        }

        // 2. Kisi aik student ko ID se dhoondna
        public async Task<StudentResponseDTO?> GetStudentByIdAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return null;
            }

            return new StudentResponseDTO
            {
                Id = student.Id,
                Name = student.Name,
                Age = student.Age,
                Email = student.Email,
                Phone = student.Phone,
                CreatedAt = student.CreatedAt
            };
        }

        // 3. Naya student create karna
        public async Task<StudentResponseDTO?> CreateStudentAsync(CreateStudentDTO dto)
        {
            var emailExists = await _context.Students.AnyAsync(s => s.Email == dto.Email);
            if (emailExists)
            {
                return null; // Duplicate email
            }

            var student = new Student
            {
                Name = dto.Name,
                Age = dto.Age,
                Email = dto.Email,
                Phone = dto.Phone,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            return new StudentResponseDTO
            {
                Id = student.Id,
                Name = student.Name,
                Age = student.Age,
                Email = student.Email,
                Phone = student.Phone,
                CreatedAt = student.CreatedAt
            };
        }

        // 4. Student update karna
        public async Task<bool?> UpdateStudentAsync(int id, UpdateStudentDTO dto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return false; // Not Found
            }

            var emailExists = await _context.Students
                .AnyAsync(s => s.Email == dto.Email && s.Id != id);

            if (emailExists)
            {
                return null; // Conflict
            }

            student.Name = dto.Name;
            student.Age = dto.Age;
            student.Email = dto.Email;
            student.Phone = dto.Phone;

            await _context.SaveChangesAsync();
            return true;
        }

        // 5. Student delete karna
        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return false;
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        // 6. Name se search karna
        public async Task<IEnumerable<StudentResponseDTO>> SearchByNameAsync(string name)
        {
            return await _context.Students
                .Where(s => s.Name.ToLower().Contains(name.ToLower()))
                .Select(s => new StudentResponseDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Age = s.Age,
                    Email = s.Email,
                    Phone = s.Phone,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();
        }

        // 7. Age range se filter karna
        public async Task<IEnumerable<StudentResponseDTO>> FilterByAgeAsync(int minAge, int maxAge)
        {
            return await _context.Students
                .Where(s => s.Age >= minAge && s.Age <= maxAge)
                .Select(s => new StudentResponseDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Age = s.Age,
                    Email = s.Email,
                    Phone = s.Phone,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();
        }

        // 8. Total students count karna
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Students.CountAsync();
        }
    }
}