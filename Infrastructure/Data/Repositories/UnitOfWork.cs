using Core.Interfaces;
using Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IStudentRepository Students { get; }
    public IDepartmentRepository Departments { get; }
    public ICourseRepository Courses { get; }
    public IEnrollmentRepository Enrollments { get; }

    public UnitOfWork(AppDbContext context, IStudentRepository studentRepository,
                    IDepartmentRepository departmentRepository, ICourseRepository courseRepository,
                    IEnrollmentRepository enrollmentRepository)
    {
        _context = context;
        Students = studentRepository;
        Departments = departmentRepository;
        Courses = courseRepository;
        Enrollments = enrollmentRepository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            int result = await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }


}