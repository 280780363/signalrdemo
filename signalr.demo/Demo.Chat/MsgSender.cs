using Demo.Chat.Dtos;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Chat
{
    public class MsgSender
    {
        public MsgSender(IConfiguration configuration)
        {
            factory = new ConnectionFactory();
            factory.HostName = configuration.GetValue<string>("RabbitMQ:Host");
            factory.UserName = configuration.GetValue<string>("RabbitMQ:User");
            factory.Password = configuration.GetValue<string>("RabbitMQ:Password");
        }
        ConnectionFactory factory;

        public void Send(MsgDto msg)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("chat_queue", false, false, false, null);//创建一个名称为hello的消息队列
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
                    channel.BasicPublish("", "chat_queue", null, body); //开始传递
                }
            }
        }
    }
}
