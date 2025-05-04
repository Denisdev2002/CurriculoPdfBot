using CVPdfBot.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Wkhtmltopdf.NetCore;
using CVPdfBot.Domain.Interfaces;
using RazorLight;
using RazorLight.Razor;
using DinkToPdf;

namespace CVPdfBot.API.Services;

public class TelegramBotService : ITelegramBotService
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
        string templateName = null;

        if (escolha == "moderno") templateName = "Moderno";
        else if (escolha == "classico" || escolha == "clássico") templateName = "Classico";
        else if (escolha == "basico" || escolha == "básico") templateName = "Basico";

        if (!string.IsNullOrEmpty(templateName))
        {
            state.Template = templateName;
            state.Step++;

            Console.WriteLine($"[HandleTemplateAndFinish] Template escolhido: {templateName}");

            string htmlContent = null;
            try
            {
                htmlContent = await GenerateHtmlFromRazor(state, templateName);
                Console.WriteLine("[HandleTemplateAndFinish] HTML gerado com sucesso.");
                // Console.WriteLine($"[HandleTemplateAndFinish] Conteúdo HTML: {htmlContent}"); // Descomente para logar o HTML (pode ser longo)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HandleTemplateAndFinish] Erro ao gerar HTML: {ex.Message}");
                await _bot.SendMessage(chatId, $"⚠️ Erro ao gerar o currículo. Detalhes: {ex.Message}");
                return;
            }

            string pdfPath = null;
            try
            {
                pdfPath = await GeneratePdfFromHtml(htmlContent);
                Console.WriteLine($"[HandleTemplateAndFinish] PDF gerado com sucesso em: {pdfPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HandleTemplateAndFinish] Erro ao gerar PDF: {ex.Message}");
                await _bot.SendMessage(chatId, $"⚠️ Erro ao gerar o PDF do currículo. Detalhes: {ex.Message}");
                return;
            }

            try
            {
                await SendPdfToUser(chatId, pdfPath);
                Console.WriteLine($"[HandleTemplateAndFinish] PDF enviado para o usuário.");
                _states.Remove(chatId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HandleTemplateAndFinish] Erro ao enviar o PDF: {ex.Message}");
                await _bot.SendMessage(chatId, "⚠️ Ocorreu um erro ao tentar enviar o seu currículo em PDF. Tente novamente.");
            }
        }
        else
        {
            await _bot.SendMessage(chatId, "⚠️ Opção inválida. Digite: `Moderno`, `Clássico` ou `Básico`.");
        }
    }

    private async Task<string> GenerateHtmlFromRazor(ConversationState state, string templateName)
    {
        var templatesPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Templates");
        var project = new FileSystemRazorProject(templatesPath);

        var engine = new RazorLightEngineBuilder()
            .UseProject(project)
            .UseMemoryCachingProvider()
            .Build();

        string templatePath = templateName.ToLowerInvariant() switch
        {
            "moderno" => "moderno.cshtml",
            "classico" => "classico.cshtml",
            "basico" => "basico.cshtml",
            _ => throw new ArgumentException("Template desconhecido: " + templateName)
        };

        Console.WriteLine($"[GenerateHtmlFromRazor] Tentando renderizar o template: {templatePath}");

        try
        {
            var htmlContent = await engine.CompileRenderAsync(templatePath, state);
            Console.WriteLine("[GenerateHtmlFromRazor] Template renderizado com sucesso.");
            return htmlContent;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GenerateHtmlFromRazor] Erro ao renderizar o template ({templatePath}): {ex.Message}");
            throw;
        }
    }

    private async Task<string> GeneratePdfFromHtml(string htmlContent)
    {
        var pdfPath = Path.Combine(Directory.GetCurrentDirectory(), $"curriculo_{Guid.NewGuid()}.pdf");
        Console.WriteLine($"[GeneratePdfFromHtml] Tentando gerar PDF para: {pdfPath}");

        try
        {
            var converter = new SynchronizedConverter(new PdfTools());

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Out = pdfPath
            },
                Objects = {
                new ObjectSettings() {
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    HeaderSettings = { FontSize = 9, Right = "Página [page] de [toPage]", Line = true }
                }
            }
            };

            converter.Convert(doc);
            Console.WriteLine($"[GeneratePdfFromHtml] PDF salvo com sucesso em: {pdfPath}");

            return pdfPath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GeneratePdfFromHtml] Erro ao gerar PDF: {ex.Message}");
            throw;
        }
    }


    private async Task SendPdfToUser(long chatId, string pdfPath)
    {
        Console.WriteLine($"[SendPdfToUser] Tentando enviar PDF do caminho: {pdfPath}");

        try
        {
            if (!File.Exists(pdfPath))
            {
                Console.WriteLine($"[SendPdfToUser] Arquivo PDF não encontrado em: {pdfPath}");
                await _bot.SendMessage(chatId, "⚠️ Erro: Arquivo do currículo não encontrado. Tente novamente.");
                return;
            }

            using (var stream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read))
            {
                Console.WriteLine("[SendPdfToUser] Stream do arquivo PDF aberto.");
                await _bot.SendDocument(
                    chatId: chatId,
                    document: new InputFileStream(stream, $"currículo_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"),
                    caption: "✅ Aqui está o seu currículo em PDF!"
                );
                Console.WriteLine("[SendPdfToUser] PDF enviado com sucesso para o usuário.");
            }

            try
            {
                File.Delete(pdfPath);
                Console.WriteLine($"[SendPdfToUser] Arquivo PDF deletado: {pdfPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SendPdfToUser] Erro ao deletar o arquivo PDF: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SendPdfToUser] Erro ao enviar o PDF: {ex.Message}");
            await _bot.SendMessage(chatId, "⚠️ Ocorreu um erro ao tentar enviar o seu currículo em PDF. Tente novamente.");
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