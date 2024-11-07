using Microsoft.AspNetCore.Mvc;
using MindMapper.WebApi.Dto;
using MindMapper.WebApi.Services.Interfaces;

namespace MindMapper.WebApi.Controllers;

[ApiController]
[Route("/results")]
public class ResultsController
{
    private readonly ITopicsService _topicsService;

    public ResultsController(ITopicsService topicsService)
    {
        _topicsService = topicsService;
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
}