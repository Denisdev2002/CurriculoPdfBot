namespace CVPdfBot.Domain.Entities
{
    /// <summary>
    /// Representa o estado da conversa de um usuário no bot de geração de currículos.
    /// </summary>
    public class ConversationState
    {
        /// <summary>
        /// Identificador único do chat (chatId do Telegram).
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        /// Nome completo do usuário.
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Nacionalidade do usuário.
        /// </summary>
        public string? Nationality { get; set; }

        /// <summary>
        /// Estado civil do usuário.
        /// </summary>
        public string? MaritalStatus { get; set; }

        /// <summary>
        /// Idade do usuário.
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Data de nascimento do usuário.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Endereço completo do usuário.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Cidade do usuário.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Estado (UF) do usuário.
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Número de telefone para contato.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Endereço de e-mail do usuário.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Data e hora da última interação do usuário com o bot.
        /// </summary>
        public DateTime LastInteraction { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Objetivo profissional informado pelo usuário.
        /// </summary>
        public string? ProfessionalObjective { get; set; }

        /// <summary>
        /// Nível de escolaridade do usuário.
        /// </summary>
        public string? EducationLevel { get; set; }

        /// <summary>
        /// Ano de conclusão da formação acadêmica.
        /// </summary>
        public int? GraduationYear { get; set; }

        /// <summary>
        /// Lista de experiências profissionais anteriores.
        /// </summary>
        public List<WorkExperience> WorkExperiences { get; set; } = new();

        /// <summary>
        /// Lista de habilidades informadas.
        /// </summary>
        public List<string> Skills { get; set; } = new();

        /// <summary>
        /// Informações adicionais fornecidas pelo usuário.
        /// </summary>
        public List<string> AdditionalInfo { get; set; } = new();

        /// <summary>
        /// Template selecionado para gerar o currículo.
        /// </summary>
        public string? Template { get; set; }

        /// <summary>
        /// Etapa atual da conversa no fluxo do bot.
        /// </summary>
        public int Step { get; set; } = 0;
    }
}