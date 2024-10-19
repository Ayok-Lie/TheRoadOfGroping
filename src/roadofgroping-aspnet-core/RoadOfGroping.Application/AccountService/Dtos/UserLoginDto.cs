using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadOfGroping.Application.AccountService.Dtos
{
    public class UserLoginDto
    {

        public UserInfoOutPut UserInfoOutPut { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public TokenInfoOutput TokenInfoOutput { get; set; }
    }
}
