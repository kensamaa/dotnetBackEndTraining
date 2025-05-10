public class Course
{
    public int Id { get; private set; }
    public string Title { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public virtual Department Department { get; set; } = null!; //properties virtual to enable lazy loading
    public virtual List<Enrollment> Enrollments { get; set; } = new();
}