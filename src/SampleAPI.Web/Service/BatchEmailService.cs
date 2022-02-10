namespace Kkokkino.SampleAPI.Web.Service
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  using Kkokkino.GithubApi;
  using Kkokkino.SampleAPI.Persistence;
  using Kkokkino.SampleAPI.Web.Helpers.Extensions;

  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;

  using Refit;
  /// <summary>
  /// Scoped service demonstrating use of <see cref="CancellationToken"/>
  /// </summary>
  public class BatchEmailService : BackgroundService
  {
    private readonly ILogger<BatchEmailService> logger;
    private readonly IPetStore api;
    private readonly IServiceScopeFactory scopeFactory;
    private CancellationTokenSource? tokenSource;

    public BatchEmailService(ILogger<BatchEmailService> logger, IServiceScopeFactory scopeFactory)
    {
      this.scopeFactory = scopeFactory;
      api = RestService.For<IPetStore>("https://petstore.swagger.io/v2");
      this.logger = logger;
    }

    public void Trigger() => tokenSource?.Cancel(); // Cancel a token source waking up the service - line 48

    protected override async Task ExecuteAsync(CancellationToken stoppingToken /* From Server */) => await Work(stoppingToken);

    private async Task Work(CancellationToken token)
    {
      while (!token.IsCancellationRequested) // Server is shutting down, otherwise it keeps running
      {
        try
        {
          tokenSource = new CancellationTokenSource(); // My token source
          using var syncedToken = CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token, token); // Links my and server token source

          await Task.Delay(TimeSpan.FromMinutes(5), syncedToken.Token);

          var gists = await api.GetByStatus(PetStatus.sold);

          using var scope = scopeFactory.CreateScope();
          var ctx = scope.ServiceProvider.GetRequiredService<PersistenceContext>();

          ctx.People
            .WhereIf(0 > 5, x => x.Name.StartsWith("A"));

          using (var transaction = await ctx.Database.BeginTransactionAsync(token))
          {
            // edw kanw o,ti thelw kai tha graftei ws ena transaction
            await transaction.CommitAsync(token);
          }
        }
        catch (TaskCanceledException)
        {
          // Tokens may be cancelled anytime
        }
        catch (Exception e)
        {
          logger.LogCritical(e, "Unhandled exception: {Message}", e.Message); // Must. Keep. Service. Running
        }
      }
    }
  }
}
