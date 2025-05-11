public interface IEnrollmentRepository 
{
    IQueryable<Enrollment> GetAll(bool asNoTracking = true);
    IQueryable<Enrollment> GetByStudent(Guid studentId, bool asNoTracking = true);
    IQueryable<Enrollment> GetByCourse(int courseId, bool asNoTracking = true);
    Task AddAsync(Enrollment enrollment, CancellationToken ct = default);
    void Remove(Enrollment enrollment);
}