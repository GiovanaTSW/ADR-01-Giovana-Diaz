using Dressly.Application.Ports.Output;
using Microsoft.Extensions.Logging;

namespace Dressly.Infrastructure.Notifications;

public class ConsoleNotifier<TEvent> : IEventObserver<TEvent>
{
    private readonly ILogger<ConsoleNotifier<TEvent>> _logger;

    public ConsoleNotifier(ILogger<ConsoleNotifier<TEvent>> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(TEvent evento)
    {
        _logger.LogInformation("📢 [NOTIFICACION] {Evento}", evento);
        return Task.CompletedTask;
    }
}
