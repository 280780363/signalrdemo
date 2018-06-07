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
    public class MsgHandler : IDisposable
    {
        public MsgHandler(IConfiguration configuration, IHubContext<MessageHub> hubContext)
        {
            factory = new ConnectionFactory();
            factory.HostName = configuration.GetValue<string>("RabbitMQ:Host");
            factory.UserName = configuration.GetValue<string>("RabbitMQ:User");
            factory.Password = configuration.GetValue<string>("RabbitMQ:Password");
            this.hubContext = hubContext;
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

        }
        ConnectionFactory factory;
        IHubContext<MessageHub> hubContext;
        IConnection connection;
        IModel channel;
        public void BeginHandleMsg()
        {
            channel.QueueDeclare("chat_queue", false, false, false, null);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume("chat_queue", false, consumer);
            consumer.Received += (model, arg) =>
            {
                var body = arg.Body;
                var message = Encoding.UTF8.GetString(body);
                var msg = JsonConvert.DeserializeObject<MsgDto>(message);

                hubContext.Clients?.Client(msg.ToUser.ConnectionId)
                                  ?.SendAsync("Receive", DateTime.Now, msg);

                channel.BasicAck(arg.DeliveryTag, false);
            };
        }

        public void Dispose()
        {
            channel?.Dispose();
            connection?.Dispose();
        }
    }
}
