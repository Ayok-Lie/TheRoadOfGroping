using System.ComponentModel.DataAnnotations.Schema;

namespace RoadOfGroping.Core.AppSettings.Entitys
{
    public class AppSetting
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        /// <summary>
        /// 应用域名
        /// </summary>
        public string domain { get; set; }

        /// <summary>
        /// 应用Key
        /// </summary>
        public string clientId { get; set; }

        /// <summary>
        /// 公钥-加密
        /// </summary>
        public string publicKey { get; set; }

        /// <summary>
        /// 私钥-解密
        /// </summary>
        public string privateKey { get; set; }
    }
}