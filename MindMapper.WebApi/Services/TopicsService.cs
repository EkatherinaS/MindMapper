using Microsoft.EntityFrameworkCore;
using MindMapper.WebApi.Data;
using MindMapper.WebApi.Models;
using MindMapper.WebApi.Services.Interfaces;

namespace MindMapper.WebApi.Services;

public class TopicsService : ITopicsService
{
    private readonly ApplicationDbContext _context;

    public TopicsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentModel?> GetDocumentInfoAsync(long id)
    {
        var document = await _context.Documents
            .Include(x => x.Topics)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
        
        if (document is null)
        {
            return null;
        }

        return new DocumentModel(
            DocumentId: document.Id,
            IsReady: document.Topics.Count > 0 && document.Topics.All(x => x.AnalysisCompleted),
            Name: document.OriginalName,
            Topics: document
                .Topics
                .Select(x => new TopicModel(x.Id, x.Name, x.Text, x.PreviousTopicId))
                .ToArray()
        );
    }

    public async Task<IReadOnlyCollection<DocumentModel>> GetAllDocuments()
    {
        var documents = await _context.Documents
            .Include(x => x.Topics)
            .ToArrayAsync();

        return documents.Select(document => new DocumentModel(
            DocumentId: document.Id,
            IsReady: document.Topics.Count > 0 && document.Topics.All(x => x.AnalysisCompleted),
            Name: document.OriginalName,
            Topics: document
                .Topics
                .Select(x => new TopicModel(x.Id, x.Name, x.Text, x.PreviousTopicId))
                .ToArray()
        )).ToArray();
    }
}