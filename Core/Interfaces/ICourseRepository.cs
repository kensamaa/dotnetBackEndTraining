public interface ICourseRepository
{
    IQueryable<Course> GetAll(bool asNoTracking = true);
    Task<Course?> GetByIdAsync(int id, bool asNoTracking = true, CancellationToken ct = default);
    Task AddAsync(Course course, CancellationToken ct = default);
    void Update(Course course);
    void Remove(Course course);
}