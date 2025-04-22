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

    public TelegramBotService(IConfiguration config, Dictionary<long, ConversationState> states)
    {
        _bot = new TelegramBotClient(config["TelegramBotToken"]);
        _states = states;
    }

    public async Task ProcessUpdateAsync(Update update)
    {
        if (update.Message == null || update.Message.Text == null)
            return;

        var chatId = update.Message.Chat.Id;
        var text = update.Message.Text.Trim();

        if (!_states.ContainsKey(chatId))
            _states[chatId] = new ConversationState();

        var state = _states[chatId];

        await HandleStepAsync(chatId, text, state);
    }

    private async Task HandleStepAsync(long chatId, string text, ConversationState state)
    {
        switch (state.Step)
        {
            case 0: await AskFullName(chatId); break;
            case 1: await HandleFullName(chatId, text, state); break;
            case 2: await HandleNationality(chatId, text, state); break;
            case 3: await HandleMaritalStatus(chatId, text, state); break;
            case 4: await HandleAge(chatId, text, state); break;
            case 5: await HandleBirth(chatId, text, state); break;
            case 6: await HandleAddress(chatId, text, state); break;
            case 7: await HandleCity(chatId, text, state); break;
            case 8: await HandleState(chatId, text, state); break;
            case 9: await HandlePhone(chatId, text, state); break;
            case 10: await HandleEmail(chatId, text, state); break;
            case 11: await HandleObjective(chatId, text, state); break;
            case 12: await HandleEducation(chatId, text, state); break;
            case 13: await HandleGraduationYear(chatId, text, state); break;
            case 14: await HandleExperience(chatId, text, state); break;
            case 15: await HandleSkills(chatId, text, state); break;
            case 16: await HandleAdditional(chatId, text, state); break;
            case 17: await HandleTemplateAndFinish(chatId, text, state); break;
        }
    }

    private Task AskFullName(long chatId) => _bot.SendMessage(chatId, "Olá! Qual seu nome completo?");

    private Task HandleFullName(long chatId, string text, ConversationState state)
    {
        state.FullName = text;
        state.Step++;
        return _bot.SendMessage(chatId, "Qual sua nacionalidade?", ParseMode.MarkdownV2);
    }

    private Task HandleNationality(long chatId, string text, ConversationState state)
    {
        state.Nationality = text;
        state.Step++;
        return _bot.SendMessage(chatId, "Estado civil?");
    }

    private Task HandleMaritalStatus(long chatId, string text, ConversationState state)
    {
        state.MaritalStatus = text;
        state.Step++;
        return _bot.SendMessage(chatId, "Quantos anos você tem?");
    }

    private Task HandleAge(long chatId, string text, ConversationState state)
    {
        if (int.TryParse(text, out int age))
            state.Age = age;
        state.Step++;
        return _bot.SendMessage(chatId, "Qual sua data de nascimento? (formato: dd/MM/yyyy)");
    }

    private Task HandleBirth(long chatId, string text, ConversationState state)
    {
        if (DateTime.TryParse(text, out DateTime birth))
            state.DateOfBirth = birth;
        state.Step++;
        return _bot.SendMessage(chatId, "Informe seu endereço completo:");
    }

    private Task HandleAddress(long chatId, string text, ConversationState state)
    {
        state.Address = text;
        state.Step++;
        return _bot.SendMessage(chatId, "Em qual cidade você mora?");
    }

    private Task HandleCity(long chatId, string text, ConversationState state)
    {
        state.City = text;
        state.Step++;
        return _bot.SendMessage(chatId, "Estado?");
    }

    private Task HandleState(long chatId, string text, ConversationState state)
    {
        state.State = text;
        state.Step++;
        return _bot.SendMessage(chatId, "Informe seu telefone:");
    }

    private Task HandlePhone(long chatId, string text, ConversationState state)
    {
        state.Phone = text;
        state.Step++;
        return _bot.SendMessage(chatId, "Agora, seu e-mail:");
    }

    private Task HandleEmail(long chatId, string text, ConversationState state)
    {
        state.Email = text;
        state.Step++;
        return _bot.SendMessage(chatId, "Qual seu objetivo profissional?");
    }

    private Task HandleObjective(long chatId, string text, ConversationState state)
    {
        state.ProfessionalObjective = text;
        state.Step++;
        return _bot.SendMessage(chatId, "Nível de escolaridade?");
    }

    private Task HandleEducation(long chatId, string text, ConversationState state)
    {
        state.EducationLevel = text;
        state.Step++;
        return _bot.SendMessage(chatId, "Ano de conclusão?");
    }

    private Task HandleGraduationYear(long chatId, string text, ConversationState state)
    {
        if (int.TryParse(text, out int year))
            state.GraduationYear = year;
        state.Step++;
        return _bot.SendMessage(chatId, "Me diga uma experiência profissional (ex: Fazenda Recreio - 2024-2025 - Vera Cruz):");
    }

    private Task HandleExperience(long chatId, string text, ConversationState state)
    {
        state.WorkExperiences.Add(new WorkExperience
        {
            Company = text,
            Period = "2024-2025",
            City = state.City,
            State = state.State,
            Role = "Serviços Agropecuários",
            Responsibilities = new List<string> { "Atividades agropecuárias variadas" }
        });
        state.Step++;
        return _bot.SendMessage(chatId, "Me diga algumas habilidades (separadas por ponto e vírgula):");
    }

    private Task HandleSkills(long chatId, string text, ConversationState state)
    {
        state.Skills = text.Split(';').Select(s => s.Trim()).ToList();
        state.Step++;
        return _bot.SendMessage(chatId, "Agora, informações adicionais (separadas por ponto e vírgula):");
    }

    private Task HandleAdditional(long chatId, string text, ConversationState state)
    {
        state.AdditionalInfo = text.Split(';').Select(s => s.Trim()).ToList();
        state.Step++;
        return _bot.SendMessage(chatId, "Escolha um modelo: Moderno | Clássico | Básico");
    }

    private async Task HandleTemplateAndFinish(long chatId, string text, ConversationState state)
    {
        state.Template = text;

        var json = JsonSerializer.Serialize(state, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await _bot.SendMessage(chatId, "Currículo finalizado! Aqui estão seus dados:");
        await _bot.SendMessage(chatId, $"```\n{json}\n```", ParseMode.MarkdownV2);

        _states.Remove(chatId); // Reset
    }
}
