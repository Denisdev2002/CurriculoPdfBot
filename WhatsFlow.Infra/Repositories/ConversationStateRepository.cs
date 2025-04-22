using CVPdfBot.Domain.Entities;
using CVPdfBot.Domain.Interfaces;

namespace CVPdfBot.Infra.Repositories
{
    public class ConversationStateRepository : IConversationStateRepository
    {
        private readonly Dictionary<long, ConversationState> _storage = new();

        public Task SaveAsync(ConversationState state)
        {
            _storage[state.ChatId] = state;
            return Task.CompletedTask;
        }

        public Task<ConversationState?> GetByChatIdAsync(long chatId)
        {
            _storage.TryGetValue(chatId, out var state);
            return Task.FromResult(state);
        }

        public Task DeleteAsync(long chatId)
        {
            _storage.Remove(chatId);
            return Task.CompletedTask;
        }

        public Task<List<ConversationState>> GetAllAsync()
        {
            return Task.FromResult(_storage.Values.ToList());
        }
    }
}
