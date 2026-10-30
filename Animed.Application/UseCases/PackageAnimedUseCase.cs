using Animed.Application.Dtos;
using Animed.Application.Interfaces;
using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Animed.Application.UseCases
{
    public class PackageAnimedUseCase : IPackageAnimedUseCase
    {
        private readonly IPackageAnimedRepository _packageRepository;
        private readonly ILogger<PackageAnimedUseCase> _logger;

        public PackageAnimedUseCase(IPackageAnimedRepository packageRepository, ILogger<PackageAnimedUseCase> logger)
        {
            _packageRepository = packageRepository;
            _logger = logger;
        }

        public OperationResult<VacinaPackageModel?> RegistrarVacina(PostVacinaPackageDto entity)
        {
            try
            {
                var result = _packageRepository.RegistrarVacina(entity.PetId, entity.NomeVacina, entity.DataAplicacao, entity.DataProximaDose, entity.Veterinario);

                _logger.LogInformation("PRC_REGISTRAR_VACINA executada: vacina {Id}, +{Pontos} pontos", result.IdVacina, result.PontosCreditados);

                return OperationResult<VacinaPackageModel?>.Success(result, (int)HttpStatusCode.Created);
            }
            catch (NoContentException ex)
            {
                return OperationResult<VacinaPackageModel?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (RegraDeNegocioException ex)
            {
                return OperationResult<VacinaPackageModel?>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<VacinaPackageModel?>.Failure("Ocorreu um erro ao executar a procedure", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<GetDescontoDto?> CalcularDesconto(string nivel, decimal valorBruto, decimal moedas)
        {
            try
            {
                var valorFinal = _packageRepository.CalcularDesconto(nivel, valorBruto, moedas)!.Value;

                return OperationResult<GetDescontoDto?>.Success(new GetDescontoDto
                {
                    Nivel = nivel.ToUpper(),
                    ValorBruto = valorBruto,
                    Moedas = moedas,
                    ValorFinal = valorFinal,
                    Desconto = valorBruto - valorFinal
                }, (int)HttpStatusCode.OK);
            }
            catch (RegraDeNegocioException ex)
            {
                return OperationResult<GetDescontoDto?>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetDescontoDto?>.Failure("Ocorreu um erro ao calcular o desconto", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<string?> NivelPorPontos(int pontos)
        {
            try
            {
                return OperationResult<string?>.Success(_packageRepository.NivelPorPontos(pontos), (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<string?>.Failure("Ocorreu um erro ao calcular o nivel", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<string?> ExportarDatasetJson()
        {
            try
            {
                return OperationResult<string?>.Success(_packageRepository.ExportarDatasetJson(), (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                return OperationResult<string?>.Failure(ex.Message, (int)HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<string?>.Failure("Ocorreu um erro ao exportar o dataset", (int)HttpStatusCode.BadRequest);
            }
        }
    }
}
