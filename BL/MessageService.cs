using ColocationAppBackend.Data;
using ColocationAppBackend.Models;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MessageService> _logger;

        public MessageService(ApplicationDbContext context, ILogger<MessageService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(int userId)
        {
            try
            {
                var conversations = await _context.Conversations
                    .Include(c => c.Utilisateur1)
                    .Include(c => c.Utilisateur2)
                    .Include(c => c.Messages.OrderByDescending(m => m.DateEnvoi).Take(1))
                        .ThenInclude(m => m.Expediteur)
                    .Where(c => c.Utilisateur1Id == userId || c.Utilisateur2Id == userId)
                    .OrderByDescending(c => c.DateDernierMessage ?? c.DateCreation)
                    .ToListAsync();

                return conversations.Select(c => new ConversationDto
                {
                    Id = c.Id,
                    DateCreation = c.DateCreation,
                    DateDernierMessage = c.DateDernierMessage,
                    DernierMessage = c.DernierMessage,
                    AutreUtilisateur = c.Utilisateur1Id == userId
                        ? new UtilisateurDto
                        {
                            Id = c.Utilisateur2.Id,
                            Nom = c.Utilisateur2.Nom,
                            Prenom = c.Utilisateur2.Prenom,
                            AvatarUrl = c.Utilisateur2.AvatarUrl,
                            EstEnLigne = IsUserOnline(c.Utilisateur2.Id)
                        }
                        : new UtilisateurDto
                        {
                            Id = c.Utilisateur1.Id,
                            Nom = c.Utilisateur1.Nom,
                            Prenom = c.Utilisateur1.Prenom,
                            AvatarUrl = c.Utilisateur1.AvatarUrl,
                            EstEnLigne = IsUserOnline(c.Utilisateur1.Id)
                        },
                    MessagesNonLus = c.Messages.Count(m => m.ExpediteurId != userId && !m.EstLu)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des conversations");
                return Enumerable.Empty<ConversationDto>();
            }
        }

        public async Task<ConversationDto> GetConversationAsync(int conversationId, int userId)
        {
            try
            {
                var conversation = await _context.Conversations
                    .Include(c => c.Utilisateur1)
                    .Include(c => c.Utilisateur2)
                    .FirstOrDefaultAsync(c => c.Id == conversationId &&
                        (c.Utilisateur1Id == userId || c.Utilisateur2Id == userId));

                if (conversation == null) return null;

                return new ConversationDto
                {
                    Id = conversation.Id,
                    DateCreation = conversation.DateCreation,
                    DateDernierMessage = conversation.DateDernierMessage,
                    DernierMessage = conversation.DernierMessage,
                    AutreUtilisateur = conversation.Utilisateur1Id == userId
                        ? new UtilisateurDto
                        {
                            Id = conversation.Utilisateur2.Id,
                            Nom = conversation.Utilisateur2.Nom,
                            Prenom = conversation.Utilisateur2.Prenom,
                            AvatarUrl = conversation.Utilisateur2.AvatarUrl,
                            EstEnLigne = IsUserOnline(conversation.Utilisateur2.Id)
                        }
                        : new UtilisateurDto
                        {
                            Id = conversation.Utilisateur1.Id,
                            Nom = conversation.Utilisateur1.Nom,
                            Prenom = conversation.Utilisateur1.Prenom,
                            AvatarUrl = conversation.Utilisateur1.AvatarUrl,
                            EstEnLigne = IsUserOnline(conversation.Utilisateur1.Id)
                        }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la conversation");
                return null;
            }
        }

        public async Task<ConversationDto> CreateOrGetConversationAsync(int userId1, int userId2)
        {
            try
            {
                // Vérifier si une conversation existe déjà
                var existingConversation = await _context.Conversations
                    .Include(c => c.Utilisateur1)
                    .Include(c => c.Utilisateur2)
                    .FirstOrDefaultAsync(c =>
                        (c.Utilisateur1Id == userId1 && c.Utilisateur2Id == userId2) ||
                        (c.Utilisateur1Id == userId2 && c.Utilisateur2Id == userId1));

                if (existingConversation != null)
                {
                    return await GetConversationAsync(existingConversation.Id, userId1);
                }

                // Créer une nouvelle conversation
                var newConversation = new Conversation
                {
                    Utilisateur1Id = userId1,
                    Utilisateur2Id = userId2,
                    DateCreation = DateTime.UtcNow,
                    DernierMessage = "" // Valeur par défaut

                };

                _context.Conversations.Add(newConversation);
                await _context.SaveChangesAsync();

                return await GetConversationAsync(newConversation.Id, userId1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création/récupération de la conversation");
                return null;
            }
        }

        public async Task<IEnumerable<MessageDto>> GetConversationMessagesAsync(int conversationId, int userId, int page = 1, int pageSize = 50)
        {
            try
            {
                // Vérifier l'accès à la conversation
                var hasAccess = await _context.Conversations
                    .AnyAsync(c => c.Id == conversationId &&
                        (c.Utilisateur1Id == userId || c.Utilisateur2Id == userId));

                if (!hasAccess) return Enumerable.Empty<MessageDto>();

                var messages = await _context.Messages
                    .Include(m => m.Expediteur)
                    .Where(m => m.ConversationId == conversationId)
                    .OrderByDescending(m => m.DateEnvoi)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return messages.Select(m => new MessageDto
                {
                    Id = m.Id,
                    Contenu = m.Contenu,
                    DateEnvoi = m.DateEnvoi,
                    DateLecture = m.DateLecture,
                    EstLu = m.EstLu,
                    Expediteur = new UtilisateurDto
                    {
                        Id = m.Expediteur.Id,
                        Nom = m.Expediteur.Nom,
                        Prenom = m.Expediteur.Prenom,
                        AvatarUrl = m.Expediteur.AvatarUrl
                    },
                    EstDeMoi = m.ExpediteurId == userId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des messages");
                return Enumerable.Empty<MessageDto>();
            }
        }

        public async Task<MessageDto> SendMessageAsync(int conversationId, int senderId, string contenu)
        {
            try
            {
                // Vérifier l'accès à la conversation
                var conversation = await _context.Conversations
                    .FirstOrDefaultAsync(c => c.Id == conversationId &&
                        (c.Utilisateur1Id == senderId || c.Utilisateur2Id == senderId));

                if (conversation == null) return null;

                var message = new Message
                {
                    ExpediteurId = senderId,
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

                // Charger l'expéditeur
                await _context.Entry(message)
                    .Reference(m => m.Expediteur)
                    .LoadAsync();

                return new MessageDto
                {
                    Id = message.Id,
                    Contenu = message.Contenu,
                    DateEnvoi = message.DateEnvoi,
                    DateLecture = message.DateLecture,
                    EstLu = message.EstLu,
                    Expediteur = new UtilisateurDto
                    {
                        Id = message.Expediteur.Id,
                        Nom = message.Expediteur.Nom,
                        Prenom = message.Expediteur.Prenom,
                        AvatarUrl = message.Expediteur.AvatarUrl
                    },
                    EstDeMoi = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi du message");
                return null;
            }
        }

        public async Task MarkMessagesAsReadAsync(int conversationId, int userId)
        {
            try
            {
                var messages = await _context.Messages
                    .Where(m => m.ConversationId == conversationId &&
                               m.ExpediteurId != userId &&
                               !m.EstLu)
                    .ToListAsync();

                foreach (var message in messages)
                {
                    message.EstLu = true;
                    message.DateLecture = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du marquage des messages comme lus");
            }
        }

        public async Task<int> GetUnreadMessageCountAsync(int userId)
        {
            try
            {
                return await _context.Messages
                    .Include(m => m.Conversation)
                    .Where(m => m.ExpediteurId != userId &&
                               !m.EstLu &&
                               (m.Conversation.Utilisateur1Id == userId || m.Conversation.Utilisateur2Id == userId))
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du comptage des messages non lus");
                return 0;
            }
        }

        private bool IsUserOnline(int userId)
        {
            // Cette méthode devrait vérifier si l'utilisateur est actuellement connecté
            // Pour simplifier, on considère qu'un utilisateur est en ligne s'il s'est connecté dans les 5 dernières minutes
            var user = _context.Utilisateurs.Find(userId);
            return user != null && user.DernierConnexion > DateTime.UtcNow.AddMinutes(-1);
        }
    }
}
