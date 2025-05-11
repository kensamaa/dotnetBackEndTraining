
using Core.Entities;

namespace Core.Interfaces;

public interface IDepartmentRepository 
{
    IQueryable<Department> GetAll(bool asNoTracking = true);
    Task<Department?> GetByIdAsync(int id, bool asNoTracking = true, CancellationToken ct = default);
    Task AddAsync(Department department, CancellationToken ct = default);
    void Update(Department department);
    void Remove(Department department);
}