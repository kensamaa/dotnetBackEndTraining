namespace Core.Entities;

public class Student
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    // Properties
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public virtual List<Enrollment> Enrollments { get; set; } = new();
    // Default constructor for EF
    protected Student() { }

    // Factory method
    public Student(string firstName, string lastName, DateTime enrolled)
    {
        FirstName = firstName;
        LastName = lastName;
        EnrollmentDate = enrolled;
    }
}