using System;
using Core.Entities;
namespace Service.Department;

public interface IDepartmentService
{
    public IQueryable<Core.Entities.Department> GetAll();
}
