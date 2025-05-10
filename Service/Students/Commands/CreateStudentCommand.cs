using MediatR;

public record CreateStudentCommand(string FirstName, string LastName, DateTime EnrollmentDate) : IRequest<Guid>;