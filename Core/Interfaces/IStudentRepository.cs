using Core.Entities;

namespace Core.Interfaces;
public interface IStudentRepository : IDisposable
// IDisposableprovide mechanism to release  unmanaged resources
{
    // Using IQueryable for EF performance and AsNoTracking
    IQueryable<Student> GetAll();
    Task<Student?> GetByIdAsync(Guid id);
    Task AddAsync(Student student);
    void Update(Student student);
    void Remove(Student student);

    // Save changes returns number of state entries written to the database
    Task<int> SaveChangesAsync();
}