using CVPdfBot.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace CVPdfBot.API.Controllers
{
    /// <summary>
    /// Controlador responsável pelo Webhook do Telegram.
    /// Recebe as atualizações do Telegram e as processa utilizando o serviço do bot.
    /// </summary>
    [Route("api/telegram")]
    [ApiController]
    public class TelegramWebhookController : ControllerBase
    {
        private readonly TelegramBotService _botService;

        /// <summary>
        /// Construtor do controlador TelegramWebhookController.
        /// </summary>
        /// <param name="botService">Instância do serviço de bot para processar atualizações.</param>
        public TelegramWebhookController(TelegramBotService botService)
        {
            _botService = botService;
        }

        /// <summary>
        /// Processa as atualizações recebidas do Telegram.
        /// Este método é acionado via POST e processa os dados do update recebido.
        /// </summary>
        /// <param name="update">O objeto contendo as informações da atualização recebida do Telegram.</param>
        /// <returns>Retorna um status de sucesso (200 OK) após o processamento do update.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            await _botService.ProcessUpdateAsync(update);
            return Ok();
        }
    }
}