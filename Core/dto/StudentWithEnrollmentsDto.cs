using System;

namespace Core.dto;

public class StudentWithEnrollmentsDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public List<EnrollmentDto> Enrollments { get; set; } = new List<EnrollmentDto>();
}
