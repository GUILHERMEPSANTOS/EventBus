## :bus: Abstraction EventBus
Este é um exemplo simples de como abstrair um message broker na sua aplicação.

>🚨 Nota: Feedback e interação são sempre bem-vindos! Caso encontre algum erro ou tenha sugestões, fico extremamente grato em aprender com você. Sinta-se à vontade para abrir uma issue ou entrar em contato através das redes sociais. Vamos interagir! Hehehe.

## O que é um Event Bus?

Um Event Bus, ou barramento de eventos, é um padrão de arquitetura utilizado em aplicações distribuídas para facilitar a comunicação e o acoplamento flexível entre diferentes serviços. Ele permite que os serviços enviem e recebam eventos assíncronos para notificar sobre a ocorrência de eventos relevantes.

## Por que abstrair um Event Bus?

A abstração do Event Bus é importante, pois ela permite que os serviços não tenham conhecimento direto uns dos outros, reduzindo a dependência e possibilitando a troca fácil de tecnologias de comunicação, como RabbitMQ ou Azure Service Bus, sem afetar a lógica do negócio. Isso resulta em uma arquitetura mais escalável, resiliente e de fácil manutenção.
