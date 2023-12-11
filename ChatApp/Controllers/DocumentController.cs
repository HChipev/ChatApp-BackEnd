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
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("add")]
        public IActionResult LoadDocuments([FromBody] DocumentsViewModel models)
        {
            var result = _documentService.AddDocuments(models);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpGet("all")]
        public IActionResult GetDocuments()
        {
            var result = _documentService.GetDocuments();

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpDelete("delete/{documentId}")]
        public IActionResult DeleteDocument(int documentId)
        {
            var result = _documentService.DeleteDocument(documentId);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpPut("restore/{documentId}")]
        public IActionResult RestoreDocument(int documentId)
        {
            var result = _documentService.RestoreDocument(documentId);

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }
    }
}