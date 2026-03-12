using System.Text;
using EventProcessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection;
    private IChannel _channel;
    private string _queueName;
    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;

        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQHost"], Port = int.Parse(_configuration["RabbitMQPort"]) };
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout).GetAwaiter().GetResult();
        _queueName = _channel.QueueDeclareAsync().GetAwaiter().GetResult().QueueName;
        _channel.QueueBindAsync(queue: _queueName, exchange: "trigger", routingKey: "").GetAwaiter().GetResult();
        Console.WriteLine($"Listening on the Message Bus using {_queueName}");
        _connection.ConnectionShutdownAsync += OnConnectionShutdownAsync;
    }

    private async Task OnConnectionShutdownAsync(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("Connection shutdown");
        if (_channel.IsOpen)
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }

        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceivedAsync;
        await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs @event)
    {
        Console.WriteLine($"Event received!");
        var body = @event.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        _eventProcessor.ProcessEvent(message);
    }
}