using Animed.Domain.Entities;

namespace Animed.Domain.Interfaces
{
    public interface IPetRepository
    {
        (IEnumerable<PetEntity> data, int rowsCount) ObterTodos(int Deslocamento, int RegistroRetornado, int? TutorId, string? Especie, string? Nome, string? OrdenarPor, bool Decrescente);
        PetEntity? ObterUm(int Id);
        PetEntity? Adicionar(PetEntity entity);
        PetEntity? Editar(int Id, PetEntity entity);
        PetEntity? Deletar(int Id);

        VacinaEntity? AdicionarVacina(VacinaEntity entity, int petId);
        VacinaEntity? DeletarVacina(int vacinaId, int petId);
    }
}
