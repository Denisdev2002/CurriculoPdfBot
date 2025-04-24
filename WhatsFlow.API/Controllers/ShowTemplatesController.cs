using CVPdfBot.Domain.Templates.ShowTemplates;
using Microsoft.AspNetCore.Mvc;

namespace CVPdfBot.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowTemplatesController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTemplates()
        {
            var show = new ShowTemplatesRazor();
            await show.RenderAndOpenAllTemplatesAsync();

            return Content("Templates gerados e abertos no navegador.");
        }

    [HttpGet("{templateName}")]
        public async Task<IActionResult> GetTemplateHtml(string templateName)
        {
            var show = new ShowTemplatesRazor();
            try
            {
                string html = await show.RenderTemplateAsync(templateName);
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao renderizar template '{templateName}': {ex.Message}");
            }
        }
    }
}