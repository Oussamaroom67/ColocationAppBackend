# 🏡 ColocationAppBackend – Backend .NET pour la gestion intelligente de colocations étudiantes

![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core%209.0.5-009688)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-brightgreen)
![ML.NET](https://img.shields.io/badge/ML.NET-4.0.2-orange)
![JWT](https://img.shields.io/badge/Auth-JWT-red)
![SignalR](https://img.shields.io/badge/Realtime-SignalR-lightblue)
![Swagger](https://img.shields.io/badge/Swagger-Enabled-lightgrey)
![Postman](https://img.shields.io/badge/Postman-Tested-orange)

---

## 📚 Sommaire

- [🚀 À propos du projet](#-à-propos-du-projet)
- [🧱 Stack technique](#-stack-technique)
- [📐 Architecture Backend](#-architecture-backend)
- [🧩 Description des responsabilités](#-description-des-responsabilités)
- [👥 Gestion des utilisateurs](#-gestion-des-utilisateurs)
- [🏘️ Données logements et annonces](#️-données-logements-et-annonces)
- [👫 Gestion des colocations et candidatures](#-gestion-des-colocations-et-candidatures)
- [💬 Messagerie en temps réel (SignalR)](#-messagerie-en-temps-réel-signalr)
- [🔐 Authentification et sécurité](#-authentification-et-sécurité)
- [🧠 Moteur de recommandation ML.NET](#-moteur-de-recommandation-mlnet)
- [📎 Documentation API - Swagger](#-documentation-api---swagger)
- [🧪 Tests via Postman & Migrations](#-tests-via-postman--migrations)
- [🧑‍💻 Auteurs](#-auteurs)
- [📂 Clonage et exécution](#-clonage-et-exécution)

---

## 🚀 À propos du projet

**ColocationAppBackend** est une API REST moderne développée en ASP.NET Core 8.0 pour gérer une plateforme intelligente de colocation étudiante au Maroc.

Fonctionnalités principales :
- 🔍 Consulter les annonces et postuler à une colocation
- 🏠 Gérer les annonces et logements côté propriétaire
- 💬 Discuter via messagerie instantanée (SignalR)
- 🤖 Bénéficier d’une recommandation intelligente avec ML.NET
- 🧑‍⚖️ Gestion des utilisateurs et modération via un rôle Admin

---

## 🧱 Stack technique

| Élément             | Technologie                |
|---------------------|-----------------------------|
| Langage             | C#                          |
| Framework           | ASP.NET Core 8.0            |
| ORM                 | Entity Framework Core 9.0.5 |
| Base de données     | SQL Server                  |
| Authentification    | JWT + BCrypt.Net            |
| Temps réel          | SignalR                     |
| Machine Learning    | ML.NET 4.0.2                |
| Documentation API   | Swagger (Swashbuckle)       |
| Tests API           | Postman                     |

---

## 📐 Architecture Backend

L’application suit une architecture modulaire inspirée de **Clean Architecture**. Elle respecte les principes **SOLID**, le **découplage** et la **séparation des responsabilités**.

### 📁 Structure des dossiers

```plaintext
ColocationAppBackend/
│
├── BL/                    → Business Layer : logique métier
├── Controllers/           → REST Controllers (API publique)
├── DTOs/                  → Data Transfer Objects : entrées/sorties API
├── Data/                  → DbContext + configurations EF Core
├── Enums/                 → Types énumérés métiers (rôles, statuts…)
├── Hubs/                  → SignalR pour la messagerie temps réel
├── Migrations/            → Historique de la base EF
├── Models/                → Entités métiers et relations
├── Middlewares/           → Auth, exceptions, CORS, logging...
├── ML/                    → Logique de recommandation (ML.NET)
├── Utils/                 → Fonctions utilitaires (token, hash, etc.)
└── wwwroot/images/        → Images statiques (avatars, logements)

Fichiers principaux :
- `Program.cs`             → Point d’entrée de l’application
- `appsettings.json`       → Configuration globale
```
---
### 🧩 Description des responsabilités

| Couche         | Rôle                                                                 |
|----------------|----------------------------------------------------------------------|
| **Controllers** | Reçoit les requêtes, valide les données, appelle les services BL     |
| **BL**          | Implémente les règles métier (ex : vérifier si un étudiant peut postuler) |
| **Data**        | Configure Entity Framework Core, gère les relations et les migrations |
| **Models**      | Définit les entités mappées avec la base de données                  |
| **DTOs**        | Transporte les données en entrée/sortie sans exposer les entités     |
| **Hubs**        | Gère le chat en temps réel avec SignalR                             |
| **ML**          | Contient les modèles d’apprentissage et de prédiction              |
| **Middlewares** | Intergiciels : authentification, logging, gestion d’erreurs globales |

---

### 👥 Gestion des utilisateurs

Trois types de comptes héritant d’un modèle `Utilisateur` commun (via Discriminator EF Core) :

- **Étudiant**
  - Peut consulter les annonces et **postuler** à une ou plusieurs
  - Peut créer ou rejoindre une colocation
  - Peut **ajouter des annonces ou colocations aux favoris**
  - Peut **accepter ou refuser les demandes de colocation reçues**
  - Définit des préférences : budget, style de vie, université…
- **Propriétaire**
  - Gère ses logements et les annonces associées
  - Peut recevoir et accepter/refuser des candidatures
  - Peut discuter via messagerie avec les étudiants intéressés
- **Administrateur**
  - Modère les contenus
  - Gère les utilisateurs (suspension, suppression)
  - Dispose d’un accès complet au dashboard
---

### 🏘️ Données logements et annonces

Chaque annonce est liée à un logement publié par un propriétaire :

#### 📦 Logement
- Champs : adresse, surface, nombre de chambres, équipements (internet, chauffage…)
- Options possibles : animaux, parking, fumeurs, proximité université

#### 📢 Annonce
- Informations : prix, durée minimale, date de début/fin, titre, description
- Visibilité : active, expirée, masquée, supprimée
- Liée à un logement et à un propriétaire

---

### 👫 Gestion des colocations et candidatures

Un étudiant peut :
- Créer une colocation (et y inviter d'autres étudiants)
- Postuler à une annonce déjà publiée

Une candidature comprend :
- La référence de l’étudiant concerné
- Un message personnalisé
- Un **statut** : acceptée, refusée, en attente
- Des références croisées avec la colocation ciblée

---

### 💬 Messagerie en temps réel (SignalR)

Fonctionnalité de chat en direct entre étudiants et propriétaires :

- Utilise un **SignalR Hub**
- Permet :
  - Conversation instantanée
  - Mise à jour des messages sans rechargement
  - Suivi de l’état de lecture
- Entités principales : `Conversation`, `Message` (avec timestamp, état lu/non lu)

---

### 🔐 Authentification et sécurité

- Basée sur **JWT (Json Web Token)**
- Hachage des mots de passe via **BCrypt.Net**
- Attribution des rôles via `[Authorize(Roles = "...")]`
- Présence d’un middleware global pour :
  - Valider les tokens
  - Gérer les erreurs de manière centralisée
---
### 🧠 Moteur de recommandation ML.NET

Module IA utilisant **ML.NET** pour suggérer les meilleures colocations aux étudiants :

- Utilise un modèle de régression linéaire (SDCA) pour prédire un score de compatibilité  
- Entraîné sur des données synthétiques simulant différentes correspondances  
- Prend en compte plusieurs caractéristiques :  
  - Similarité de budget  
  - Correspondance d’école/université  
  - Proximité géographique  
  - Adéquation des dates d’entrée  
  - Compatibilité des préférences personnelles  
- Calcule un score entre 0 (pas compatible) et 1 (parfaitement compatible)  
- Recommande les colocations avec un score ≥ 0.6  
- Dispose d’un fallback avec un calcul pondéré simple en cas de problème avec le modèle ML  
- Permet des prédictions rapides pour un matching dynamique entre profils

---
### 📎 Documentation API - Swagger

- Interface Swagger générée automatiquement
- Accessible via l’URL `/swagger`
- Permet de :
  - Explorer tous les endpoints
  - Tester les requêtes directement sans client externe

#### 🔍 Exemples de routes disponibles :
- `POST /api/auth/login` → Connexion utilisateur  
- `GET /api/annonces` → Récupération des annonces disponibles  
- `POST /api/colocations` → Création d’une colocation  
- `GET /api/messages/{conversationId}` → Récupération d’une conversation

---

### 🧪 Tests via Postman & Migrations

#### ✅ Tests avec Postman
Collection complète incluant :
- Authentification (login, inscription)
- Gestion des annonces (ajout, modification, suppression)
- Postulation à des annonces
- Chat en temps réel
- Tests de cas d’échec et scénarios limites

#### 🛠 Migrations Entity Framework
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
- Relations définies avec précision
- Clés primaires et étrangères explicites
- Typage rigoureux : `decimal`, `bool`, `DateTime`, etc.

---

### 🧑‍💻 Auteurs

- 👨‍💻 **Oussama Nouhar** – [oussamanouhar0@gmail.com](mailto:oussamanouhar0@gmail.com)  
- 👩‍💻 **Omaima Siaf** – [siafomaima5@gmail.com](mailto:siafomaima5@gmail.com)

---
## 📂 Clonage et exécution

```bash
git clone https://github.com/Oussamaroom67/ColocationAppBackend.git
cd ColocationAppBackend

# Modifier les informations de connexion dans appsettings.json

dotnet restore
dotnet ef database update
dotnet run
```
