using Microsoft.AspNetCore.Mvc;
using MindMapper.WebApi.Models;
using MindMapper.WebApi.Services.Interfaces;

namespace MindMapper.WebApi.Controllers
{

    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _uploadService;

        public FilesController(IFileService uploadService)
        {
            _uploadService = uploadService;
        }

        /// <summary>
        /// Single File Upload
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("PostSingleFile")]
        public async Task<ActionResult> PostSingleFile([FromForm] FileUploadModel fileDetails)
        {
            if (fileDetails?.FileDetails == null || !fileDetails.FileDetails.FileName.EndsWith(".pdf"))
            {
                return BadRequest();
            }

            await _uploadService.PostFileAsync(fileDetails.FileDetails);
            return Ok();
        }

        /// <summary>
        /// Multiple File Upload
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("PostMultipleFile")]
        public async Task<ActionResult> PostMultipleFile([FromForm] List<FileUploadModel> fileDetails)
        {
            if (fileDetails.Any(x => x.FileDetails is null) || fileDetails.Any(x => !x.FileDetails.FileName.EndsWith(".pdf")))
            {
                return BadRequest();
            }

            await _uploadService.PostMultiFileAsync(fileDetails);
            return Ok();
        }
    }
}
