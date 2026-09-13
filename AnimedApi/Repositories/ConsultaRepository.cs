using AnimedApi.Data;
using AnimedApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimedApi.Repositories;

/// <summary>Acesso a dados de Consulta (TB_CONSULTA).</summary>
public class ConsultaRepository : IConsultaRepository
{
    private readonly AnimedDbContext _context;

    public ConsultaRepository(AnimedDbContext context) => _context = context;

    /// <summary>
    /// Lista consultas com filtros opcionais. tutorId filtra pelo tutor do pet
    /// (a consulta nao aponta para tutor); data filtra por dia inteiro.
    /// </summary>
    public async Task<List<Consulta>> ListarAsync(long? tutorId, long? petId, DateTime? data)
    {
        IQueryable<Consulta> query = _context.Consultas.AsNoTracking();

        if (petId is not null)
            query = query.Where(c => c.PetId == petId);

        if (tutorId is not null)
            query = query.Where(c => c.Pet!.TutorId == tutorId);

        if (data is not null)
        {
            // Intervalo [inicio, fim) em vez de comparar .Date: traduz para SQL sem depender
            // de funcao de data especifica do provider.
            DateTime inicio = data.Value.Date;
            DateTime fim = inicio.AddDays(1);
            query = query.Where(c => c.DataHora >= inicio && c.DataHora < fim);
        }

        return await query.ToListAsync();
    }

    public async Task<Consulta?> BuscarPorIdAsync(long id) =>
        await _context.Consultas.FirstOrDefaultAsync(c => c.Id == id);

    public async Task AdicionarAsync(Consulta consulta)
    {
        await _context.Consultas.AddAsync(consulta);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Consulta consulta)
    {
        _context.Consultas.Update(consulta);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Consulta consulta)
    {
        _context.Consultas.Remove(consulta);
        await _context.SaveChangesAsync();
    }
}
