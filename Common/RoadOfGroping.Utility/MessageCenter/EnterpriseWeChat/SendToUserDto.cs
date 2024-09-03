namespace RoadOfGroping.Utility.MessageCenter.EnterpriseWeChat
{
    public class SendToUserDto
    {
        public string touser { get; set; }
        public string msgtype { get; set; }
        public long agentid { get; set; }
        public SendToUserContent text { get; set; }
        public int safe { get; set; }

    }

    public class SendToUserContent
    {
        public string content { get; set; }
    }
}
