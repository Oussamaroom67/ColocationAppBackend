using ColocationAppBackend.DTOs.Responses;
using System.Collections.Generic;
using ColocationAppBackend.DTOs.Requests;
namespace ColocationAppBackend.BL
{
    public interface IMessageService
    {
        Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(int userId);
        Task<ConversationDto> GetConversationAsync(int conversationId, int userId);
        Task<ConversationDto> CreateOrGetConversationAsync(int userId1, int userId2);
        Task<IEnumerable<MessageDto>> GetConversationMessagesAsync(int conversationId, int userId, int page = 1, int pageSize = 50);
        Task<MessageDto> SendMessageAsync(int conversationId, int senderId, string contenu);
        Task MarkMessagesAsReadAsync(int conversationId, int userId);
        Task<int> GetUnreadMessageCountAsync(int userId);
    }
}
