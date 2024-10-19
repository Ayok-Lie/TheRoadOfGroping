using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.Token.Dtos
{
    public class RefreshTokenInput
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
