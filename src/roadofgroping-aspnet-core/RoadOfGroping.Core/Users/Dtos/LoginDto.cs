using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadOfGroping.Core.Users.Dtos
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class LoginDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsApiLogin { get; set; }
    }
}
