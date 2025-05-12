using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Department;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;
        public DepartmentController(IDepartmentService departmentService)
        {
            _service = departmentService;
        }
        [HttpGet]
        public  IActionResult GetAll()
        {
            var list = _service.GetAll();
            return Ok(list);
        }
    }
}
