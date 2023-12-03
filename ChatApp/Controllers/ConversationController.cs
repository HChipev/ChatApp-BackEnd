using Data.ViewModels.Conversation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpPost("ask")]
        public IActionResult GenerateAnswer([FromBody] GenerateAnswerViewModel model)
        {
            var result = _conversationService.GenerateAnswer(model);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }
    }
}