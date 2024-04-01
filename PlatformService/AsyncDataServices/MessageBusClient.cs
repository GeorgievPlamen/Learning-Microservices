using System.Text;
using System.Text.Json;
using PlatformService.DTOs;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection? _connection;
    private IModel? _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"]!)
        };
        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown!;
            System.Console.WriteLine("Connected to Msg Buss");
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Cant connect to rabbitmq");
            System.Console.WriteLine(ex);
        }
    }
    public void PublishNewPlatform(PlatformPublishedDTO platformPublishedDTO)
    {
        var message = JsonSerializer.Serialize(platformPublishedDTO);

        if (_connection!.IsOpen)
        {
            System.Console.WriteLine("RabbitMq Connection Open, sending msg...");
            SendMessage(message);
        }
        else
        {
            System.Console.WriteLine("RabbitMq Connection closed, not sending msg");
        }
    }

    private void SendMessage(string msg)
    {
        var body = Encoding.UTF8.GetBytes(msg);

        _channel.BasicPublish(
            exchange: "trigger",
            routingKey: "",
            basicProperties: null,
            body: body);

        System.Console.WriteLine("We have sent msg " + msg);
    }

    public void Dispose()
    {
        System.Console.WriteLine("Bus disposed");
        if (_channel!.IsOpen)
        {
            _channel.Close();
            _connection!.Close();
        }
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        System.Console.WriteLine("Connection shut down");
    }
}