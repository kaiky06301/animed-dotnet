using AnimedApi.Data;
using AnimedApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimedApi.Repositories;

/// <summary>Acesso a dados de Tutor (TB_TUTOR).</summary>
public class TutorRepository : ITutorRepository
{
    private readonly AnimedDbContext _context;

    public TutorRepository(AnimedDbContext context) => _context = context;

    /// <summary>Lista todos os tutores (sem tracking: leitura pura).</summary>
    public async Task<List<Tutor>> ListarAsync() =>
        await _context.Tutores.AsNoTracking().ToListAsync();

    /// <summary>Busca por id; devolve null se nao existir. Rastreado, para permitir update/delete.</summary>
    public async Task<Tutor?> BuscarPorIdAsync(long id) =>
        await _context.Tutores.FirstOrDefaultAsync(t => t.Id == id);

    /// <summary>Insere e persiste. O id e preenchido pelo banco apos o SaveChanges.</summary>
    public async Task AdicionarAsync(Tutor tutor)
    {
        await _context.Tutores.AddAsync(tutor);
        await _context.SaveChangesAsync();
    }

    /// <summary>Persiste alteracoes da entidade rastreada.</summary>
    public async Task AtualizarAsync(Tutor tutor)
    {
        _context.Tutores.Update(tutor);
        await _context.SaveChangesAsync();
    }

    /// <summary>Remove e persiste.</summary>
    public async Task RemoverAsync(Tutor tutor)
    {
        _context.Tutores.Remove(tutor);
        await _context.SaveChangesAsync();
    }
}
