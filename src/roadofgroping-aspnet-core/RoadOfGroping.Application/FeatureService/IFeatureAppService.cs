using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoadOfGroping.Application.FeatureService.Dtos;
using RoadOfGroping.Repository.DomainService;
using Always.DynamicWebAPI;

namespace RoadOfGroping.Application.FeatureService
{
    public interface IFeatureAppService : IApplicationService
    {
        Task<FeatureListDto> GetUserConfigurations();
    }
}
