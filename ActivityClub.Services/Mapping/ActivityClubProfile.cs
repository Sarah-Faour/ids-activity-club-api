using ActivityClub.Contracts.DTOs.Events;
using ActivityClub.Contracts.DTOs.Guides;
using ActivityClub.Contracts.DTOs.Members;
using ActivityClub.Contracts.DTOs.Roles;
using ActivityClub.Contracts.DTOs.Users;
using ActivityClub.Data.Models;
using AutoMapper;


namespace ActivityClub.Services.Mapping
{
    public class ActivityClubProfile : Profile
    {
        public ActivityClubProfile()
        {
            // ========================
            // Members
            // ========================
            CreateMap<Member, MemberResponseDto>()
                .ForMember(d => d.ProfessionName,
                    o => o.MapFrom(s => s.Profession != null ? s.Profession.Name : null))
                .ForMember(d => d.NationalityName,
                    o => o.MapFrom(s => s.Nationality != null ? s.Nationality.Name : null));

            CreateMap<CreateMemberDto, Member>();
            CreateMap<UpdateMemberDto, Member>();

            // ========================
            // Guides
            // ========================
            CreateMap<Guide, GuideResponseDto>()
                .ForMember(d => d.ProfessionName,
                    o => o.MapFrom(s => s.Profession != null ? s.Profession.Name : null));

            CreateMap<CreateGuideDto, Guide>();
            CreateMap<UpdateGuideDto, Guide>();

            // ========================
            // Events
            // ========================
            CreateMap<Event, EventResponseDto>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.Name));

            CreateMap<CreateEventDto, Event>();
            CreateMap<UpdateEventDto, Event>();

            // ========================
            // Roles
            // ========================
            CreateMap<Role, RoleResponseDto>();

            // ========================
            // Users
            // ========================
            CreateMap<User, UserResponseDto>()
                .ForMember(d => d.GenderName,
                    o => o.MapFrom(s => s.GenderLookup != null ? s.GenderLookup.Name : string.Empty))
                .ForMember(d => d.Roles, o => o.MapFrom(s => s.Roles));

            CreateMap<CreateUserDto, User>()
                .ForMember(d => d.PasswordHash, o => o.Ignore()); // hashed in service
            CreateMap<UpdateUserDto, User>();
        }
    }
}
