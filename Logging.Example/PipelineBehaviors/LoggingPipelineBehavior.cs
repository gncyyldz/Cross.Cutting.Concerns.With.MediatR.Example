using MediatR;

namespace Logging.Example.PipelineBehaviors
{
    public class LoggingPipelineBehavior<TRequest, TResponse>(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Handling {typeof(TRequest).Name}");
            TResponse response = await next();
            logger.LogInformation($"Handled {typeof(TRequest).Name}");

            return response;
        }
    }
}