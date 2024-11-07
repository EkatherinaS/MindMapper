using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MindMapper.WebApi.Models;
using MindMapper.WebApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using MindMapper.WebApi.Data;
using MindMapper.WebApi.Data.Entities;
using UglyToad.PdfPig;
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
        var tasks = fileData.Select(file => PostFileAsync(file.File));
        await Task.WhenAll(tasks);
    }

    public async Task<string?> GetFileContents(long id, CancellationToken token)
    {
        var file = await _context.Documents.FirstOrDefaultAsync(x => x.Id == id, token);
        if (file is null)
        {
            return null;
        }

        try
        {
            var filepath = Path.Combine(_fileOptions.Value.SavePath, file.SavedName);
            using var pdf = PdfDocument.Open(filepath);


            var builder = new StringBuilder();
            foreach (var page in pdf.GetPages())
            {
                var text = Regex.Replace(page.Text, @"[^\d\w ]", string.Empty, RegexOptions.Compiled);
                builder.Append(text);
            }
            
            return builder.ToString();
        }
        catch (Exception _)
        {
            return null;
        }
    }
}