namespace RoadOfGroping.Utility.MessageCenter.EnterpriseWeChat
{
    public class GetMobileUserDto
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int Errcode { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Errmsg { get; set; }
        /// <summary>
        /// 企业微信用户id
        /// </summary>
        public string Userid { get; set; }
    }
}
