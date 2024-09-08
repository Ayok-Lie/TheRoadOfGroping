using AutoMapper;
using RoadOfGroping.Application.Service.Dtos;
using RoadOfGroping.Core.Users.Entity;

namespace RoadOfGroping.Application.Service.Mappers
{
    public class AutoMappers
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<RoadOfGropingUsers, UserDto>().ReverseMap();
        }
    }
}