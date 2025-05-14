using System;
using Core.Entities;
using Core.Interfaces;
using MediatR;

namespace Service.Students.Queries;
public record GetStudentEnrollmentAfterQuery(DateTime date) : IRequest<List<Student>>;
public class GetStudentEnrollmentAfterHandler : IRequestHandler<GetStudentEnrollmentAfterQuery, List<Student>>
{
    private readonly IStudentRepository _repo;
    public GetStudentEnrollmentAfterHandler(IStudentRepository r)
    {
        _repo = r;
    }
    public async Task<List<Student>> Handle(GetStudentEnrollmentAfterQuery q, CancellationToken cancellationToken)
    {
        try
        {
            return await _repo.GetStudentsEnrolledAfterAsync(q.date);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

    }
}
