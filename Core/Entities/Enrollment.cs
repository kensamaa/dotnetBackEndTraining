using Core.Entities;

public class Enrollment
{
    public Guid StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrolledOn { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
}