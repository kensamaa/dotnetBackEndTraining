using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Student> Students { get; set; } = default!;
    public DbSet<Department> Departments { get; set; } = default!;
    public DbSet<Course> Courses { get; set; } = default!;
    public DbSet<Enrollment> Enrollments { get; set; } = default!;

    public async Task<int> SaveChangesAsync()
    {
        Console.WriteLine("Saving changes...");
        return await base.SaveChangesAsync(default);
    }
    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
                => base.SaveChangesAsync(ct);
    // Dispose pattern provided by DbContext
    public override void Dispose() => base.Dispose();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enrollment>()
            .HasKey(e => new { e.StudentId, e.CourseId });

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId);
    }
}