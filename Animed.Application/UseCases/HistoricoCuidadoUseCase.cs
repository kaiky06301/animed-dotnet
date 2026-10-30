using Animed.Application.Dtos;
using Animed.Application.Interfaces;
using Animed.Application.Mappers;
using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Animed.Application.UseCases
{
    public class HistoricoCuidadoUseCase : IHistoricoCuidadoUseCase
    {
        private readonly IHistoricoCuidadoRepository _historicoRepository;
        private readonly IPetRepository _petRepository;
        private readonly ILogger<HistoricoCuidadoUseCase> _logger;

        public HistoricoCuidadoUseCase(IHistoricoCuidadoRepository historicoRepository, IPetRepository petRepository, ILogger<HistoricoCuidadoUseCase> logger)
        {
            _historicoRepository = historicoRepository;
            _petRepository = petRepository;
            _logger = logger;
        }

        private static List<LinkDto> LinksDoHistorico(string id, int petId)
        {
            return new List<LinkDto>
            {
                new LinkDto($"/api/historico/{id}", "get_um_historico", "GET"),
                new LinkDto($"/api/historico/pet/{petId}/evento", "registrar_evento", "POST"),
                new LinkDto($"/api/historico/{id}", "delete_historico", "DELETE"),
                new LinkDto($"/api/pet/{petId}", "pet", "GET"),
            };
        }

        public OperationResult<PageResponseDto<GetHistoricoCuidadoDto>> ObterTodosHistoricos(int Deslocamento, int RegistroRetornado, int? TutorId)
        {
            try
            {
                var result = _historicoRepository.ObterTodos(Deslocamento, RegistroRetornado, TutorId);

                var pageResponse = new PageResponseDto<GetHistoricoCuidadoDto>
                {
                    Items = result.data.Select(item => item.ToHistoricoDto(LinksDoHistorico(item.Id.ToString(), item.PetId))),
                    Deslocamento = Deslocamento,
                    RegistroRetornado = RegistroRetornado,
                    TotalRegistros = result.rowsCount,
                    Links = new List<LinkDto>
                    {
                        new LinkDto($"/api/historico?Deslocamento={Deslocamento}&RegistroRetornado={RegistroRetornado}", "self", "GET"),
                    }
                };

                return OperationResult<PageResponseDto<GetHistoricoCuidadoDto>>.Success(pageResponse, (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                return OperationResult<PageResponseDto<GetHistoricoCuidadoDto>>.Failure(ex.Message, (int)HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<PageResponseDto<GetHistoricoCuidadoDto>>.Failure("Ocorreu um erro ao buscar os históricos", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<GetHistoricoCuidadoDto?> ObterUmHistorico(string Id)
        {
            try
            {
                var result = _historicoRepository.ObterUm(Id);

                return OperationResult<GetHistoricoCuidadoDto?>.Success(result?.ToHistoricoDto(LinksDoHistorico(Id, result.PetId)), (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                return OperationResult<GetHistoricoCuidadoDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetHistoricoCuidadoDto?>.Failure("Ocorreu um erro ao obter o histórico", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<GetHistoricoCuidadoDto?> ObterHistoricoDoPet(int petId)
        {
            try
            {
                var result = _historicoRepository.ObterPorPet(petId);

                return OperationResult<GetHistoricoCuidadoDto?>.Success(result?.ToHistoricoDto(LinksDoHistorico(result.Id.ToString(), petId)), (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                return OperationResult<GetHistoricoCuidadoDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetHistoricoCuidadoDto?>.Failure("Ocorreu um erro ao obter o histórico do pet", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<GetHistoricoCuidadoDto?> RegistrarEvento(int petId, PostEventoCuidadoDto entity)
        {
            try
            {
                var pet = _petRepository.ObterUm(petId);

                var result = _historicoRepository.RegistrarEvento(pet!.Id, pet.TutorId, pet.Nome, entity.ToEventoEntity());

                return OperationResult<GetHistoricoCuidadoDto?>.Success(result?.ToHistoricoDto(LinksDoHistorico(result.Id.ToString(), petId)), (int)HttpStatusCode.Created);
            }
            catch (NoContentException ex)
            {
                return OperationResult<GetHistoricoCuidadoDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetHistoricoCuidadoDto?>.Failure("Ocorreu um erro ao registrar o evento", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<GetHistoricoCuidadoDto?> DeletarHistorico(string Id)
        {
            try
            {
                var result = _historicoRepository.Deletar(Id);

                return OperationResult<GetHistoricoCuidadoDto?>.Success(result?.ToHistoricoDto([]), (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                return OperationResult<GetHistoricoCuidadoDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetHistoricoCuidadoDto?>.Failure("Ocorreu um erro ao deletar o histórico", (int)HttpStatusCode.BadRequest);
            }
        }
    }
}
