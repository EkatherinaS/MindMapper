using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using MindMapper.WebApi.Models;
using MindMapper.WebApi.Options;
using MindMapper.WebApi.Services.Interfaces;

namespace MindMapper.WebApi.Services;

public sealed class GptService : IGptService
{
    private readonly IOptions<YandexGptOptions> _yandexGptOptions;
    private readonly IHttpClientFactory _httpClientFactory;

    public GptService(IOptions<YandexGptOptions> yandexGptOptions, IHttpClientFactory httpClientFactory)
    {
        _yandexGptOptions = yandexGptOptions;
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<string> QueryPrompt(string prompt, CancellationToken cancellationToken)
    {
        var content = new
        {
            modelUri = $"gpt://{_yandexGptOptions.Value.Folder}/yandexgpt-lite",
            completionOptions = new
            {
                stream = false,
                temperature = 0.1,
                maxTokens = 1000,
            },
            messages = new[]
            {
                new
                {
                    role = "user",
                    text = prompt,
                }
            }
        };

        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://llm.api.cloud.yandex.net/foundationModels/v1/completion"),
            Headers =
            {
                { "Authorization", $"Bearer {_yandexGptOptions.Value.IamToken}" },
                { "x-folder-id", _yandexGptOptions.Value.Folder },
            },
            Content = JsonContent.Create(content),
        };
        
        using var client = _httpClientFactory.CreateClient();
        var result = await client.SendAsync(request, cancellationToken);
        if (result.IsSuccessStatusCode)
        {
            var stream = await result.Content.ReadAsStreamAsync(cancellationToken);
            var json = await JsonSerializer.DeserializeAsync<JsonNode>(stream, cancellationToken: cancellationToken);
            return json?["result"]?["alternatives"]?[0]?["message"]?["text"]?.GetValue<string>() ?? string.Empty;
        }

        throw new Exception($"GPT Error occured: {result.StatusCode}");
    }
}