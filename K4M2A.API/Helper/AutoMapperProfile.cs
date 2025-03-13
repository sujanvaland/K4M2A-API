using AutoMapper;
using K4M2A.Entities.Model;
using K4M2A.Entities;

namespace K4M2A.API.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, SignupRequest>();
            CreateMap<SignupRequest, User>();
            CreateMap<User, ProfileModel>();
            CreateMap<ProfileModel, User>();
            CreateMap<Notification, NotificationRes>();
            CreateMap<NotificationRes, Notification > ();
            CreateMap<User, UserDetails>();
            CreateMap<UserDetails, User>();
            CreateMap<ActivityLog, ActivityModel>();
            CreateMap<ActivityModel, ActivityLog>();


        }
    }
}
