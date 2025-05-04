using CVPdfBot.Domain.Entities;
using Microsoft.Extensions.Hosting;

namespace CVPdfBot.Domain.Services;

public sealed class SessionCleanerService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly Dictionary<long, ConversationState> _states;
    private readonly TimeSpan _timeout = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

    public SessionCleanerService(Dictionary<long, ConversationState> states)
    {
        _states = states;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, _interval);
        Console.WriteLine("SessionCleanerService iniciado.");
        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var agora = DateTime.UtcNow;

        var expirados = _states
            .Where(p => agora - p.Value.LastInteraction > _timeout)
            .Select(p => p.Key)
            .ToList();

        foreach (var id in expirados)
        {
            _states.Remove(id);
            Console.WriteLine($"Sessão expirada e removida: ChatId={id}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("SessionCleanerService parando.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}