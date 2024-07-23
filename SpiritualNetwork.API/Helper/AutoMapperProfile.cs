using AutoMapper;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Helper
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
        }
    }
}
