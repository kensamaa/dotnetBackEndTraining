namespace Core.Entities;
public class Department
{
    public int Id { get; private set; }
    public string Name { get; set; } = string.Empty;
    public virtual List<Course> Courses { get; set; } = new();
}