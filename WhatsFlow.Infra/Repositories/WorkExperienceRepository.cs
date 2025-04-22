using CVPdfBot.Domain.Entities;
using CVPdfBot.Domain.Interfaces;

namespace CVPdfBot.Infra.Repositories
{
    public class WorkExperienceRepository : IWorkExperienceRepository
    {
        private readonly Dictionary<long, List<WorkExperience>> _storage = new();

        public Task AddExperienceAsync(long chatId, WorkExperience experience)
        {
            if (!_storage.ContainsKey(chatId))
                _storage[chatId] = new List<WorkExperience>();

            _storage[chatId].Add(experience);
            return Task.CompletedTask;
        }

        public Task<List<WorkExperience>> GetExperiencesAsync(long chatId)
        {
            _storage.TryGetValue(chatId, out var list);
            return Task.FromResult(list ?? new List<WorkExperience>());
        }
    }
}