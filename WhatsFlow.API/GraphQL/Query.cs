using CVPdfBot.Domain.Entities;
using CVPdfBot.Domain.Interfaces;

namespace CVPdfBot.API.GraphQL
{
    public class Query
    {
        public async Task<IEnumerable<ConversationState>> GetCurriculosAsync(
            [Service] IConversationStateRepository repo)
        {
            return await repo.GetAllAsync();
        }
    }
}
