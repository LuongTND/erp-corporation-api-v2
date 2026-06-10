using Application.Common.Exceptions;
using Application.Common.Mapping;
using Application.Common.Models;
using Application.DTOs.JobLevels;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.JobLevels;
using Application.Interfaces.Repositories.Users;
using Application.Interfaces.Services.JobLevels;
using AutoMapper;
using FluentValidation;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations.Services.JobLevels;

public class JobLevelService : IJobLevelService
{
    private readonly IJobLevelRepository _jobLevelRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateJobLevelRequest> _createValidator;
    private readonly IValidator<UpdateJobLevelRequest> _updateValidator;
    private readonly IMapper _mapper;

    public JobLevelService(
        IJobLevelRepository jobLevelRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateJobLevelRequest> createValidator,
        IValidator<UpdateJobLevelRequest> updateValidator,
        IMapper mapper)
    {
        _jobLevelRepository = jobLevelRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mapper = mapper;
    }

    public async Task<JobLevelDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var jobLevel = await _jobLevelRepository.GetByIdAsync(id, ct);
        if (jobLevel == null)
            throw new NotFoundException("Không tìm thấy cấp bậc chức danh.");

        return _mapper.Map<JobLevelDto>(jobLevel);
    }

    public async Task<PaginatedResult<JobLevelDto>> GetPagedAsync(PaginationQuery query, CancellationToken ct = default)
    {
        var result = await _jobLevelRepository.GetQueryable()
            .AsNoTracking()
            .OrderBy(j => j.LevelOrder)
            .ThenBy(j => j.LevelName)
            .ToPaginatedResultAsync(query, ct);

        return PaginationMapper.Map<JobLevel, JobLevelDto>(result, _mapper);
    }

    public async Task<JobLevelDto> CreateAsync(CreateJobLevelRequest request, CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);

        // Check unique LevelName
        var nameUpper = request.LevelName.Trim();
        var exists = await _jobLevelRepository.ExistsByNameAsync(nameUpper, ct);
        if (exists)
            throw new ConflictException($"Tên cấp bậc chức danh '{request.LevelName}' đã tồn tại.");

        var jobLevel = JobLevel.Create(
            request.LevelName,
            request.LevelOrder,
            request.DefaultScopeType,
            request.Description,
            request.BaseSalaryMin,
            request.BaseSalaryMax
        );

        await _jobLevelRepository.AddAsync(jobLevel, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<JobLevelDto>(jobLevel);
    }

    public async Task<JobLevelDto> UpdateAsync(Guid id, UpdateJobLevelRequest request, CancellationToken ct = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);

        var jobLevel = await _jobLevelRepository.GetByIdAsync(id, ct);
        if (jobLevel == null)
            throw new NotFoundException("Không tìm thấy cấp bậc chức danh cần cập nhật.");

        // Check unique LevelName excluding current ID
        var nameUpper = request.LevelName.Trim();
        var exists = await _jobLevelRepository.ExistsByNameExcludeIdAsync(nameUpper, id, ct);
        if (exists)
            throw new ConflictException($"Tên cấp bậc chức danh '{request.LevelName}' đã tồn tại.");

        // If trying to deactivate, check if any active user is using it
        if (!request.IsActive && jobLevel.IsActive)
        {
            var isUsed = await _userRepository.HasActiveUsersWithJobLevelAsync(id, ct);
            if (isUsed)
                throw new ConflictException("Không thể vô hiệu hóa cấp bậc chức danh đang được gán cho nhân sự hoạt động.");
        }

        jobLevel.Update(
            request.LevelName,
            request.LevelOrder,
            request.DefaultScopeType,
            request.Description,
            request.BaseSalaryMin,
            request.BaseSalaryMax
        );
        jobLevel.IsActive = request.IsActive;

        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<JobLevelDto>(jobLevel);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var jobLevel = await _jobLevelRepository.GetByIdAsync(id, ct);
        if (jobLevel == null)
            throw new NotFoundException("Không tìm thấy cấp bậc chức danh.");

        // Check if any active user is using it
        var isUsed = await _userRepository.HasActiveUsersWithJobLevelAsync(id, ct);
        if (isUsed)
            throw new ConflictException("Không thể xóa cấp bậc chức danh đang được gán cho nhân sự hoạt động.");

        jobLevel.IsActive = false;

        await _unitOfWork.SaveChangesAsync(ct);
    }
}
