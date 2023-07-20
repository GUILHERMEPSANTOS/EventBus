using Microsoft.AspNetCore.Mvc;
using WebAppPublisher.Domain;
using WebAppPublisher.IntegrationEvents;

namespace WebAppPublisher.Controller
{
    [ApiController]
    [Route("cliente")]
    public class ClienteController : ControllerBase
    {
        private readonly ICustomerIntegrationEventService _customerIntegrationEventService;

        public ClienteController(ICustomerIntegrationEventService customerIntegrationEventService)
        {
            _customerIntegrationEventService = customerIntegrationEventService;
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarCliente()
        {
            var customer = new CustomerCreatedIntegrationEvent("Guilherme", "guilherme@hotmail.com");

            await _customerIntegrationEventService.PublishThroughEventBusAsync(customer);

            return Ok();
        }
    }
}