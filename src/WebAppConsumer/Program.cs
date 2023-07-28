using EventBus.Abstractions;
using EventBusRabbitMQ;
using WebAppConsumer.IntegrationEvents.EventHandling;
using WebAppConsumer.IntegrationEvents.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEventBus(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IIntegrationEventHandler<CustomerCreatedIntegrationEvent>, CustomerCreatedIntegrationEventHandler>();

var app = builder.Build();

var eventBus = app.Services.GetRequiredService<IEventBus>();

eventBus.Subscribe<CustomerCreatedIntegrationEvent, CustomerCreatedIntegrationEventHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
