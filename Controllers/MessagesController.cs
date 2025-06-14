using ColocationAppBackend.BL;
using ColocationAppBackend.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ColocationAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(IMessageService messageService, ILogger<MessagesController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        [HttpGet("conversations")]
        public async Task<ActionResult<IEnumerable<ConversationDto>>> GetConversations()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var conversations = await _messageService.GetUserConversationsAsync(userId);
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des conversations");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpGet("conversations/{conversationId}")]
        public async Task<ActionResult<ConversationDto>> GetConversation(int conversationId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var conversation = await _messageService.GetConversationAsync(conversationId, userId);

                if (conversation == null)
                    return NotFound("Conversation non trouvée");

                return Ok(conversation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la conversation");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpPost("conversations")]
        public async Task<ActionResult<ConversationDto>> CreateConversation([FromBody] CreateConversationRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _logger.LogInformation($"Tentative de création de conversation entre {userId} et {request.UtilisateurId}");

                var conversation = await _messageService.CreateOrGetConversationAsync(userId, request.UtilisateurId);

                if (conversation == null)
                {
                    _logger.LogError("La création a retourné null");
                    return BadRequest("Impossible de créer la conversation");
                }

                return Ok(conversation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la conversation");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpGet("conversations/{conversationId}/messages")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(int conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var messages = await _messageService.GetConversationMessagesAsync(conversationId, userId, page, pageSize);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des messages");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpPost("conversations/{conversationId}/messages")]
        public async Task<ActionResult<MessageDto>> SendMessage(int conversationId, [FromBody] SendMessageRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var message = await _messageService.SendMessageAsync(conversationId, userId, request.Contenu);

                if (message == null)
                    return BadRequest("Impossible d'envoyer le message");

                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi du message");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpPut("conversations/{conversationId}/read")]
        public async Task<IActionResult> MarkAsRead(int conversationId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                await _messageService.MarkMessagesAsReadAsync(conversationId, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du marquage comme lu");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var count = await _messageService.GetUnreadMessageCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du comptage des messages non lus");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}
