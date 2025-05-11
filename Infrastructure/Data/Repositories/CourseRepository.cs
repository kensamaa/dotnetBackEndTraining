using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Course> GetAll(bool asNoTracking = true)
        => asNoTracking ? _context.Courses.AsNoTracking() : _context.Courses;

    public async Task<Course?> GetByIdAsync(int id, bool asNoTracking = true, CancellationToken ct = default)
    {
        var query = asNoTracking ? _context.Courses.AsNoTracking() : _context.Courses;
        return await query.FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task AddAsync(Course course, CancellationToken ct = default)
        => await _context.Courses.AddAsync(course, ct);

    public void Update(Course course)
        => _context.Courses.Update(course);

    public void Remove(Course course)
        => _context.Courses.Remove(course);


}