using Core.Entities;
using Core.Interfaces;
using MediatR;
using Service.Students.Queries;

public class StudentHandlers : IRequestHandler<CreateStudentCommand, Guid>,
    IRequestHandler<GetAllStudentsQuery, IEnumerable<Student>>
{
    private readonly IStudentRepository _repo;
    private readonly IUnitOfWork _unitOfWork;
    public StudentHandlers(IStudentRepository repo, IUnitOfWork unitwork)
    {
        _unitOfWork = unitwork;
        _repo = repo;
    }
    public async Task<Guid> Handle(CreateStudentCommand cmd, CancellationToken ct)
    {
        var student = new Student(cmd.FirstName, cmd.LastName, cmd.EnrollmentDate);
        await _repo.AddAsync(student);
        await _unitOfWork.SaveChangesAsync();
        return student.Id;
    }
    public async Task<IEnumerable<Student>> Handle(GetAllStudentsQuery q, CancellationToken ct)
        => _repo.GetAll().ToList();
}