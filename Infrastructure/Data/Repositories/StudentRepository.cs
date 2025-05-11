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
        => asNoTracking ? _context.Students.AsNoTracking() : _context.Students;

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

    
}