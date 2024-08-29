using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadOfGroping.Utility.ResultResponse
{
    public abstract class ResponseBase
    {

        public bool Success { get; set; }

        public ErrorInfo Error { get; set; }

        public bool UnAuthorizedRequest { get; set; }

        public int StatusCode { get; set; }

        public object Extras { get; set; }
    }
}
