using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MindMapper.WebApi.Dto;
using MindMapper.WebApi.Models;
using MindMapper.WebApi.Services.Interfaces;

namespace MindMapper.WebApi.Controllers;

[ApiController]
[Route("/api/results")]
public partial class ResultsController
{
    private readonly ITopicsService _topicsService;
    
    public ResultsController(ITopicsService topicsService)
    {
        _topicsService = topicsService;
    }
    
    [HttpGet("GetDocumentTopics")]
    public async Task<GetDocumentTopicsResultDto> GetDocumentTopics([FromQuery] GetDocumentTopicsRequest request)
    {
        var result = await _topicsService.GetDocumentInfoAsync(request.Id);
        if (result is null)
        {
            return null;
        }

        return new GetDocumentTopicsResultDto(
            IsReady: result.IsReady,
            DocumentId: result.DocumentId,
            Name: result.Name,
            Topics: result
                .Topics
                .Select(x => new DocumentTopicsDto(x.Id, x.Name, x.Text))
                .ToArray()
        );
    }

    [HttpGet("GetAllDocuments")]
    public async Task<IReadOnlyCollection<GetDocumentTopicsResultDto>> GetAllDocuments()
    {
        var documents = await _topicsService.GetAllDocuments();
        return documents.Select(result => new GetDocumentTopicsResultDto(
            IsReady: result.IsReady,
            DocumentId: result.DocumentId,
            Name: result.Name,
            Topics: result
                .Topics
                .Select(x => new DocumentTopicsDto(x.Id, x.Name, x.Text))
                .ToArray())).ToArray();
    }
}