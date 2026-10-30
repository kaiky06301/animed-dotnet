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
    public class TutorUseCase : ITutorUseCase
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IEnderecoService _enderecoService;
        private readonly ILogger<TutorUseCase> _logger;

        public TutorUseCase(ITutorRepository tutorRepository, IEnderecoService enderecoService, ILogger<TutorUseCase> logger)
        {
            _tutorRepository = tutorRepository;
            _enderecoService = enderecoService;
            _logger = logger;
        }

        private static List<LinkDto> LinksDoTutor(int id)
        {
            return new List<LinkDto>
            {
                new LinkDto($"/api/tutor/{id}", "get_um_tutor", "GET"),
                new LinkDto($"/api/tutor/{id}", "update_tutor", "PUT"),
                new LinkDto($"/api/tutor/{id}", "delete_tutor", "DELETE"),
                new LinkDto($"/api/pet?TutorId={id}", "pets_do_tutor", "GET"),
            };
        }

        public OperationResult<GetTutorDto?> AdicionarTutor(PostTutorDto entity)
        {
            try
            {
                var result = _tutorRepository.Adicionar(entity.ToTutorEntity());

                _logger.LogInformation("Tutor {Id} cadastrado", result?.Id);

                return OperationResult<GetTutorDto?>.Success(result?.ToTutorDto(LinksDoTutor(result.Id)), (int)HttpStatusCode.Created);
            }
            catch (RegraDeNegocioException ex)
            {
                return OperationResult<GetTutorDto?>.Failure(ex.Message, (int)HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetTutorDto?>.Failure("Ocorreu um erro ao salvar o tutor", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<GetTutorDto?> DeletarTutor(int Id)
        {
            try
            {
                var result = _tutorRepository.Deletar(Id);

                return OperationResult<GetTutorDto?>.Success(result?.ToTutorDto([]), (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                return OperationResult<GetTutorDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetTutorDto?>.Failure("Ocorreu um erro ao deletar o tutor", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<GetTutorDto?> EditarTutor(int Id, PostTutorDto entity)
        {
            try
            {
                var result = _tutorRepository.Editar(Id, entity.ToTutorEntity());

                return OperationResult<GetTutorDto?>.Success(result?.ToTutorDto(LinksDoTutor(Id)), (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                return OperationResult<GetTutorDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (RegraDeNegocioException ex)
            {
                return OperationResult<GetTutorDto?>.Failure(ex.Message, (int)HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetTutorDto?>.Failure("Ocorreu um erro ao editar o tutor", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<PageResponseDto<GetTutorDto>> ObterTodosTutores(FiltroDto filtro, string? Nivel)
        {
            try
            {
                _logger.LogInformation("ObterTodosTutores iniciado na application");
                var result = _tutorRepository.ObterTodos(filtro.Deslocamento, filtro.RegistroRetornado, filtro.Nome, Nivel, filtro.OrdenarPor, filtro.Decrescente);

                var proximo = filtro.Deslocamento + filtro.RegistroRetornado;

                var links = new List<LinkDto>
                {
                    new LinkDto($"/api/tutor?Deslocamento={filtro.Deslocamento}&RegistroRetornado={filtro.RegistroRetornado}", "self", "GET"),
                    new LinkDto("/api/tutor", "create_tutor", "POST")
                };

                if (proximo < result.rowsCount)
                    links.Add(new LinkDto($"/api/tutor?Deslocamento={proximo}&RegistroRetornado={filtro.RegistroRetornado}", "next_page", "GET"));

                if (filtro.Deslocamento > 0)
                    links.Add(new LinkDto($"/api/tutor?Deslocamento={Math.Max(0, filtro.Deslocamento - filtro.RegistroRetornado)}&RegistroRetornado={filtro.RegistroRetornado}", "previous_page", "GET"));

                var pageResponse = new PageResponseDto<GetTutorDto>
                {
                    Items = result.data.Select(item => item.ToTutorDto(LinksDoTutor(item.Id))),
                    Deslocamento = filtro.Deslocamento,
                    RegistroRetornado = filtro.RegistroRetornado,
                    TotalRegistros = result.rowsCount,
                    Links = links
                };

                return OperationResult<PageResponseDto<GetTutorDto>>.Success(pageResponse, (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                _logger.LogWarning("Nao retornou nenhuma informação do ObterTodosTutores - Parametros: D: {0} - R: {1}", filtro.Deslocamento, filtro.RegistroRetornado);
                return OperationResult<PageResponseDto<GetTutorDto>>.Failure(ex.Message, (int)HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<PageResponseDto<GetTutorDto>>.Failure("Ocorreu um erro ao buscar todos os tutores", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<GetTutorDto?> ObterUmTutor(int Id)
        {
            try
            {
                var result = _tutorRepository.ObterUm(Id);

                return OperationResult<GetTutorDto?>.Success(result?.ToTutorDto(LinksDoTutor(Id)), (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                _logger.LogWarning("Nao retornou nenhuma informação do ObterUmTutor - Parametros: ID: {0}", Id);
                return OperationResult<GetTutorDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetTutorDto?>.Failure("Ocorreu um erro ao obter um tutor", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<OperationResult<EnderecoModel?>> ObterEnderecoPorCepAsync(string cep)
        {
            try
            {
                var result = await _enderecoService.ObterEnderecoPorCepAsync(cep);

                return OperationResult<EnderecoModel?>.Success(result, (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                return OperationResult<EnderecoModel?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<EnderecoModel?>.Failure("Ocorreu um erro ao buscar o cep", (int)HttpStatusCode.BadRequest);
            }
        }
    }
}
