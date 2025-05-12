using System;

namespace Core.dto;

public class CourseEnrollmentCountDto
{
    public string CourseTitle { get; set; } = string.Empty;
    public int StudentCount { get; set; }
}
