using System;
using System.Linq;
using Application.DTOs.Attendances;
using AutoMapper;
using Domain.Entities.Attendances;

namespace Application.Mappings.Attendances;

public class AttendanceMappingProfile : Profile
{
    public AttendanceMappingProfile()
    {
        CreateMap<DateTime, DateTime>().ConvertUsing(src => DateTime.SpecifyKind(src, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(src => src.HasValue ? DateTime.SpecifyKind(src.Value, DateTimeKind.Utc) : null);

        CreateMap<AttendanceLocation, AttendanceLocationDto>()
            .ForMember(dest => dest.AssignedUserIds, opt => opt.MapFrom(src => src.AssignedUsers.Select(x => x.UserId).ToList()))
            .ForMember(dest => dest.AssignedDepartmentIds, opt => opt.MapFrom(src => src.AssignedDepartments.Select(x => x.DepartmentId).ToList()));

        CreateMap<AttendanceLog, AttendanceLogDto>()
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.UserEmployeeCode, opt => opt.MapFrom(src => src.User.EmployeeCode))
            .ForMember(dest => dest.AttendanceLocationName, opt => opt.MapFrom(src => src.AttendanceLocation != null ? src.AttendanceLocation.Name : null));

        CreateMap<Attendance, AttendanceDto>()
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.CheckInLocationName, opt => opt.MapFrom(src => src.CheckInLocation != null ? src.CheckInLocation.Name : null))
            .ForMember(dest => dest.CheckOutLocationName, opt => opt.MapFrom(src => src.CheckOutLocation != null ? src.CheckOutLocation.Name : null));
    }
}
