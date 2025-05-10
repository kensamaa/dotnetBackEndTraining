using Core.Entities;
using MediatR;

public record GetAllStudentsQuery() : IRequest<IEnumerable<Student>>;