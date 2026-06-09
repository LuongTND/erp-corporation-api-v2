using API.Base;
using Application.DTOs.Departments;
using Application.Interfaces.Services.Departments;

namespace API.Controllers.Departments;

public class DepartmentsController : CrudApiController<IDepartmentService, DepartmentDto, CreateDepartmentRequest, UpdateDepartmentRequest>
{
    public DepartmentsController(IDepartmentService departmentService)
        : base(departmentService, new CrudPermissions
        {
            Read = "hrm.department.read",
            Create = "hrm.department.create",
            Update = "hrm.department.update",
            Delete = "hrm.department.delete"
        })
    {
    }
}
