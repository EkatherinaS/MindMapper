using System.Collections.Generic;

namespace MindMapper.WebApi.Data.Entities;

public class Document
{
    public long Id { get; set; }

    public string SavedName { get; set; }

    public string OriginalName { get; set; }
    public virtual ICollection<Topic> Topics { get; set; }

    public bool IsCompleted => Topics.All(x => x.AnalysisCompleted);
}