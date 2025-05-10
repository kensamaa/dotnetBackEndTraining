using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;
    public StudentsController(IMediator mediator, ILogger<StudentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var student = await _mediator.Send(new GetStudentByIdQuery(id));
        return student is null ? NotFound() : Ok(student);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _mediator.Send(new GetAllStudentsQuery());
        return Ok(list);
    }
}