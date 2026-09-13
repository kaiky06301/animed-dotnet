using AnimedApi.Data;
using AnimedApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimedApi.Repositories;

/// <summary>Acesso a dados de Pet (TB_PET).</summary>
public class PetRepository : IPetRepository
{
    private readonly AnimedDbContext _context;

    public PetRepository(AnimedDbContext context) => _context = context;

    /// <summary>Lista pets; se tutorId vier, filtra pelos pets daquele tutor.</summary>
    public async Task<List<Pet>> ListarAsync(long? tutorId)
    {
        IQueryable<Pet> query = _context.Pets.AsNoTracking();

        if (tutorId is not null)
            query = query.Where(p => p.TutorId == tutorId);

        return await query.ToListAsync();
    }

    public async Task<Pet?> BuscarPorIdAsync(long id) =>
        await _context.Pets.FirstOrDefaultAsync(p => p.Id == id);

    /// <summary>Confirma se um pet existe (usado antes de vincular consulta/vacina).</summary>
    public async Task<bool> ExisteAsync(long id) =>
        await _context.Pets.AnyAsync(p => p.Id == id);

    public async Task AdicionarAsync(Pet pet)
    {
        await _context.Pets.AddAsync(pet);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Pet pet)
    {
        _context.Pets.Update(pet);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Pet pet)
    {
        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync();
    }
}
