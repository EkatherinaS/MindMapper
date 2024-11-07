using System.ComponentModel;

namespace MindMapper.WebApi.Data.Entities;

public class Topic
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public long DocumentId { get; set; }
    
    public Document Document { get; set; }
    
    public long? PreviousTopicId { get; set; }
    
    [DefaultValue(false)]
    public bool AnalysisCompleted { get; set; }
    
    public string Text { get; set; }
}