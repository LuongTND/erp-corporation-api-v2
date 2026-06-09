using Application.DTOs.Departments;
using Application.Interfaces.Services.Common;

namespace Application.Interfaces.Services.Departments;

public interface IDepartmentService : ICrudService<DepartmentDto, CreateDepartmentRequest, UpdateDepartmentRequest>;
