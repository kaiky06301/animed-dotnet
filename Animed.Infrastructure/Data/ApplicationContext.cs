using Animed.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Animed.Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

        public DbSet<UserEntity> Usuario { get; set; }

        public DbSet<TutorEntity> Tutor { get; set; }

        public DbSet<PetEntity> Pet { get; set; }

        public DbSet<VacinaEntity> Vacina { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Usuarios de demonstracao (um por perfil)
            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity { Id = 1, Nome = "Administrador", Email = "admin@animed.com.br", Password = "animed123", Role = "admin" },
                new UserEntity { Id = 2, Nome = "Dra. Helena Prado", Email = "doutor@animed.com.br", Password = "animed123", Role = "veterinario" },
                new UserEntity { Id = 3, Nome = "Tutor Demonstração", Email = "tutor@animed.com.br", Password = "animed123", Role = "tutor" }
            );
        }
    }
}
