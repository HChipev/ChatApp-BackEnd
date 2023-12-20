using Back_End.Controllers.Abstract;
using Common.Classes;
using Data.ViewModels.Document.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class DocumentController : AbstractController
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> LoadDocuments([FromBody] DocumentsViewModel models)
        {
            var result = await _documentService.AddDocumentsAsync(models, GetUserId());

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpGet("all")]
        public IActionResult GetDocuments()
        {
            var result = _documentService.GetDocuments();

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpDelete("delete/{documentId:int}")]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {
            var result = await _documentService.DeleteDocumentAsync(documentId, GetUserId());

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpPut("restore/{documentId:int}")]
        public async Task<IActionResult> RestoreDocument(int documentId)
        {
            var result = await _documentService.RestoreDocumentAsync(documentId, GetUserId());

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }
    }
}