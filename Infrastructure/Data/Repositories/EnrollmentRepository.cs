using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly AppDbContext _context;

    public EnrollmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Enrollment> GetAll(bool asNoTracking = true)
        => asNoTracking ? _context.Enrollments.AsNoTracking() : _context.Enrollments;

    public IQueryable<Enrollment> GetByStudent(Guid studentId, bool asNoTracking = true)
    {
        var query = asNoTracking ? _context.Enrollments.AsNoTracking() : _context.Enrollments;
        return query.Where(e => e.StudentId == studentId);
    }

    public IQueryable<Enrollment> GetByCourse(int courseId, bool asNoTracking = true)
    {
        var query = asNoTracking ? _context.Enrollments.AsNoTracking() : _context.Enrollments;
        return query.Where(e => e.CourseId == courseId);
    }

    public async Task AddAsync(Enrollment enrollment, CancellationToken ct = default)
        => await _context.Enrollments.AddAsync(enrollment, ct);

    public void Remove(Enrollment enrollment)
        => _context.Enrollments.Remove(enrollment);

   
}