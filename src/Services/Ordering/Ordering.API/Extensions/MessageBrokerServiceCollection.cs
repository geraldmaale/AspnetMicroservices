using System.Reflection;
using EventBus.Messages.Commons;
using MassTransit;
using Ordering.API.EventBusConsumers;
using RabbitMQ.Client;

namespace Ordering.API.Extensions;
public static class MessageBrokerServiceCollection
{
    public static void AddMassTransitServices(this WebApplicationBuilder builder)
    {
        // RabbitMQ Config
        builder.Services.AddMassTransit(config =>
        {
            var hostName = builder.Configuration["RabbitMq:HostName"];
            var userName = builder.Configuration["RabbitMq:UserName"];
            var password = builder.Configuration["RabbitMq:Password"];
            var port = builder.Configuration.GetValue<ushort>("RabbitMq:Port");

            var host = $"amqp://{userName}:{password}@{hostName}:{port}/";
            var host2 = $"rabbitmq://{hostName}:{port}";
            
            var entryAssembly = Assembly.GetEntryAssembly();
            config.AddConsumers(entryAssembly);
            
            config.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(host2, configurator =>
                {
                    configurator.Username(userName);
                    configurator.Password(password);
                });
                cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, e =>
                {
                    e.ConfigureConsumer<BasketCheckoutConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            }));
        });
    }

    public static void AddMassTransitWithConsumerServices(this WebApplicationBuilder builder)
    {
        // MassTransit Config
        builder.Services.AddMassTransit(config =>
        {
            //config.AddConsumer<BasketCheckoutConsumer>();
            //config.SetKebabCaseEndpointNameFormatter();

            var entryAssembly = Assembly.GetEntryAssembly();
            config.AddConsumers(entryAssembly);

            var hostName = builder.Configuration["RabbitMq:HostName"];
            var userName = builder.Configuration["RabbitMq:UserName"];
            var password = builder.Configuration["RabbitMq:Password"];
            var port = builder.Configuration.GetValue<ushort>("RabbitMq:Port");
            
            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host:hostName, port:port,"/" , hostConfig => {
                    hostConfig.Username(userName);
                    hostConfig.Password(password);
                });

                cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, e =>
                {
                    e.ConfigureConsumer<BasketCheckoutConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
