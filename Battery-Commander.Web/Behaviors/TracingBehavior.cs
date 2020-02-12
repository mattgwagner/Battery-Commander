using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BatteryCommander.Web.Behaviors
{
    internal class TracingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TRequest> logger;

        public TracingBehavior(ILogger<TRequest> logger)
        {
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var stopwatch = Stopwatch.StartNew();

            logger.LogInformation("Starting Request handler for {@request}", request);

            var response = await next();

            logger.LogInformation("Completed Request handler for {@request}, took {elapsed}", request, stopwatch.Elapsed);

            return response;
        }
    }
}