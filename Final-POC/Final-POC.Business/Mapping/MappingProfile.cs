using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Final_POC.Core.DTOs;
using Final_POC.Data.Models;

namespace Final_POC.Business.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile() 
        {
            CreateMap<User, RegisterUserDto>().ReverseMap();

            CreateMap<User, UpdateUserDto>().ReverseMap();

            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<User, LoginUserDto>().ReverseMap();

            // Create Task DTO -> Tasks (One-way mapping)
            CreateMap<CreateTaskDto, Tasks>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Todo")) // Default status
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.TaskDetail, opt => opt.Ignore());

            // Create Task DTO -> TaskDetail (One-way mapping)
            CreateMap<CreateTaskDto, TaskDetail>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TaskId, opt => opt.Ignore());

            // Update Task DTO <-> Tasks (Bidirectional mapping)
            CreateMap<UpdateTaskDto, Tasks>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.TaskDetail, opt => opt.Ignore());

            CreateMap<Tasks, UpdateTaskDto>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    src.TaskDetail != null ? src.TaskDetail.Description : null));

            // Update Task DTO <-> TaskDetail (One-way mapping - reverse not needed)
            CreateMap<UpdateTaskDto, TaskDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TaskId, opt => opt.Ignore());

            // Update Task Status DTO <-> Tasks (Bidirectional mapping)
            CreateMap<UpdateTaskStatusDto, Tasks>()
                .ForMember(dest => dest.Title, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedToId, opt => opt.Ignore())
                .ForMember(dest => dest.TaskDetail, opt => opt.Ignore());

            CreateMap<Tasks, UpdateTaskStatusDto>();

            // Tasks <-> TaskListDto (Bidirectional mapping)
            CreateMap<Tasks, TaskListDto>()
                .ForMember(dest => dest.AssignedToName, opt => opt.MapFrom(src =>
                    src.AssignedTo != null ? src.AssignedTo.Username : null))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src =>
                    src.CreatedBy != null ? src.CreatedBy.Username : null));


            // Tasks <-> TaskDetailDto (Bidirectional mapping)
            CreateMap<Tasks, TaskDetailDto>()
                .ForMember(dest => dest.AssignedToName, opt => opt.MapFrom(src =>
                    src.AssignedTo != null ? src.AssignedTo.Username : null))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src =>
                    src.CreatedBy != null ? src.CreatedBy.Username : null))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    src.TaskDetail != null ? src.TaskDetail.Description : null));

            // TaskDetailDto -> TaskDetail (Special mapping for description)
            CreateMap<TaskDetailDto, TaskDetail>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.Id));

            //CreateMap<TaskListDto, Tasks>()
            //    .ForMember(dest => dest.TaskDetail, opt => opt.Ignore())
            //    .ForMember(dest => dest.AssignedTo, opt => opt.Ignore())
            //    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            //CreateMap<TaskDetailDto, Tasks>()
            //    .ForMember(dest => dest.TaskDetail, opt => opt.Ignore())
            //    .ForMember(dest => dest.AssignedTo, opt => opt.Ignore())
            //    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
        }

    }
}
