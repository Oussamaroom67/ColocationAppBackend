using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ColocationAppBackend.Data;
using ColocationAppBackend.Models;
using System.Security.Claims;

namespace ColocationAppBackend.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChatHub> _logger;
        private static readonly Dictionary<string, string> _connections = new();

        public ChatHub(ApplicationDbContext context, ILogger<ChatHub> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _connections[userId] = Context.ConnectionId;
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");

                // Marquer l'utilisateur comme en ligne
                await UpdateUserStatus(int.Parse(userId), true);

                _logger.LogInformation($"User {userId} connected with connection {Context.ConnectionId}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _connections.Remove(userId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");

                // Marquer l'utilisateur comme hors ligne
                await UpdateUserStatus(int.Parse(userId), false);

                _logger.LogInformation($"User {userId} disconnected");
            }
            await base.OnDisconnectedAsync(exception);
        }

        // Envoyer un message
        public async Task SendMessage(int conversationId, string contenu, int destinataireId)
        {
            try
            {
                var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                // Vérifier si la conversation existe et si l'utilisateur en fait partie
                var conversation = await _context.Conversations
                    .Include(c => c.Utilisateur1)
                    .Include(c => c.Utilisateur2)
                    .FirstOrDefaultAsync(c => c.Id == conversationId &&
                        (c.Utilisateur1Id == userId || c.Utilisateur2Id == userId));

                if (conversation == null)
                {
                    await Clients.Caller.SendAsync("Error", "Conversation non trouvée ou accès non autorisé");
                    return;
                }

                // Créer le nouveau message
                var message = new Message
                {
                    ExpediteurId = userId,
                    Contenu = contenu,
                    ConversationId = conversationId,
                    DateEnvoi = DateTime.UtcNow,
                    EstLu = false
                };

                _context.Messages.Add(message);

                // Mettre à jour la conversation
                conversation.DernierMessage = contenu;
                conversation.DateDernierMessage = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Charger les données complètes du message
                await _context.Entry(message)
                    .Reference(m => m.Expediteur)
                    .LoadAsync();

                // Préparer les données à envoyer
                var messageData = new
                {
                    Id = message.Id,
                    ConversationId = message.ConversationId,
                    Contenu = message.Contenu,
                    DateEnvoi = message.DateEnvoi,
                    EstLu = message.EstLu,
                    Expediteur = new
                    {
                        Id = message.Expediteur.Id,
                        Nom = message.Expediteur.Nom,
                        Prenom = message.Expediteur.Prenom,
                        AvatarUrl = message.Expediteur.AvatarUrl
                    }
                };

                // Envoyer aux participants de la conversation
                await Clients.Group($"User_{conversation.Utilisateur1Id}")
                    .SendAsync("ReceiveMessage", messageData);

                await Clients.Group($"User_{conversation.Utilisateur2Id}")
                    .SendAsync("ReceiveMessage", messageData);

                _logger.LogInformation($"Message envoyé dans la conversation {conversationId} par l'utilisateur {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi du message");
                await Clients.Caller.SendAsync("Error", "Erreur lors de l'envoi du message");
            }
        }

        // Marquer un message comme lu
        public async Task MarkMessageAsRead(int messageId)
        {
            try
            {
                var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var message = await _context.Messages
                    .Include(m => m.Conversation)
                    .FirstOrDefaultAsync(m => m.Id == messageId);

                if (message == null) return;

                // Vérifier que l'utilisateur est le destinataire
                var conversation = message.Conversation;
                if ((conversation.Utilisateur1Id == userId && message.ExpediteurId != userId) ||
                    (conversation.Utilisateur2Id == userId && message.ExpediteurId != userId))
                {
                    message.EstLu = true;
                    message.DateLecture = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    // Notifier l'expéditeur que son message a été lu
                    await Clients.Group($"User_{message.ExpediteurId}")
                        .SendAsync("MessageRead", new { MessageId = messageId, ReadAt = message.DateLecture });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du marquage du message comme lu");
            }
        }

        // Rejoindre une conversation
        public async Task JoinConversation(int conversationId)
        {
            try
            {
                var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var conversation = await _context.Conversations
                    .FirstOrDefaultAsync(c => c.Id == conversationId &&
                        (c.Utilisateur1Id == userId || c.Utilisateur2Id == userId));

                if (conversation != null)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
                    _logger.LogInformation($"User {userId} joined conversation {conversationId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la jointure à la conversation");
            }
        }

        // Quitter une conversation
        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
        }

        // Indiquer que l'utilisateur est en train de taper
        public async Task StartTyping(int conversationId)
        {
            try
            {
                var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                // Notifier l'autre participant
                await Clients.OthersInGroup($"Conversation_{conversationId}")
                    .SendAsync("UserTyping", new { UserId = userId, ConversationId = conversationId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'indication de frappe");
            }
        }

        // Indiquer que l'utilisateur a arrêté de taper
        public async Task StopTyping(int conversationId)
        {
            try
            {
                var userId = int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                await Clients.OthersInGroup($"Conversation_{conversationId}")
                    .SendAsync("UserStoppedTyping", new { UserId = userId, ConversationId = conversationId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'arrêt d'indication de frappe");
            }
        }

        private async Task UpdateUserStatus(int userId, bool isOnline)
        {
            try
            {
                var user = await _context.Utilisateurs.FindAsync(userId);
                if (user != null)
                {
                    user.DernierConnexion = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    // Notifier les contacts de l'utilisateur du changement de statut
                    var conversations = await _context.Conversations
                        .Where(c => c.Utilisateur1Id == userId || c.Utilisateur2Id == userId)
                        .ToListAsync();

                    foreach (var conv in conversations)
                    {
                        var otherUserId = conv.Utilisateur1Id == userId ? conv.Utilisateur2Id : conv.Utilisateur1Id;
                        await Clients.Group($"User_{otherUserId}")
                            .SendAsync("UserStatusChanged", new { UserId = userId, IsOnline = isOnline });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du statut utilisateur");
            }
        }
    }
}