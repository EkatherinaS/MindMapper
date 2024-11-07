using MindMapper.WebApi.Models;

namespace MindMapper.WebApi.Services.Interfaces;

public interface ITopicsService
{
    public Task<DocumentModel?> GetDocumentInfoAsync(long id);

    public Task<IReadOnlyCollection<DocumentModel>> GetAllDocuments();
}