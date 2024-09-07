namespace RoadOfGroping.Utility.MessageCenter.EnterpriseWeChat
{
    public class AccessTokenResult
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int errcode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string errmsg { get; set; }

        /// <summary>
        /// token值
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 持续时间
        /// </summary>
        public long expires_in { get; set; }
    }
}