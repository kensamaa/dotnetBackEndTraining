using Core.Entities;
using Core.Interfaces;
using MediatR;
namespace Service.Students.Queries;
public record GetStudentByIdQuery(Guid Id) : IRequest<Student?>;

public class GetStudentByIdHandler : IRequestHandler<GetStudentByIdQuery, Student?>
{
    private readonly IStudentRepository _repo;

    public GetStudentByIdHandler(IStudentRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public async Task<Student?> Handle(GetStudentByIdQuery q, CancellationToken ct)
    {
        if (q == null)
            throw new ArgumentNullException(nameof(q));

        if (q.Id == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty.", nameof(q.Id));

        try
        {
            return await _repo.GetByIdAsync(q.Id, true, ct);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to retrieve student with ID {q.Id}.", ex);
        }
    }
}