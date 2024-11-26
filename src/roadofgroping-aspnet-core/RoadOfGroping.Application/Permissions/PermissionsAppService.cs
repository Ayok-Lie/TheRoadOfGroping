using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.Permissions
{
    public class PermissionsAppService : ApplicationService, IPermissionsAppService
    {
        public PermissionsAppService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<int> GetABC(abc abc)
        {
            return abc.A + int.Parse(abc.B) + abc.C;
        }
    }

    public class abc
    {
        [Required]
        public int A { get; set; }
        [Required]
        [MinLength(10)]
        public string B { get; set; }
        public int C { get; set; }
    }
}
