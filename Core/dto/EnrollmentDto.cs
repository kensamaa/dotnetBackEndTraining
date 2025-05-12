using System;

namespace Core.dto;

public class EnrollmentDto
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime EnrolledOn { get; set; }
}
