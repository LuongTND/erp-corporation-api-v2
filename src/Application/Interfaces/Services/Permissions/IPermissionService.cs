using Application.Common.Models;
using Application.DTOs.Permissions;
using Application.DTOs.Roles;
using Application.Interfaces.Services.Common;

namespace Application.Interfaces.Services.Permissions;

public interface IPermissionService : ICrudService<PermissionDto, CreatePermissionRequest, UpdatePermissionRequest>;
