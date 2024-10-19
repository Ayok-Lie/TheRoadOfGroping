using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadOfGroping.Application.AccountService.Dtos
{
    public class UserInfoOutPut
    {
        /// <summary>
        /// 头像
        /// </summary>
        public string? Avater { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string? NickName { get; set; }
    }
}
