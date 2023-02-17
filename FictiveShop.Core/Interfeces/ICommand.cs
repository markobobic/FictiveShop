using MediatR;

namespace FictiveShop.Core.Interfeces
{
    public interface ICommand : IRequest
    {
    }

    public interface ICommand<TResponse> : IRequest<TResponse>
    {
    }
}