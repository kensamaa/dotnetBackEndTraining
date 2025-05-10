using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext, IStudentRepository
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Student> Students { get; set; } = null!;
    public IQueryable<Student> GetAll() => Students.AsNoTracking();
    public async Task<Student?> GetByIdAsync(Guid id) => await Students.FindAsync(id);
    public async Task AddAsync(Student student)
               => await Students.AddAsync(student);

    public void Update(Student student)
        => Students.Update(student);

    public void Remove(Student student)
        => Students.Remove(student);

    public async Task<int> SaveChangesAsync()
    {
        Console.WriteLine("Saving changes...");
        return await base.SaveChangesAsync(default);
    }

    // Dispose pattern provided by DbContext
    public void Dispose() => base.Dispose();


}