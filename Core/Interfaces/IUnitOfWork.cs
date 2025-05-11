using Core.Interfaces;

public interface IUnitOfWork 
{
    IStudentRepository Students { get; }
    IDepartmentRepository Departments { get; }
    ICourseRepository Courses { get; }
    IEnrollmentRepository Enrollments { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}