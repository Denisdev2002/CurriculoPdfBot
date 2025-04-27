using CVPdfBot.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Text.Json;
using RazorLight;
using System.Reflection;
using TheArtOfDev.HtmlRenderer.PdfSharp;

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
    AskTemplateChoice,          
    HandleTemplateAndFinish     // Agora realmente finaliza
};

    }

    public async Task ProcessUpdateAsync(Update update)
    {
        if (update.Message == null || string.IsNullOrWhiteSpace(update.Message.Text))
            return;

        var chatId = update.Message.Chat.Id;
        var text = update.Message.Text.Trim();

        // Novo: Verifica comandos
        if (text.Equals("/start", StringComparison.OrdinalIgnoreCase) || text.Equals("/gerar", StringComparison.OrdinalIgnoreCase))
        {
            _states[chatId] = new ConversationState();
            await _bot.SendMessage(chatId, "👋 Olá! Vamos começar seu currículo. Qual seu nome completo?");
            return;
        }

        if (!_states.TryGetValue(chatId, out var state))
        {
            await _bot.SendMessage(chatId, "❗ Envie /start para iniciar a criação do currículo.");
            return;
        }

        // Atualiza a última interação:
        state.LastInteraction = DateTime.UtcNow;


        if (state.Step >= _steps.Count)
        {
            await _bot.SendMessage(chatId, "✅ Currículo finalizado. Envie /start para começar novamente.");
            return;
        }

        await _steps[state.Step].Invoke(chatId, text, state);
    }

    //nPreciso arrumar aqui, porque ele pergunta duas vezes mesmo eu respondendo
    private async Task HandleNationality(long chatId, string text, ConversationState state)
    {
        state.Nationality = text;
        state.Step++;
        await _bot.SendMessage(chatId, "🌎 Qual é a sua nacionalidade?");
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
        await _bot.SendMessage(chatId, "📎 Agora, informe as suas habillidades e conhecimentos técnicos (separadas por ponto e vírgula):");
    }


    private async Task HandleAdditional(long chatId, string text, ConversationState state)
    {
        state.Skills = text.Split(';').Select(s => s.Trim()).ToList();
        state.Step++;
        await _bot.SendMessage(chatId, "📌 Informações adicionais ? (separe por ponto e vírgula)");
    }

    private async Task HandleTemplateAndFinish(long chatId, string text, ConversationState state)
    {
        var escolha = text.Trim().ToLowerInvariant();

        if (escolha == "moderno" || escolha == "classico" || escolha == "clássico" || escolha == "básico" || escolha == "basico")
        {
            state.Template = char.ToUpper(escolha[0]) + escolha.Substring(1).ToLowerInvariant();
            state.Step++;

            // Geração do currículo em formato JSON (para visualização)
            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });

            // Envia o currículo em JSON para o usuário
            await _bot.SendMessage(chatId, "📄 Currículo finalizado! Seus dados:");
            await _bot.SendMessage(chatId, $"```\n{json}\n```", ParseMode.MarkdownV2);

            // Geração do HTML com base no template Razor
            var htmlContent = await GenerateHtmlFromRazor(state);

            // Geração do PDF com o HTML renderizado
            var pdfPath = await GeneratePdfFromHtml(htmlContent);

            // Envia o PDF gerado para o usuário
            await SendPdfToUser(chatId, pdfPath);

            // Remove o estado do usuário após o envio
            _states.Remove(chatId);
        }
        else
        {
            await _bot.SendMessage(chatId, "⚠️ Opção inválida. Digite: `Moderno`, `Clássico` ou `Básico`.");
        }
    }

    private async Task<string> GenerateHtmlFromRazor(ConversationState state)
    {
        // Inicializa o RazorLight Engine
        var engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(Assembly.GetExecutingAssembly())  // Certifica-se de que está buscando os recursos incorporados
            .UseMemoryCachingProvider()
            .Build();

        // Caminho correto do template incorporado
        var templatePath = "Templates.Curriculo.cshtml";  // O caminho deve ser com '.' ao invés de '/' para recursos incorporados

        try
        {
            // Renderiza o HTML do template
            var htmlContent = await engine.CompileRenderAsync(templatePath, state);
            return htmlContent;
        }
        catch (Exception ex)
        {
            // Log para identificar erros
            Console.WriteLine($"Erro ao gerar HTML do Razor: {ex.Message}");
            throw;
        }
    }

    private async Task<string> GeneratePdfFromHtml(string htmlContent)
    {
        // Cria o caminho completo para salvar o PDF
        var pdfPath = Path.Combine(Directory.GetCurrentDirectory(), "curriculo_" + Guid.NewGuid().ToString() + ".pdf");

        try
        {
            // Usando HtmlRenderer.PdfSharp para gerar o PDF a partir do HTML
            var pdf = PdfGenerator.GeneratePdf(htmlContent, PdfSharp.PageSize.A4);

            // Salva o PDF no disco
            pdf.Save(pdfPath);
            return pdfPath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao gerar PDF: {ex.Message}");
            throw;
        }
    }

    private async Task SendPdfToUser(long chatId, string pdfPath)
    {
        try
        {
            // Envia o PDF gerado para o usuário
            using (var stream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read))
            {
                await _bot.SendDocument(chatId, new InputFileStream(stream), "Aqui está o seu currículo em PDF.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao enviar o PDF: {ex.Message}");
            await _bot.SendMessage(chatId, "⚠️ Ocorreu um erro ao tentar enviar o seu currículo. Tente novamente.");
        }
    }

    private async Task AskTemplateChoice(long chatId, string text, ConversationState state)
    {
        state.AdditionalInfo = text.Split(';').Select(s => s.Trim()).ToList();
        state.Step++;

        await _bot.SendMessage(chatId, "📸 Olha só os modelos disponíveis para seu currículo:");

        // Envia imagens ilustrativas
        await _bot.SendPhoto(chatId, InputFile.FromUri("https://s.tmimgcdn.com/scr/1200x750/184500/layout-minimo-de-curriculo-com-barra-lateral-preta_184529-original.jpg"), "🖼️ 1️⃣ Modelo Moderno");
        await _bot.SendPhoto(chatId, InputFile.FromUri("https://www.modelos-de-curriculos.com/wp-content/uploads/2020/01/67-modelo-curriculo-portugues.jpg"), "🖼️ 2️⃣ Modelo Clássico");
        await _bot.SendPhoto(chatId, InputFile.FromUri("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTALZ702nL3l-OgdZB9kdjqANCQTWec4QEPmuvHAXY79Z6oJgk9dbBokaWsGgdanEyVIrM&usqp=CAU"), "🖼️ 3️⃣ Modelo Básico");

        await _bot.SendMessage(chatId, "✍️ Agora, digite o modelo que deseja usar: `Moderno`, `Clássico` ou `Básico`");
    }
}