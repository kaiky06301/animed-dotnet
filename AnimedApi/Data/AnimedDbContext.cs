using AnimedApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimedApi.Data;

/// <summary>
/// Contexto do EF Core. Mapeia as entidades para as tabelas TB_* do schema ANIMED,
/// que ja existe no Oracle (criado pelas migrations Flyway da API Java).
/// Por isso o EF apenas mapeia o que existe: nao ha migrations do EF neste projeto.
/// </summary>
public class AnimedDbContext : DbContext
{
    public AnimedDbContext(DbContextOptions<AnimedDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tutor> Tutores => Set<Tutor>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Consulta> Consultas => Set<Consulta>();
    public DbSet<Vacina> Vacinas => Set<Vacina>();

    /// <summary>
    /// Configura o mapeamento entidade -> tabela/coluna. As tabelas e colunas usam
    /// os nomes reais do schema (TB_TUTOR, NOME, ...). Validacao de entrada mora nos
    /// DTOs, nao aqui: as constraints ja existem no banco e o EF nao as recria.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tutor>(entity =>
        {
            entity.ToTable("TB_TUTOR");
            entity.HasKey(t => t.Id);

            // ID_TUTOR e GENERATED ALWAYS AS IDENTITY: o EF nao envia valor no INSERT,
            // apenas le o id gerado de volta.
            entity.Property(t => t.Id).HasColumnName("ID_TUTOR").ValueGeneratedOnAdd();
            entity.Property(t => t.Nome).HasColumnName("NOME");
            entity.Property(t => t.Email).HasColumnName("EMAIL");
            entity.Property(t => t.Cpf).HasColumnName("CPF");
            entity.Property(t => t.Telefone).HasColumnName("TELEFONE");
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.ToTable("TB_PET");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id).HasColumnName("ID_PET").ValueGeneratedOnAdd();
            entity.Property(p => p.Nome).HasColumnName("NOME");
            entity.Property(p => p.Especie).HasColumnName("ESPECIE");
            entity.Property(p => p.Raca).HasColumnName("RACA");
            entity.Property(p => p.Peso).HasColumnName("PESO_KG").HasPrecision(5, 2);
            entity.Property(p => p.TutorId).HasColumnName("ID_TUTOR");

            entity.HasOne(p => p.Tutor)
                  .WithMany(t => t.Pets)
                  .HasForeignKey(p => p.TutorId);
        });

        modelBuilder.Entity<Consulta>(entity =>
        {
            entity.ToTable("TB_CONSULTA");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id).HasColumnName("ID_CONSULTA").ValueGeneratedOnAdd();
            entity.Property(c => c.DataHora).HasColumnName("DATA_HORA");
            entity.Property(c => c.Motivo).HasColumnName("MOTIVO");
            entity.Property(c => c.Diagnostico).HasColumnName("DIAGNOSTICO");
            entity.Property(c => c.Prescricao).HasColumnName("PRESCRICAO");
            entity.Property(c => c.PetId).HasColumnName("ID_PET");

            // A consulta liga a PET (nao a tutor). Filtrar por tutor passa pelo pet.
            entity.HasOne(c => c.Pet)
                  .WithMany(p => p.Consultas)
                  .HasForeignKey(c => c.PetId);
        });

        modelBuilder.Entity<Vacina>(entity =>
        {
            entity.ToTable("TB_VACINA");
            entity.HasKey(v => v.Id);

            entity.Property(v => v.Id).HasColumnName("ID_VACINA").ValueGeneratedOnAdd();
            entity.Property(v => v.Nome).HasColumnName("NOME_VACINA");
            entity.Property(v => v.DataAplicacao).HasColumnName("DATA_APLICACAO");
            entity.Property(v => v.Observacoes).HasColumnName("OBSERVACOES");
            entity.Property(v => v.PetId).HasColumnName("ID_PET");

            entity.HasOne(v => v.Pet)
                  .WithMany(p => p.Vacinas)
                  .HasForeignKey(v => v.PetId);
        });
    }
}
