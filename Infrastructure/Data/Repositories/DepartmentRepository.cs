using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Department> GetAll(bool asNoTracking = true)
            => asNoTracking ? _context.Departments.AsNoTracking() : _context.Departments;

        public async Task<Department?> GetByIdAsync(int id, bool asNoTracking = true, CancellationToken ct = default)
        {
            var query = asNoTracking ? _context.Departments.AsNoTracking() : _context.Departments;
            return await query.FirstOrDefaultAsync(d => d.Id == id, ct);
        }

        public async Task AddAsync(Department department, CancellationToken ct = default)
            => await _context.Departments.AddAsync(department, ct);

        public void Update(Department department)
            => _context.Departments.Update(department);

        public void Remove(Department department)
            => _context.Departments.Remove(department);

   
}