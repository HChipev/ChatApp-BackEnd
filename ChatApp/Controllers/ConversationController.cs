using Back_End.Controllers.Abstract;
using Common.Enums;
using Data.ViewModels.Conversation.Models;
using Infrastructure.Attributes;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [HasPermission(Permission.NonSubscriber)]
    public class ConversationController : AbstractController
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HasPermission(Permission.Subscriber)]
        [HttpPost("ask")]
        public async Task<IActionResult> GenerateAnswer([FromBody] GenerateQuestionViewModel model)
        {
            var result = await _conversationService.GenerateAnswerAsync(GetUserId(), model);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpGet("all")]
        public IActionResult GetUserConversations([FromQuery] int userId)
        {
            if (GetUserId() != userId)
            {
                return Forbid();
            }

            var result = _conversationService.GetConversationsByUserId(userId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpGet("{conversationId:int}")]
        public IActionResult GetConversation([FromRoute] int conversationId)
        {
            var result = _conversationService.GetConversation(GetUserId(), conversationId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpDelete("{conversationId:int}")]
        public async Task<IActionResult> DeleteConversation([FromRoute] int conversationId)
        {
            var result = await _conversationService.DeleteConversationAsync(GetUserId(), conversationId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }


        [HttpPut("{conversationId:int}")]
        public async Task<IActionResult> ShareConversation([FromRoute] int conversationId)
        {
            var result = await _conversationService.ShareConversationAsync(GetUserId(), conversationId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
        }
    }
}