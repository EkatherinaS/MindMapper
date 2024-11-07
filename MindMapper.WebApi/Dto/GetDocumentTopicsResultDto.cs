using MindMapper.WebApi.Data.Entities;

namespace MindMapper.WebApi.Dto;

public record GetDocumentTopicsResultDto(long DocumentId, string Name, IReadOnlyCollection<DocumentTopicsDto> Topics, bool IsReady);

public record DocumentTopicsDto(long Id, string Name, string Text);