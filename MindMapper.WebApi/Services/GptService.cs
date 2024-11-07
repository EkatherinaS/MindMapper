using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using MindMapper.WebApi.Models;
using MindMapper.WebApi.Options;
using MindMapper.WebApi.Services.Interfaces;

namespace MindMapper.WebApi.Services;

public sealed class GptService : IGptService
{
    private readonly IOptions<YandexGptOptions> _yandexGptOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private string ModelUrl => $"gpt://{_yandexGptOptions.Value.Folder}/yandexgpt/latest";
    private string QueryUrl => "https://llm.api.cloud.yandex.net/foundationModels/v1/completion";

    public GptService(IOptions<YandexGptOptions> yandexGptOptions, IHttpClientFactory httpClientFactory)
    {
        _yandexGptOptions = yandexGptOptions;
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<string> QueryPrompt(string prompt, CancellationToken cancellationToken)
    {
        var content = new
        {
            modelUri = ModelUrl,
            completionOptions = new
            {
                stream = false,
                temperature = _yandexGptOptions.Value.Temperature,
                maxTokens = 100000,
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
            RequestUri = new Uri(QueryUrl),
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

    public async Task<string> QueryInstruction(string instruction, string prompt, CancellationToken cancellationToken)
    {
        var content = new
        {
            modelUri = ModelUrl,
            completionOptions = new
            {
                stream = false,
                temperature = _yandexGptOptions.Value.Temperature,
                maxTokens = 100000,
            },
            messages = new[]
            {
                new
                {
                    role = "user",
                    text = prompt,
                },
                new
                {
                    role = "system",
                    text = instruction,
                }
            }
        };

        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(QueryUrl),
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

    public async Task<IReadOnlyCollection<TopicModel>> QueryTopics(string prompt, CancellationToken token)
    {
        const string instruction = "Тебе нужно выделить основные темы из данного текста. " +
                                   "Каждое название темы должно быть максимально лаконичным и не превышать 4 слов. " +
                                   "Количество тем - от 10 до 20 штук.  Каждую тему выводи с новой строки." +
                                   " Не используй нумерацию, знаки препинания, спецсимволы. Используй только символ новой строки." +
                                   " Не используй нумерацию ни прикаких условиях. Не нумеруй строки. " +
                                   "Не пиши цифры в начале строки. Ничего кроме этого писать не нужно.";
        
        var regex = new Regex("[^а-яА-Яё ]");
        var result = await QueryInstruction(instruction, prompt, token);

        var lines = result
            .Split('\n')
            .Select(x => regex.Replace(x, string.Empty))
            .ToArray();

        const double rootProbability = 0.3;

        var list = new List<TopicModel> { new(0, lines[0], string.Empty, null) };
        
        for (var i = 1; i < lines.Length; i++)
        {
            var isRoot = Random.Shared.NextSingle() < rootProbability;
            if (isRoot)
            {
                list.Add(new TopicModel(i, lines[i], string.Empty, null));
            }
            else
            {
                var randomId = Random.Shared.Next(0, list.Count);
                list.Add(new TopicModel(i, lines[i], string.Empty, randomId));
            }
        }

        return list;
    }

    public async Task<IReadOnlyCollection<TopicModel>> EnrichTopics(string prompt, IReadOnlyCollection<TopicModel> topics, CancellationToken cancellationToken)
    {
        var tasks = topics.Select(x => EnrichTopic(prompt, x.Name, cancellationToken));
        var results = await Task.WhenAll(tasks);
        return topics.Select((x, i) => x with { Text = results[i] }).ToArray();
    }

    public async Task<string> EnrichTopic(string prompt, string topicName, CancellationToken cancellationToken)
    {
        var instruction = $"Выдели из данного текста основную мысль на тему \"{topicName}\". Ответ оформи в формате Markdown. Не выводи дополнительную информацию, только выжимку из текста.";
        return await QueryInstruction(instruction, prompt, cancellationToken);
    }
}