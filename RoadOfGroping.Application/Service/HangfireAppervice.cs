using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Hosting;
using RoadOfGroping.Repository.DomainService;

namespace RoadOfGroping.Application.Service
{
    public class HangfireAppervice : ApplicationService
    {
        public HangfireAppervice(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public void HangfireTest()
        {
            #region Hangfire延时执行作业
            BackgroundJob.Schedule<IMessageService>(x => x.SendMessage("Delayed Message"), TimeSpan.FromMinutes(5));
            #endregion
            #region Hangfire周期性作业
            RecurringJob.AddOrUpdate<IMessageService>("sendMessageJob", x => x.SendMessage("Hello Message"), "0 2 * * *");
            #endregion
        }
    }

    public interface IMessageService
    {
        void SendMessage(string message);

        void ReceivedMessage(string message);
    }

    public class MessageService : IMessageService
    {
        public void ReceivedMessage(string message)
        {
            Console.WriteLine($"接收消息{message}");
        }

        public void SendMessage(string message)
        {
            Console.WriteLine($"发送消息{message}");
        }
    }
}
