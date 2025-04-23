using CVPdfBot.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Text.Json;

namespace CVPdfBot.Domain.Services;

public class TelegramBotService
{
    private readonly TelegramBotClient _bot;
    private readonly Dictionary<long, ConversationState> _states;
    private readonly List<Func<long, string, ConversationState, Task>> _steps;

    public TelegramBotService(IConfiguration config, Dictionary<long, ConversationState> states)
    {
        _bot = new TelegramBotClient(config["TelegramBotToken"]);
        _states = states;

        _steps = new()
        {
            HandleFullName,
            HandleNationality,
            HandleMaritalStatus,
            HandleAge,
            HandleBirth,
            HandleAddress,
            HandleCity,
            HandleState,
            HandlePhone,
            HandleEmail,
            HandleObjective,
            HandleEducation,
            HandleGraduationYear,
            HandleExperience,
            HandleSkills,
            HandleAdditional,
            HandleTemplateAndFinish
        };
    }

    public async Task ProcessUpdateAsync(Update update)
    {
        if (update.Message == null || string.IsNullOrWhiteSpace(update.Message.Text))
            return;

        var chatId = update.Message.Chat.Id;
        var text = update.Message.Text.Trim();

        if (!_states.TryGetValue(chatId, out var state))
        {
            state = new ConversationState();
            _states[chatId] = state;
        }

        if (state.Step >= _steps.Count)
        {
            await _bot.SendMessage(chatId, "✅ Currículo finalizado. Envie /start para começar novamente.");
            return;
        }

        await _steps[state.Step].Invoke(chatId, text, state);
    }

    private async Task HandleFullName(long chatId, string text, ConversationState state)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            state.FullName = text;
            state.Step++;
            await _bot.SendMessage(chatId, "📍 Qual sua nacionalidade?");
        }
        else
        {
            await _bot.SendMessage(chatId, "⚠️ Nome inválido. Por favor, digite novamente:");
        }
    }

    //Preciso arrumar aqui, porque ele pergunta duas vezes mesmo eu respondendo
    private async Task HandleNationality(long chatId, string text, ConversationState state)
    {
        state.FullName = text;
        state.Step++;
        await _bot.SendMessage(chatId, "📍 Qual sua nacionalidade?");
    }

    private async Task HandleMaritalStatus(long chatId, string text, ConversationState state)
    {
        state.Nationality = text;
        state.Step++;
        await _bot.SendMessage(chatId, "💍 Qual seu estado civil?");
    }

    private async Task HandleAge(long chatId, string text, ConversationState state)
    {
        state.MaritalStatus = text;
        state.Step++;
        await _bot.SendMessage(chatId, "🔢 Qual sua idade?");
    }

    private async Task HandleBirth(long chatId, string text, ConversationState state)
    {
        if (int.TryParse(text, out int age))
        {
            state.Age = age;
            state.Step++;
            await _bot.SendMessage(chatId, "📅 Qual sua data de nascimento? (dd/MM/yyyy)");
        }
        else
        {
            await _bot.SendMessage(chatId, "⚠️ Por favor, insira sua idade como um número.");
        }
    }

    private async Task HandleAddress(long chatId, string text, ConversationState state)
    {
        if (DateTime.TryParse(text, out DateTime birth))
        {
            state.DateOfBirth = birth;
            state.Step++;
            await _bot.SendMessage(chatId, "🏠 Informe seu endereço completo:");
        }
        else
        {
            await _bot.SendMessage(chatId, "⚠️ Data inválida. Tente no formato dd/MM/yyyy.");
        }
    }

    private async Task HandleCity(long chatId, string text, ConversationState state)
    {
        state.Address = text;
        state.Step++;
        await _bot.SendMessage(chatId, "🏙️ Qual sua cidade?");
    }

    private async Task HandleState(long chatId, string text, ConversationState state)
    {
        state.City = text;
        state.Step++;
        await _bot.SendMessage(chatId, "🗺️ Qual seu estado?");
    }

    private async Task HandlePhone(long chatId, string text, ConversationState state)
    {
        state.State = text;
        state.Step++;
        await _bot.SendMessage(chatId, "📞 Informe seu telefone:");
    }

    private async Task HandleEmail(long chatId, string text, ConversationState state)
    {
        state.Phone = text;
        state.Step++;
        await _bot.SendMessage(chatId, "📧 Agora seu e-mail:");
    }

    private async Task HandleObjective(long chatId, string text, ConversationState state)
    {
        state.Email = text;
        state.Step++;
        await _bot.SendMessage(chatId, "🎯 Qual seu objetivo profissional?");
    }

    private async Task HandleEducation(long chatId, string text, ConversationState state)
    {
        state.ProfessionalObjective = text;
        state.Step++;
        await _bot.SendMessage(chatId, "🎓 Nível de escolaridade?");
    }

    private async Task HandleGraduationYear(long chatId, string text, ConversationState state)
    {
        state.EducationLevel = text;
        state.Step++;
        await _bot.SendMessage(chatId, "📅 Ano de conclusão?");
    }

    private async Task HandleExperience(long chatId, string text, ConversationState state)
    {
        if (int.TryParse(text, out int year))
        {
            state.GraduationYear = year;
            state.Step++;
            await _bot.SendMessage(chatId, "💼 Descreva sua experiência profissional (ex: Fazenda Recreio - 2024-2025 - Vera Cruz):");
        }
        else
        {
            await _bot.SendMessage(chatId, "⚠️ Ano inválido. Por favor, insira apenas números.");
        }
    }

    private async Task HandleSkills(long chatId, string text, ConversationState state)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            state.Skills = text.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
        }

        state.Step++;
        await _bot.SendMessage(chatId, "📎 Agora, informe informações adicionais (separadas por ponto e vírgula):");
    }


    private async Task HandleAdditional(long chatId, string text, ConversationState state)
    {
        state.Skills = text.Split(';').Select(s => s.Trim()).ToList();
        state.Step++;
        await _bot.SendMessage(chatId, "📌 Informações adicionais? (separe por ponto e vírgula)");
    }

    private async Task HandleTemplateAndFinish(long chatId, string text, ConversationState state)
    {
        state.AdditionalInfo = text.Split(';').Select(s => s.Trim()).ToList();
        state.Step++;
        state.Template = "Moderno";

        var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });

        await _bot.SendMessage(chatId, "📄 Currículo finalizado! Aqui estão os dados que você forneceu:");
        await _bot.SendMessage(chatId, $"```\n{json}\n```", ParseMode.MarkdownV2);

        _states.Remove(chatId);
    }
}
