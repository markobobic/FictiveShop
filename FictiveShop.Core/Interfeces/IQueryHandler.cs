using MediatR;

namespace FictiveShop.Core.Interfeces
{
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
                where TQuery : IQuery<TResponse>
    {
    }
}