using CVPdfBot.Domain.Templates.ShowTemplates;
using Microsoft.AspNetCore.Mvc;

namespace CVPdfBot.API.Controllers
{
    /// <summary>
    /// Controlador responsável por renderizar templates HTML.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ShowTemplatesController : ControllerBase
    {
        /// <summary>
        /// Método para obter o HTML de um template pelo nome.
        /// </summary>
        /// <param name="templateName">Nome do template a ser renderizado.</param>
        /// <returns>O conteúdo HTML renderizado do template.</returns>
        /// <response code="200">HTML do template renderizado com sucesso.</response>
        /// <response code="400">Erro ao renderizar o template.</response>
        [HttpGet("{templateName}")]
        public async Task<IActionResult> GetTemplateHtml(string templateName)
        {
            var show = new ShowTemplatesRazor();
            try
            {
                // Tenta renderizar o template HTML
                string html = await show.RenderTemplateAsync(templateName);
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                // Retorna um erro caso a renderização falhe
                return BadRequest($"Erro ao renderizar template '{templateName}': {ex.Message}");
            }
        }
    }
}