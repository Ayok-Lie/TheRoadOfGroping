using System.ComponentModel;

namespace RoadOfGroping.Utility.MessageCenter
{
    /// <summary>
    /// 通知类型
    /// </summary>
    public enum PushChannelType
    {
        [Description("RobotFeishu")]
        RobotFeishu,
        [Description("RobotWechat")]
        RobotWechat,
        [Description("RobotDingding")]
        RobotDingding,
        [Description("SmsAliyun")]
        SmsAliyun,
        [Description("SmsTenxunyun")]
        SmsTenxunyun,
        [Description("SmsHuaweiyun")]
        SmsHuaweiyun,
        [Description("Email")]
        Email,
        [Description("Firebase")]
        Firebase,
    }
}
