namespace CVPdfBot.Domain.Entities
{
    public class ConversationState
    {
        public long ChatId { get; set; }
        public string? FullName { get; set; }
        public string? Nationality { get; set; }
        public string? MaritalStatus { get; set; }
        public int? Age { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        // Objetivo
        public string? ProfessionalObjective { get; set; }
        // Formação
        public string? EducationLevel { get; set; }
        public int? GraduationYear { get; set; }
        // Experiência Profissional
        public List<WorkExperience> WorkExperiences { get; set; }
        // Habilidades
        public List<string> Skills { get; set; } = new();
        // Informações Adicionais
        public List<string> AdditionalInfo { get; set; } = new();
        // Template escolhido
        public string? Template { get; set; }
        // Controle do fluxo
        public int Step { get; set; } = 0;
    }
}
