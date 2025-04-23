using CVPdfBot.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace CVPdfBot.API.Controllers
{
    [Route("api/telegram")]
    [ApiController]
    public class TelegramWebhookController : ControllerBase
    {
        private readonly TelegramBotService _botService;

        public TelegramWebhookController(TelegramBotService botService)
        {
            _botService = botService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            await _botService.ProcessUpdateAsync(update);
            return Ok();
        }
    }
}