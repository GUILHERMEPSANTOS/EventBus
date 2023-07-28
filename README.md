## :bus: Abstraction EventBus
Este é um exemplo simples de como abstrair um message broker na sua aplicação. 
     
E claro, caso esse projeto tenha ajudado você, deixe uma star :star:
     
>🚨 Nota: Feedback e interação são sempre bem-vindos! Caso encontre algum erro ou tenha sugestões, fico extremamente grato em aprender com você. Sinta-se à vontade para abrir uma issue ou entrar em contato através das redes sociais. Vamos interagir! Hehehe. 

Esse projeto foi baseado no (**eShopOnContainers**)

&nbsp;
&nbsp;

![image](https://github.com/GUILHERMEPSANTOS/EventBus/assets/89268597/097c08d4-2452-49bd-b2be-9f07698bd577)

## O que é um Event Bus?

Um Event Bus, ou barramento de eventos, é um padrão de arquitetura utilizado em aplicações distribuídas para facilitar a comunicação e o acoplamento flexível entre diferentes serviços. Ele permite que os serviços enviem e recebam eventos assíncronos para notificar sobre a ocorrência de eventos relevantes.

## Por que abstrair um Event Bus?

A abstração do Event Bus é importante, pois ela permite que os serviços não tenham conhecimento direto uns dos outros, reduzindo a dependência e possibilitando a troca fácil de tecnologias de comunicação, como RabbitMQ ou Azure Service Bus, sem afetar a lógica do negócio. Isso resulta em uma arquitetura mais escalável, resiliente e de fácil manutenção.  [Para mais informações](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/integration-event-based-microservice-communications)

## Arquitetura do Projeto

Como podem notar a implementação escolhida para exemplo foi o 🥕 RabbitMQ

![image](https://github.com/GUILHERMEPSANTOS/EventBus/assets/89268597/de6aeb0a-088e-49aa-8201-d4ae18c105b5)

## Interface EventBus

Olhando para esses arquivos e diretórios desse projeto, a princípio pode parecer estranho, mas existe sim um ponto de partida: 
- o arquivo
[IEventBus.cs](
./src/EventBus/EventBus/Abstractions/IEventBus.cs) nesse arquivo, definimos o contrato para o nosso event bus, estabelecendo as bases para o sistema de comunicação entre componentes

 ```cs
using EventBus.Events;

namespace EventBus.Abstractions
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

        void Subscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>;
    }
}
```
O event bus possui dois métodos essenciais: `Publish` e `Subscribe`. O método `Publish` é responsável por difundir o evento de integração para os microsserviços ou aplicativos externos que assinarem esse evento. É usado pelo microsserviço que está publicando o evento.

Já os métodos ``Subscribe`` são utilizados pelos microsserviços que desejam receber eventos. Esses métodos possuem dois argumentos: o primeiro é o evento de integração que desejam assinar, e o segundo é o manipulador de eventos de integração (ou método de retorno de chamada) a ser executado quando o microsserviço receptor obtiver a mensagem do evento de integração. Com isso, o event bus facilita a comunicação e sincronização de eventos entre diferentes partes da aplicação. [Para mais informações](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/integration-event-based-microservice-communications)

## IIntegrationEventHandler e classe base de eventos
A interface `IEventBus` determina que o tipo `TEvent` deve ser um `IntegrationEvent`, e a interface do Handler deve herdar de `IIntegrationEventHandler`. O Handler receberá o `TEvent`.

- [IIntegrationEventHandler.cs](./src/EventBus/EventBus/Abstractions/IIntegrationEventHandler.cs)
- [IntegrationEvent.cs](./src/EventBus/EventBus/Events/IntegrationEvent.cs)




