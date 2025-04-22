
using CVPdfBot.Domain.Entities;

namespace CVPdfBot.Domain.Interfaces
{
    public interface IWorkExperienceRepository
    {
        Task AddExperienceAsync(long chatId, WorkExperience experience);
        Task<List<WorkExperience>> GetExperiencesAsync(long chatId);
    }
}