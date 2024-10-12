using AutoMapper;
using RoadOfGroping.Application.Service.Dtos;
using RoadOfGroping.Core.Files.Entitys;
using RoadOfGroping.Core.Users.Entity;

namespace RoadOfGroping.Application.Service.Mappers
{
    public class AutoMappers
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Users, UserDto>().ReverseMap();
            configuration.CreateMap<FileInfos, FileInfoOutput>().ReverseMap();
        }
    }
}