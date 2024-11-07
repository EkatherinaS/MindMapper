namespace MindMapper.WebApi.Models;

public record DocumentModel(long DocumentId, string Name, IReadOnlyCollection<TopicModel> Topics, bool IsReady);

public record TopicModel(long Id, string Name, string Text, long? PreviousTopicId);