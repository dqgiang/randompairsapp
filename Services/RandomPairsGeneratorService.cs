using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RandomPairsApp.Models;

namespace RandomPairsApp.Services
{
    // Note: Reference example implementation of IHostedService
    // at https://gist.github.com/davidfowl/a7dd5064d9dcf35b6eae1a7953d615e3

    // The background service to generate a random pairs every 1 second
    public class RandomPairsGeneratorSerivce : IHostedService
    {
        private Task _executingTask;
        private CancellationTokenSource _cts;
        private readonly RandomPairsServiceProvider _randomPairsServiceProvider;

        public RandomPairsGeneratorSerivce(RandomPairsServiceProvider serviceProvider)
        {
            _randomPairsServiceProvider = serviceProvider;
        }

        //
        // Summary:
        //     Triggered when the application host is ready to start the service.
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executingTask = GeneratePairsAsync(_cts.Token);
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        //
        // Summary:
        //     Triggered when the application host is performing a graceful shutdown.
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
                return;

            _cts.Cancel(); // Signal the Async task to cancel
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));
            cancellationToken.ThrowIfCancellationRequested();
        }

        // Update the random pair number list every 1 second: 
        // Remove expired items, remove exceeded items, generate 1 new pair.
        protected async Task GeneratePairsAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting background task...");
            while (!cancellationToken.IsCancellationRequested)
            {
                _randomPairsServiceProvider.UpdatePairsList();
                await Task.Delay(1000);
            }
        }
    }
}