using CVPdfBot.Domain.Entities;

namespace CVPdfBot.Domain.Interfaces
{
    public interface IConversationStateRepository
    {
        Task SaveAsync(ConversationState state);
        Task<ConversationState?> GetByChatIdAsync(long chatId);
        Task DeleteAsync(long chatId);
        Task<List<ConversationState>> GetAllAsync();
    }
}
