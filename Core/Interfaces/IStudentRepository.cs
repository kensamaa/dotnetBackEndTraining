using Core.dto;
using Core.Entities;

namespace Core.Interfaces;
public interface IStudentRepository
// IDisposableprovide mechanism to release  unmanaged resources
{
    // Using IQueryable for EF performance and AsNoTracking
    IQueryable<Student> GetAll(bool asNoTracking = true);
    Task<Student?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken ct = default);
    Task AddAsync(Student student, CancellationToken ct = default);
    void Update(Student student);
    void Remove(Student student);
    Task<List<Student>> GetStudentsWithEnrollmentsAsync(CancellationToken ct = default);

    Task<List<Student>> GetStudentsEnrolledAfterAsync(DateTime date, CancellationToken ct = default);

    Task<List<StudentCourseDto>> GetStudentsWithCoursesAsync(CancellationToken ct = default);

    Task<List<IGrouping<int, Student>>> GetStudentsGroupedByEnrollmentYearAsync(CancellationToken ct = default);

    Task<List<CourseEnrollmentCountDto>> GetCourseEnrollmentCountsAsync(CancellationToken ct = default);

    Task<List<Student>> GetPagedStudentsAsync(int pageNumber, int pageSize, CancellationToken ct = default);

    Task<StudentWithEnrollmentsDto?> GetStudentWithEnrollmentsAsync(Guid studentId, CancellationToken ct = default);
}