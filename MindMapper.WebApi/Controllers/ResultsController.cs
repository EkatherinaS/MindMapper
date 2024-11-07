using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MindMapper.WebApi.Dto;
using MindMapper.WebApi.Options;
using MindMapper.WebApi.Services.Interfaces;
using YandexGPTWrapper;

namespace MindMapper.WebApi.Controllers;

[ApiController]
[Route("/results")]
public class ResultsController
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
}