using Core.Entities;
using Core.Interfaces;
using MediatR;

public class StudentHandlers : IRequestHandler<CreateStudentCommand, Guid>,
    IRequestHandler<GetStudentByIdQuery, Student?>,
    IRequestHandler<GetAllStudentsQuery, IEnumerable<Student>>
{
    private readonly IStudentRepository _repo;
    public StudentHandlers(IStudentRepository repo) => _repo = repo;
    public async Task<Guid> Handle(CreateStudentCommand cmd, CancellationToken ct)
    {
        var student = new Student(cmd.FirstName, cmd.LastName, cmd.EnrollmentDate);
        await _repo.AddAsync(student);
        await _repo.SaveChangesAsync();
        return student.Id;
    }

    public async Task<Student?> Handle(GetStudentByIdQuery q, CancellationToken ct)
        => await _repo.GetByIdAsync(q.Id);

    public async Task<IEnumerable<Student>> Handle(GetAllStudentsQuery q, CancellationToken ct)
        => _repo.GetAll().ToList();
}