﻿using ColocationAppBackend.Data;
using ColocationAppBackend.DTOs.Requests;
using ColocationAppBackend.DTOs.Responses;
using ColocationAppBackend.Enums;
using ColocationAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ColocationAppBackend.BL
{
    public class UtilisateurService
    {
        private readonly ApplicationDbContext _context;
        public UtilisateurService(ApplicationDbContext context)
        {
            _context = context;
        }
        //Get all Users
        public async Task<List<UtilisateurDto>> GetAllUtilisateursAsync()
        {
            var utilisateurs = await _context.Utilisateurs.ToListAsync();
            var result = utilisateurs.Select(u => new UtilisateurDto
            {
                Id = u.Id,
                Nom = u.Nom + " " + u.Prenom,
                Email = u.Email,
                TypeUtilisateur = u is Etudiant ? "Étudiant" :
                  u is Proprietaire ? "Propriétaire" :
                  u is Administrateur ? "Administrateur" : "Inconnu",
                Statut = IsUserSuspended(u) ? "Suspendu" : "Actif",
                Verifie = u.EstVerifie ? "Vérifié" : "Non vérifié",
                DateInscription = u.DateInscription,
                NbrUtilisateur = utilisateurs.Count(),
                NbrProprietaire = utilisateurs.Count(r => r is Proprietaire),
                NbrEtudiants = utilisateurs.Count(r => r is Etudiant),
                NbrAdministrateurs = utilisateurs.Count(r => r is Administrateur),
            }).ToList();

            return result;
        }
        //bannir User
        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Utilisateurs.FirstOrDefaultAsync(r => r.Id == id);
                if (user == null)
                    return false;
                _context.Utilisateurs.Remove(user);
                _context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        //suspendre user
        public async Task<bool> SuspendreUser(int id, bool suspendre)
        {
            var user = await _context.Utilisateurs.FirstOrDefaultAsync(r => r.Id == id);
            if (user == null) return false;

            if (suspendre)
            {
                user.LastSuspendedAt = DateTime.UtcNow;
            }
            else
            {
                user.LastSuspendedAt = null;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public bool IsUserSuspended(Utilisateur user)
        {
            if (user.LastSuspendedAt == null)
                return false;

            return user.LastSuspendedAt.Value.AddDays(7) > DateTime.UtcNow;
        }


    }
}
