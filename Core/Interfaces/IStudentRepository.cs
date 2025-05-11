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
}