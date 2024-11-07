using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MindMapper.WebApi.Dto;
using MindMapper.WebApi.Models;
using MindMapper.WebApi.Services.Interfaces;

namespace MindMapper.WebApi.Controllers;

[ApiController]
[Route("/api/gpt")]
public partial class GptController
{
    private readonly IGptService _gptService;

    public GptController(IGptService gptService)
    {
        _gptService = gptService;
    }

    [HttpGet("Query")]
    public async Task<string> QueryYandexGpt([FromQuery] string prompt, CancellationToken token)
    {
        return await _gptService.QueryPrompt(prompt, token);
    }

    [HttpPost("QueryInstruction")]
    public async Task<string> QueryInstructionYandexGpt([FromBody] QueryInstructionGptDto dto, CancellationToken token)
    {
        var instruction = WhitespaceReplacement().Replace(dto.Instruction, " ");
        var prompt = WhitespaceReplacement().Replace(dto.Prompt, " ");
        return await _gptService.QueryInstruction(instruction, prompt, token);
    }

    [HttpPost("QueryTopics")]
    public async Task<IReadOnlyCollection<TopicModel>> QueryTopics([FromBody] QueryTopicsFromTextDto dto, CancellationToken token)
    {
        var result = await _gptService.QueryTopics(dto.Text, token);
        return result;
    }

    [HttpPost("EnrichTopic")]
    public async Task<string> GetTopicExplanation([FromBody] GetTopicExplanationDto dto, CancellationToken token)
    {
        return await _gptService.EnrichTopic(dto.Prompt, dto.Name, token);
    }
    
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceReplacement();
}