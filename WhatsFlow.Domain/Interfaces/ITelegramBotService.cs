using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace CVPdfBot.Domain.Interfaces
{
    public interface ITelegramBotService
    {
        Task ProcessUpdateAsync(Update update);
    }
}
