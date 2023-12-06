using Back_End.Controllers.Abstract;
using Data.ViewModels.Conversation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationController : AbstractController
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpPost("ask")]
        public IActionResult GenerateAnswer([FromBody] GenerateQuestionViewModel model)
        {
            var result = _conversationService.GenerateAnswer(model);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpGet("all")]
        public IActionResult GetUSerConversations([FromQuery] int userId)
        {
            if (GetUserId() != userId)
            {
                return Forbid();
            }

            var result = _conversationService.GetUserConversations(userId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }
    }
}