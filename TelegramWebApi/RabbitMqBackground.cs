using Domain;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace TelegramWebApi;

public class RabbitMqBackground : BackgroundService
{
    private IConnection _connection;
    private IModel _model;
    private IModel model2;
    private Domain.Message message;
    private readonly ITelegramBotClient _botClient;



    public RabbitMqBackground(ITelegramBotClient botClient)
    {
        var _connectionFactory = new ConnectionFactory();
        _connection = _connectionFactory.CreateConnection();
        _model = _connection.CreateModel();
        message = new Domain.Message();
        Console.WriteLine("Waiting message");
        model2 = _connection.CreateModel();
        _botClient = botClient;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_model);

     

        var consumer2 = new EventingBasicConsumer(model2);
        model2.BasicConsume(queue: "TitleQueue", true, consumer2);
        consumer2.Received +=  (model2, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var nameTitle = JsonConvert.DeserializeObject<NameTitle>(json);
            message.ImageName = nameTitle.Name;
            message.Title = nameTitle.Title;
           
           
        };
        _model.BasicConsume(queue: "FileQueue", true, consumer);
        consumer.Received += async (model, ea) =>
        {
            message.ImageByte = ea.Body.ToArray();
            using (var fs = new MemoryStream(message.ImageByte))
            {
                await _botClient.SendDocumentAsync("33780774", new InputOnlineFile(fs, message.ImageName));

            }
        };

    }
}
