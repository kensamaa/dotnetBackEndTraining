using Core.dto;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Student> GetAll(bool asNoTracking = true)
    {
        return asNoTracking ? _context.Students.AsNoTracking() : _context.Students;
    }


    public async Task<Student?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken ct = default)
    {
        var query = asNoTracking ? _context.Students.AsNoTracking() : _context.Students;
        return await query.FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task AddAsync(Student student, CancellationToken ct = default)
        => await _context.Students.AddAsync(student, ct);

    public void Update(Student student)
        => _context.Students.Update(student);

    public void Remove(Student student)
        => _context.Students.Remove(student);
    public async Task<List<Student>> GetStudentsWithEnrollmentsAsync(CancellationToken ct = default)
    {
        return await _context.Students
            .Include(s => s.Enrollments)  // Eager load the Enrollments
            .ToListAsync(ct);
    }
    public async Task<List<Student>> GetStudentsEnrolledAfterAsync(DateTime date, CancellationToken ct = default)
    {
        return await _context.Students
            .Where(s => s.EnrollmentDate > date)  // Filter by enrollment date
            .ToListAsync(ct);
    }
    public async Task<List<StudentCourseDto>> GetStudentsWithCoursesAsync(CancellationToken ct = default)
    {
        var query = from student in _context.Students
                    join enrollment in _context.Enrollments on student.Id equals enrollment.StudentId
                    join course in _context.Courses on enrollment.CourseId equals course.Id
                    select new StudentCourseDto
                    {
                        StudentId = student.Id,
                        StudentName = student.FirstName + " " + student.LastName,
                        CourseTitle = course.Title
                    };

        return await query.ToListAsync(ct);
    }
    public async Task<List<IGrouping<int, Student>>> GetStudentsGroupedByEnrollmentYearAsync(CancellationToken ct = default)
    {
        var query = _context.Students
            .GroupBy(s => s.EnrollmentDate.Year)  // Group by year of enrollment
            .ToListAsync(ct);

        return await query;
    }
    public async Task<List<CourseEnrollmentCountDto>> GetCourseEnrollmentCountsAsync(CancellationToken ct = default)
    {
        var query = from course in _context.Courses
                    join enrollment in _context.Enrollments on course.Id equals enrollment.CourseId
                    group enrollment by course.Title into courseGroup
                    select new CourseEnrollmentCountDto
                    {
                        CourseTitle = courseGroup.Key, // Changed from CourseName to CourseTitle
                        StudentCount = courseGroup.Count()
                    };

        return await query.ToListAsync(ct);
    }

    public async Task<List<Student>> GetPagedStudentsAsync(int pageNumber, int pageSize, CancellationToken ct = default)
    {
        return await _context.Students
            .Skip((pageNumber - 1) * pageSize)  // Skip to the correct page
            .Take(pageSize)                    // Limit the results to page size
            .ToListAsync(ct);
    }
    public async Task<StudentWithEnrollmentsDto?> GetStudentWithEnrollmentsAsync(Guid studentId, CancellationToken ct = default)
    {
        var query = from student in _context.Students
                    where student.Id == studentId
                    join enrollment in _context.Enrollments on student.Id equals enrollment.StudentId
                    join course in _context.Courses on enrollment.CourseId equals course.Id
                    select new StudentWithEnrollmentsDto
                    {
                        StudentId = student.Id,
                        StudentName = student.FirstName + " " + student.LastName,
                        Enrollments = student.Enrollments.Select(e => new EnrollmentDto
                        {
                            CourseId = e.CourseId,
                            CourseTitle = e.Course.Title,
                            EnrolledOn = e.EnrolledOn
                        }).ToList()
                    };

        return await query.FirstOrDefaultAsync(ct);
    }

}