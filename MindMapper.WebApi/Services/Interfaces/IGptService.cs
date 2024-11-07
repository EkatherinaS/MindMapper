namespace MindMapper.WebApi.Services.Interfaces;

public interface IGptService
{
    public Task<string> QueryPrompt(string prompt, CancellationToken cancellationToken);
}