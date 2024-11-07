using Microsoft.AspNetCore.Mvc;
using MindMapper.WebApi.Dto;
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
        [ProducesResponseType<LoadDocumentDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostSingleFile([FromForm] FileUploadModel fileDetails)
        {
            if (fileDetails?.File == null || !fileDetails.File.FileName.EndsWith(".pdf"))
            {
                return BadRequest();
            }

            var documentId = await _uploadService.PostFileAsync(fileDetails.File);
            return new JsonResult(new LoadDocumentDto(documentId));
        }

        /// <summary>
        /// Multiple File Upload
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("PostMultipleFile")]
        public async Task<ActionResult> PostMultipleFile([FromForm] List<FileUploadModel> fileDetails)
        {
            if (fileDetails.Any(x => x.File is null) || fileDetails.Any(x => !x.File.FileName.EndsWith(".pdf")))
            {
                return BadRequest();
            }

            await _uploadService.PostMultiFileAsync(fileDetails);
            return Ok();
        }
    }
}
