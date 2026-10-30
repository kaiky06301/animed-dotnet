using Animed.Domain.Entities;

namespace Animed.Domain.Interfaces
{
    public interface IHistoricoCuidadoRepository
    {
        (IEnumerable<HistoricoCuidadoEntity> data, int rowsCount) ObterTodos(int Deslocamento, int RegistroRetornado, int? TutorId);
        HistoricoCuidadoEntity? ObterUm(string Id);
        HistoricoCuidadoEntity? ObterPorPet(int petId);
        HistoricoCuidadoEntity? Adicionar(HistoricoCuidadoEntity entity);
        HistoricoCuidadoEntity? RegistrarEvento(int petId, int tutorId, string nomePet, EventoCuidadoEntity evento);
        HistoricoCuidadoEntity? Deletar(string Id);
    }
}
