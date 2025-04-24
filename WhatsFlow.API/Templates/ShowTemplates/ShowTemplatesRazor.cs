using CVPdfBot.Domain.Entities;
using RazorLight;
using System.Diagnostics;

namespace CVPdfBot.Domain.Templates.ShowTemplates;

public class ShowTemplatesRazor
{
    private readonly string _templateFolder = @"C:\WhatsFlowSolution\WhatsFlow.Domain\Templates";

    public async Task RenderAndOpenAllTemplatesAsync()
    {
        var engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(typeof(ShowTemplatesRazor)) // agora está na API, perfeito
            .UseMemoryCachingProvider()
            .Build();


        var templates = new[] { "moderno", "classico", "basico" };

        var exampleModel = GetSampleModel();

        foreach (var template in templates)
        {
            string templatePath = template;
            string html = await engine.CompileRenderAsync(templatePath, exampleModel);

            string outputPath = System.IO.Path.Combine(AppContext.BaseDirectory, $"{template}.html");
            await File.WriteAllTextAsync(outputPath, html);

            // Abre no navegador
            Process.Start(new ProcessStartInfo
            {
                FileName = outputPath,
                UseShellExecute = true
            });
        }
    }

    private ConversationState GetSampleModel()
    {
        return new ConversationState
        {
            FullName = "Jefferson Ricardo",
            Nationality = "Brasileiro",
            MaritalStatus = "Solteiro",
            Age = 18,
            DateOfBirth = new DateTime(2006, 7, 24),
            Address = "Rua José Godoy Alves, 372",
            City = "Vera Cruz",
            State = "SP",
            Phone = "(14) 98807-9680",
            Email = "jeffersoncardoso17482@gmail.com",
            ProfessionalObjective = "Buscar oportunidade para aplicar minhas habilidades.",
            EducationLevel = "Ensino Médio Completo",
            GraduationYear = 2024,
            WorkExperiences = new List<WorkExperience>
            {
                new WorkExperience
                {
                    Company = "Fazenda Recreio",
                    Period = "2024-2025",
                    City = "Vera Cruz",
                    State = "SP",
                    Role = "Serviços Agropecuários",
                    Responsibilities = new List<string> { "Diversas atividades agropecuárias" }
                }
            },
            Skills = new List<string> { "Trabalho em equipe", "Raciocínio lógico", "Vitalidade" },
            AdditionalInfo = new List<string> { "Proativo", "Responsável", "Adaptável" },
            Template = "Moderno"
        };
    }
    public async Task<string> RenderTemplateAsync(string templateName)
    {
        var engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(_templateFolder) // ou UseEmbeddedResourcesProject(typeof(ShowTemplatesRazor))
            .UseMemoryCachingProvider()
            .Build();

        var model = GetSampleModel();
        string templatePath = $"{templateName}.cshtml";

        return await engine.CompileRenderAsync(templatePath, model);
    }

}