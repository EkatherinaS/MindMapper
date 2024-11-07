using MindMapper.WebApi.Models;

namespace MindMapper.WebApi.Services.Interfaces
{
    public interface IFileService
    {
        public Task PostFileAsync(IFormFile fileData);

        public Task PostMultiFileAsync(List<FileUploadModel> fileData);

        public Task<string?> GetFileContents(long id, CancellationToken token);
    }
}
