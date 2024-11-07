using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MindMapper.WebApi.Dto;
using MindMapper.WebApi.Models;
using MindMapper.WebApi.Services.Interfaces;

namespace MindMapper.WebApi.Controllers;

[ApiController]
[Route("/results")]
public partial class ResultsController
{
    private readonly ITopicsService _topicsService;
    private readonly IGptService _gptService;

    public ResultsController(ITopicsService topicsService, IGptService gptService)
    {
        _topicsService = topicsService;
        _gptService = gptService;
    }
    
    [HttpGet("/getDocumentTopics")]
    public async Task<GetDocumentTopicsResult> GetDocumentTopics([FromQuery] GetDocumentTopicsRequest request)
    {
        var result = await _topicsService.GetDocumentInfoAsync(request.Id);
        if (result is null)
        {
            return null;
        }

        return new GetDocumentTopicsResult(
            IsReady: result.IsReady,
            DocumentId: result.DocumentId,
            Topics: result
                .Topics
                .Select(x => new DocumentTopics(x.Id, x.Name, x.Text))
                .ToArray()
        );
    }

    [HttpGet("/queryGpt")]
    public async Task<string> QueryYandexGpt([FromQuery] string prompt, CancellationToken token)
    {
        return await _gptService.QueryPrompt(prompt, token);
    }

    [HttpPost("/queryInstructionGpt")]
    public async Task<string> QueryInstructionYandexGpt([FromBody] QueryInstructionGptDto dto, CancellationToken token)
    {
        var instruction = WhitespaceReplacement().Replace(dto.Instruction, " ");
        var prompt = WhitespaceReplacement().Replace(dto.Prompt, " ");
        return await _gptService.QueryInstruction(instruction, prompt, token);
    }

    [HttpPost("/queryTopics")]
    public async Task<IReadOnlyCollection<TopicModel>> QueryTopics([FromBody] QueryTopicsFromTextDto dto, CancellationToken token)
    {
        var result = await _gptService.QueryTopics(dto.Text, token);
        return result;
    }

    [HttpPost("/enrichTopic")]
    public async Task<string> GetTopicExplanation([FromBody] GetTopicExplanationDto dto, CancellationToken token)
    {
        return await _gptService.EnrichTopic(dto.Prompt, dto.Name, token);
    }
    
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceReplacement();
}