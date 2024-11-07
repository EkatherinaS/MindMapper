using MindMapper.WebApi.Data.Entities;

namespace MindMapper.WebApi.Models;

public record DocumentModel(long DocumentId, IReadOnlyCollection<TopicModel> Topics, bool IsReady);

public record TopicModel(long Id, string Name, string Text);