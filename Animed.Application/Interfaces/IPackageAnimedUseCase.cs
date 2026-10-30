using Animed.Application.Dtos;
using Animed.Domain.Entities;

namespace Animed.Application.Interfaces
{
    public interface IPackageAnimedUseCase
    {
        OperationResult<VacinaPackageModel?> RegistrarVacina(PostVacinaPackageDto entity);
        OperationResult<GetDescontoDto?> CalcularDesconto(string nivel, decimal valorBruto, decimal moedas);
        OperationResult<string?> NivelPorPontos(int pontos);
        OperationResult<string?> ExportarDatasetJson();
    }
}
