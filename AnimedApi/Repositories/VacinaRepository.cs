using AnimedApi.Data;
using AnimedApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimedApi.Repositories;

/// <summary>Acesso a dados de Vacina (TB_VACINA).</summary>
public class VacinaRepository : IVacinaRepository
{
    private readonly AnimedDbContext _context;

    public VacinaRepository(AnimedDbContext context) => _context = context;

    /// <summary>Lista vacinas; se petId vier, filtra pelas vacinas daquele pet.</summary>
    public async Task<List<Vacina>> ListarAsync(long? petId)
    {
        IQueryable<Vacina> query = _context.Vacinas.AsNoTracking();

        if (petId is not null)
            query = query.Where(v => v.PetId == petId);

        return await query.ToListAsync();
    }

    public async Task<Vacina?> BuscarPorIdAsync(long id) =>
        await _context.Vacinas.FirstOrDefaultAsync(v => v.Id == id);

    public async Task AdicionarAsync(Vacina vacina)
    {
        await _context.Vacinas.AddAsync(vacina);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Vacina vacina)
    {
        _context.Vacinas.Update(vacina);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Vacina vacina)
    {
        _context.Vacinas.Remove(vacina);
        await _context.SaveChangesAsync();
    }
}
