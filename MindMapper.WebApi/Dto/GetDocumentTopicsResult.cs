using MindMapper.WebApi.Data.Entities;

namespace MindMapper.WebApi.Dto;

public record GetDocumentTopicsResult(long DocumentId, IReadOnlyCollection<DocumentTopics> Topics, bool IsReady);

public record DocumentTopics(long Id, string Name, string Text);