using Animed.Domain.Entities;

namespace Animed.Domain.Interfaces
{
    public interface IEnderecoService
    {
        Task<EnderecoModel?> ObterEnderecoPorCepAsync(string cep);
    }
}
