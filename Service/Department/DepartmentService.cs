using System;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Service.Department;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDepartmentRepository _repo;
    private readonly ILogger _log;

    public DepartmentService(IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository, ILogger<DepartmentService> log)
    {
        _log = log;
        _repo = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public IQueryable<Core.Entities.Department> GetAll()
    {
        try
        {
            _log.LogInformation("get all department");
            return _repo.GetAll();
        }
        catch (System.Exception ex)
        {
            _log.LogError(ex.Message);
            throw new Exception($"error getting all department  {ex.Message}");
        }
    }
}
