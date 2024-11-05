using Microsoft.AspNetCore.Mvc;
using MindMapper.WebApi.Dto;
using MindMapper.WebApi.Models;
using MindMapper.WebApi.Services.Interfaces;

namespace MindMapper.WebApi.Controllers;

[ApiController]
[Route("/cool")]
public class CoolController
{
    private readonly ISomeShitService _service;

    public CoolController(ISomeShitService service)
    {
        _service = service;
    }

    [HttpGet("/tell-me-something")]
    public async Task<TellMeSomethingResponseDto> TellMeSomethingAsync(TellMeSomethingRequestDto request, CancellationToken token)
    {
        var result = await _service.SomeAction(new SomeCoolModel());
        return new TellMeSomethingResponseDto(result);
    }
}