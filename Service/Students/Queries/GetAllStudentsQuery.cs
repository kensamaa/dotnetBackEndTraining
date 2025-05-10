using Core.Entities;
using MediatR;

public record GetStudentByIdQuery(Guid Id) : IRequest<Student?>;