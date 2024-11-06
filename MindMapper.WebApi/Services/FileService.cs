using MindMapper.WebApi.Models;
using MindMapper.WebApi.Services.Interfaces;
using System.Text.RegularExpressions;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig;

namespace MindMapper.WebApi.Services
{
    public class FileService : IFileService
    {
        //private readonly DbContextClass dbContextClass;

        //public FileService(DbContextClass dbContextClass)
        //{
        //    this.dbContextClass = dbContextClass;
        //}

        public async Task PostFileAsync(IFormFile fileData, FileType fileType)
        {
            try
            {
                var fileDetails = new FileDetails()
                {
                    ID = 0,
                    FileName = fileData.FileName,
                    FileType = fileType,
                };

                using (var stream = new MemoryStream())
                {
                    fileData.CopyTo(stream);
                    fileDetails.FileData = stream.ToArray();


                }

                using (var pdf = PdfDocument.Open(fileDetails.FileData))
                {
                    foreach (var page in pdf.GetPages())
                    {
                        // Either extract based on order in the underlying document with newlines and spaces.
                        var text = ContentOrderTextExtractor.GetText(page);

                        text = Regex.Replace(text, @"[^\w\s]", string.Empty);
                        // Or based on grouping letters into words.
                        var otherText = string.Join(" ", page.GetWords());

                        // Or the raw text of the page's content stream.
                        var rawText = page.Text;

                    }

                }

                //   var result = dbContextClass.FileDetails.Add(fileDetails);
                //   await dbContextClass.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task PostMultiFileAsync(List<FileUploadModel> fileData)
        {
            try
            {
                foreach (FileUploadModel file in fileData)
                {
                    var fileDetails = new FileDetails()
                    {
                        ID = 0,
                        FileName = file.FileDetails.FileName,
                        FileType = file.FileType,
                    };

                    using (var stream = new MemoryStream())
                    {
                        file.FileDetails.CopyTo(stream);
                        fileDetails.FileData = stream.ToArray();
                    }

                 //   var result = dbContextClass.FileDetails.Add(fileDetails);
                }
               // await dbContextClass.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public async Task DownloadFileById(int Id)
        //{
        //    try
        //    {
        //        var file = dbContextClass.FileDetails.Where(x => x.ID == Id).FirstOrDefaultAsync();

        //        var content = new System.IO.MemoryStream(file.Result.FileData);
        //        var path = Path.Combine(
        //           Directory.GetCurrentDirectory(), "FileDownloaded",
        //           file.Result.FileName);

        //        await CopyStream(content, path);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public async Task CopyStream(Stream stream, string downloadPath)
        {
            using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
        }
    }
}
