using MindMapper.WebApi.Models;

namespace MindMapper.WebApi.Services.Interfaces;

public interface IGptService
{
    public Task<string> QueryPrompt(string prompt, CancellationToken cancellationToken);

    public Task<string> QueryInstruction(string instruction, string prompt, CancellationToken cancellationToken);

    public Task<IReadOnlyCollection<TopicModel>> QueryTopics(string prompt, CancellationToken token);
    
    public Task<IReadOnlyCollection<TopicModel>> EnrichTopics(string prompt, IReadOnlyCollection<TopicModel> topics, CancellationToken cancellationToken);

    public Task<string> EnrichTopic(string prompt, string topicName, CancellationToken cancellationToken);
}