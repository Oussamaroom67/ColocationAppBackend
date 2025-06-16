
using ColocationAppBackend.BL;
using ColocationAppBackend.Data;
using ColocationAppBackend.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ColocationAppBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<ProprietaireManager>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<AnnonceFilterService>();
            builder.Services.AddScoped<FavoriService>();
            builder.Services.AddScoped<AnnonceService>();
            builder.Services.AddScoped<DashboardService>();
            builder.Services.AddScoped<AnalytiquesService>();
            builder.Services.AddScoped<UtilisateurService>();
            builder.Services.AddScoped<LogementService>();
            //recommendation des colocations 
            builder.Services.AddScoped<RecommendationManager>();
            //signal r pour les messages en temps réel
            builder.Services.AddSignalR();
            builder.Services.AddScoped<IMessageService, MessageService>();
            //signalement service
            builder.Services.AddScoped<SignalementService>();


            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });
            builder.Services.AddControllers()
            .AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve);
            builder.Services.AddScoped<ColocationManager>();


            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,  // Tu peux activer plus tard si tu veux
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });


            var app = builder.Build();
            
            app.MapHub<ChatHub>("/chathub");

            app.UseAuthentication();
            app.UseMiddleware<UserSuspensionMiddleware>();
            app.UseAuthorization();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
