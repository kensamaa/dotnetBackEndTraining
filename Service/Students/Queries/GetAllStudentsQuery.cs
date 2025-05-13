using Core.Entities;
using MediatR;

namespace Service.Students.Queries;
public record GetAllStudentsQuery() : IRequest<IEnumerable<Student?>>;