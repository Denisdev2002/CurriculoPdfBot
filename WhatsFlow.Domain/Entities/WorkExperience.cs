using System;
using System.Collections.Generic;

namespace CVPdfBot.Domain.Entities
{
    /// <summary>
    /// Representa uma experiência de trabalho de um usuário.
    /// Contém informações sobre a empresa, cargo, localização, período e responsabilidades.
    /// </summary>
    public class WorkExperience
    {
        /// <summary>
        /// Nome da empresa onde o usuário trabalhou.
        /// </summary>
        public string? Company { get; set; }

        /// <summary>
        /// Cargo desempenhado pelo usuário na empresa.
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// Cidade onde a empresa está localizada.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Estado onde a empresa está localizada.
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Período de tempo em que o usuário trabalhou na empresa.
        /// </summary>
        public string? Period { get; set; }

        /// <summary>
        /// Lista de responsabilidades do usuário durante o período de trabalho.
        /// </summary>
        public List<string> Responsibilities { get; set; } = new();
    }
}