using Microsoft.EntityFrameworkCore;
using MindMapper.WebApi.Data;
using MindMapper.WebApi.Models;

namespace MindMapper.WebApi.Services.Interfaces;

public interface ITopicsService
{
    public Task<DocumentModel?> GetDocumentInfoAsync(long id);
}