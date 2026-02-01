
using Microsoft.Extensions.DependencyInjection;

namespace Application.Mediator
{
    public class Mediator(IServiceProvider serviceProvider) : IMediator
    {
        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            dynamic handler = serviceProvider.GetRequiredService(handlerType);
            return handler.Handle((dynamic)request, cancellationToken);
        }
    }
}
