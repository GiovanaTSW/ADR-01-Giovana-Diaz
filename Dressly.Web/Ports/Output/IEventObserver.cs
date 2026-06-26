namespace Dressly.Application.Ports.Output;

public interface IEventObserver<in TEvent>
{
    Task HandleAsync(TEvent evento);
}
