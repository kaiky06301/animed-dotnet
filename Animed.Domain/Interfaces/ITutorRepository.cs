using Animed.Domain.Entities;

namespace Animed.Domain.Interfaces
{
    public interface ITutorRepository
    {
        (IEnumerable<TutorEntity> data, int rowsCount) ObterTodos(int Deslocamento, int RegistroRetornado, string? Nome, string? Nivel, string? OrdenarPor, bool Decrescente);
        TutorEntity? ObterUm(int Id);
        TutorEntity? Adicionar(TutorEntity entity);
        TutorEntity? Editar(int Id, TutorEntity entity);
        TutorEntity? Deletar(int Id);
        TutorEntity? AdicionarPontos(int Id, int pontos);
    }
}
