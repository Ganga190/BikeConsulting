using AutoMapper;
using BikeConsulting.Api.Model.User;
using BikeConsulting.Dto.User;


namespace BikeConsulting.Api.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserModel, UserDto>();
            
        }

    }
}
