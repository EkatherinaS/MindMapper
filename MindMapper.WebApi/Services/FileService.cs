using MindMapper.WebApi.Models;
using MindMapper.WebApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using MindMapper.WebApi.Data;
using MindMapper.WebApi.Data.Entities;
using FileOptions = MindMapper.WebApi.Options.FileOptions;

namespace MindMapper.WebApi.Services;

public class FileService : IFileService
{
    private readonly IOptions<FileOptions> _fileOptions;
    private readonly ApplicationDbContext _context;

    public FileService(IOptions<FileOptions> fileOptions, ApplicationDbContext context)
    {
        _fileOptions = fileOptions;
        _context = context;
    }

    public async Task PostFileAsync(IFormFile fileData)
    {
        var newName = $"{Guid.NewGuid()}.pdf";
        var originalName = fileData.FileName;

        await using var stream = fileData.OpenReadStream();
        using var memStream = new MemoryStream();
        await stream.CopyToAsync(memStream);

        var resultPath = Path.Combine(_fileOptions.Value.SavePath, newName);
        await File.WriteAllBytesAsync(resultPath, memStream.ToArray());

        var document = new Document
        {
            SavedName = newName,
            OriginalName = originalName
        };

        await _context.Documents.AddAsync(document);
        await _context.SaveChangesAsync();
    }

    public async Task PostMultiFileAsync(List<FileUploadModel> fileData)
    {
        var tasks = fileData.Select(file => PostFileAsync(file.FileDetails));
        await Task.WhenAll(tasks);
    }
}