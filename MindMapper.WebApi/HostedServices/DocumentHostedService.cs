using Microsoft.EntityFrameworkCore;
using MindMapper.WebApi.Data;
using MindMapper.WebApi.Data.Entities;
using MindMapper.WebApi.Services.Interfaces;

namespace MindMapper.WebApi.HostedServices;

public sealed class DocumentHostedService : IHostedService,  IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DocumentHostedService> _logger;
    
    private CancellationTokenSource _cts = new();

    public DocumentHostedService(IServiceProvider provider, ILogger<DocumentHostedService> logger)
    {
        _serviceProvider = provider;
        _logger = logger;
    }

    private async Task Run()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var gptService = scope.ServiceProvider.GetRequiredService<IGptService>();
        var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
        
        while (!_cts.IsCancellationRequested)
        {
            var notReadyDocuments = await context
                .Documents
                .Include(x => x.Topics)
                .Where(x => x.Topics.All(y => !y.AnalysisCompleted))
                .ToArrayAsync();

            if (notReadyDocuments.Length != 0)
            {
                _logger.LogInformation("Обнаружены необработанные документы");
            }

            foreach (var document in notReadyDocuments)
            {
                if (_cts.IsCancellationRequested)
                {
                    break;
                }
                var pdfContents = await fileService.GetFileContents(document.Id, _cts.Token);
                if (pdfContents is null)
                {
                    continue;
                }
                
                var topics = (await gptService.QueryTopics(pdfContents, _cts.Token)).ToArray();
                _logger.LogInformation("Сделан запрос на парсинг тем");
                var enrichedTopics = (await gptService.EnrichTopics(pdfContents, topics, _cts.Token)).ToArray();
                _logger.LogInformation("Сделан запрос на насыщение тем");
                
                var dbTopics = enrichedTopics.Select(topic => new Topic
                {
                    Name = topic.Name,
                    DocumentId = document.Id,
                    PreviousTopicId = null,
                    AnalysisCompleted = false,
                    Text = topic.Text
                })
                .ToArray();
                
                await context.Topics.AddRangeAsync(dbTopics);
                await context.SaveChangesAsync();
                
                for (var i = 0; i < dbTopics.Length; i++)
                {
                    var topic = dbTopics[i];
                    var inEnrichedPreviousTopicId = enrichedTopics[i].PreviousTopicId;
                    if (inEnrichedPreviousTopicId is not null)
                    {
                        topic.PreviousTopicId = topics[inEnrichedPreviousTopicId.Value].Id;
                    }

                    topic.AnalysisCompleted = true;
                    context.Topics.Update(topic);
                }

                await context.SaveChangesAsync();
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _ = Run();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _cts.Dispose();
    }
}