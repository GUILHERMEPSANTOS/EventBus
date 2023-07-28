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

Olhando para esses arquivos e diretórios desse projeto, a princípio pode parecer estranho, mas existe sim um ponto de partida: o arquivo IEventBus.cs.
[Texto do link](
./src/EventBus/EventBus/Abstractions/IEventBus.cs)

