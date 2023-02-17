using MediatR;

namespace FictiveShop.Core.Interfeces
{
    public interface IQuery<TResponse> : IRequest<TResponse>
    {
    }
}