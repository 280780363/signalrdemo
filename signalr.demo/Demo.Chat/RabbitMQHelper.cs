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
    public class RabbitMQHandler
    {
        public RabbitMQHandler(IConfiguration configuration, IHubContext<MessageHub> hubContext)
        {
            factory = new ConnectionFactory();
            factory.HostName = configuration.GetValue<string>("RabbitMQ:Host");
            factory.UserName = configuration.GetValue<string>("RabbitMQ:User");
            factory.Password = configuration.GetValue<string>("RabbitMQ:Password");
            this.hubContext = hubContext;
        }
        ConnectionFactory factory;
        IHubContext<MessageHub> hubContext;
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
        IConnection connection;
        IModel channel;
        public void BeginHandleMsg()
        {
            connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare("chat_queue", false, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume("chat_queue", false, consumer);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var msg = JsonConvert.DeserializeObject<MsgDto>(message);

                channel.BasicAck(ea.DeliveryTag, false);
            };
        }
    }
}
