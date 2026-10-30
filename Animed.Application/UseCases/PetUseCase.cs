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
    public class PetUseCase : IPetUseCase
    {
        private readonly IPetRepository _petRepository;
        private readonly ITutorRepository _tutorRepository;
        private readonly IHistoricoCuidadoRepository _historicoRepository;
        private readonly ILogger<PetUseCase> _logger;

        public PetUseCase(IPetRepository petRepository,
            ITutorRepository tutorRepository,
            IHistoricoCuidadoRepository historicoRepository,
            ILogger<PetUseCase> logger)
        {
            _petRepository = petRepository;
            _tutorRepository = tutorRepository;
            _historicoRepository = historicoRepository;
            _logger = logger;
        }

        private static List<LinkDto> LinksDoPet(int id, int tutorId)
        {
            return new List<LinkDto>
            {
                new LinkDto($"/api/pet/{id}", "get_um_pet", "GET"),
                new LinkDto($"/api/pet/{id}", "update_pet", "PUT"),
                new LinkDto($"/api/pet/{id}", "delete_pet", "DELETE"),
                new LinkDto($"/api/pet/{id}/vacina", "registrar_vacina", "POST"),
                new LinkDto($"/api/historico/pet/{id}", "historico_de_cuidados", "GET"),
                new LinkDto($"/api/tutor/{tutorId}", "tutor", "GET"),
            };
        }

        public OperationResult<GetPetDto?> AdicionarPet(PostPetDto entity)
        {
            try
            {
                var result = _petRepository.Adicionar(entity.ToPetEntity());

                return OperationResult<GetPetDto?>.Success(result?.ToPetDto(LinksDoPet(result.Id, result.TutorId)), (int)HttpStatusCode.Created);
            }
            catch (NoContentException ex)
            {
                return OperationResult<GetPetDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetPetDto?>.Failure("Ocorreu um erro ao salvar o pet", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<GetPetDto?> DeletarPet(int Id)
        {
            try
            {
                var result = _petRepository.Deletar(Id);

                return OperationResult<GetPetDto?>.Success(result?.ToPetDto([]), (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                return OperationResult<GetPetDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetPetDto?>.Failure("Ocorreu um erro ao deletar o pet", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<GetPetDto?> EditarPet(int Id, PostPetDto entity)
        {
            try
            {
                var result = _petRepository.Editar(Id, entity.ToPetEntity());

                return OperationResult<GetPetDto?>.Success(result?.ToPetDto(LinksDoPet(Id, result.TutorId)), (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                return OperationResult<GetPetDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetPetDto?>.Failure("Ocorreu um erro ao editar o pet", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<PageResponseDto<GetPetDto>> ObterTodosPets(FiltroDto filtro, int? TutorId, string? Especie)
        {
            try
            {
                _logger.LogInformation("ObterTodosPets iniciado na application");
                var result = _petRepository.ObterTodos(filtro.Deslocamento, filtro.RegistroRetornado, TutorId, Especie, filtro.Nome, filtro.OrdenarPor, filtro.Decrescente);

                var proximo = filtro.Deslocamento + filtro.RegistroRetornado;

                var links = new List<LinkDto>
                {
                    new LinkDto($"/api/pet?Deslocamento={filtro.Deslocamento}&RegistroRetornado={filtro.RegistroRetornado}", "self", "GET"),
                    new LinkDto("/api/pet", "create_pet", "POST")
                };

                if (proximo < result.rowsCount)
                    links.Add(new LinkDto($"/api/pet?Deslocamento={proximo}&RegistroRetornado={filtro.RegistroRetornado}", "next_page", "GET"));

                if (filtro.Deslocamento > 0)
                    links.Add(new LinkDto($"/api/pet?Deslocamento={Math.Max(0, filtro.Deslocamento - filtro.RegistroRetornado)}&RegistroRetornado={filtro.RegistroRetornado}", "previous_page", "GET"));

                var pageResponse = new PageResponseDto<GetPetDto>
                {
                    Items = result.data.Select(item => item.ToPetDto(LinksDoPet(item.Id, item.TutorId))),
                    Deslocamento = filtro.Deslocamento,
                    RegistroRetornado = filtro.RegistroRetornado,
                    TotalRegistros = result.rowsCount,
                    Links = links
                };

                return OperationResult<PageResponseDto<GetPetDto>>.Success(pageResponse, (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                _logger.LogWarning("Nao retornou nenhuma informação do ObterTodosPets - Parametros: D: {0} - R: {1}", filtro.Deslocamento, filtro.RegistroRetornado);
                return OperationResult<PageResponseDto<GetPetDto>>.Failure(ex.Message, (int)HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<PageResponseDto<GetPetDto>>.Failure("Ocorreu um erro ao buscar todos os pets", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<GetPetDto?> ObterUmPet(int Id)
        {
            try
            {
                var result = _petRepository.ObterUm(Id);

                return OperationResult<GetPetDto?>.Success(result?.ToPetDto(LinksDoPet(Id, result.TutorId)), (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                _logger.LogWarning("Nao retornou nenhuma informação do ObterUmPet - Parametros: ID: {0}", Id);
                return OperationResult<GetPetDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetPetDto?>.Failure("Ocorreu um erro ao obter um pet", (int)HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Registra a vacina no Oracle, credita os pontos ao tutor e
        /// guarda o evento na linha do tempo do pet no MongoDB.
        /// </summary>
        public OperationResult<GetVacinaDto?> RegistrarVacina(int petId, PostVacinaDto entity)
        {
            try
            {
                if (entity.DataAplicacao.Date > DateTime.Today)
                    throw new RegraDeNegocioException("A data de aplicação não pode estar no futuro");

                if (entity.DataProximaDose.HasValue && entity.DataProximaDose.Value.Date <= entity.DataAplicacao.Date)
                    throw new RegraDeNegocioException("A próxima dose deve ser depois da aplicação");

                var pet = _petRepository.ObterUm(petId);
                var vacina = _petRepository.AdicionarVacina(entity.ToVacinaEntity(), petId);

                _tutorRepository.AdicionarPontos(pet!.TutorId, VacinaEntity.PontosPorVacina);

                try
                {
                    _historicoRepository.RegistrarEvento(pet.Id, pet.TutorId, pet.Nome, new EventoCuidadoEntity
                    {
                        Tipo = "VACINA",
                        Descricao = $"Vacina {entity.NomeVacina} aplicada",
                        Pontos = VacinaEntity.PontosPorVacina,
                        Data = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    // A vacina já está salva no Oracle; o histórico é complementar
                    _logger.LogWarning("Vacina {Id} salva, mas o histórico no MongoDB falhou: {Erro}", vacina?.Id, ex.Message);
                }

                _logger.LogInformation("Vacina {Vacina} registrada para o pet {PetId}", entity.NomeVacina, petId);

                return OperationResult<GetVacinaDto?>.Success(vacina?.ToVacinaDto(VacinaEntity.PontosPorVacina), (int)HttpStatusCode.Created);
            }
            catch (RegraDeNegocioException ex)
            {
                return OperationResult<GetVacinaDto?>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (NoContentException ex)
            {
                return OperationResult<GetVacinaDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetVacinaDto?>.Failure("Ocorreu um erro ao registrar a vacina", (int)HttpStatusCode.BadRequest);
            }
        }

        public OperationResult<GetVacinaDto?> DeletarVacina(int petId, int vacinaId)
        {
            try
            {
                var result = _petRepository.DeletarVacina(vacinaId, petId);

                return OperationResult<GetVacinaDto?>.Success(result?.ToVacinaDto(), (int)HttpStatusCode.OK);
            }
            catch (NoContentException ex)
            {
                return OperationResult<GetVacinaDto?>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError("[ERROR] - {0}", ex.Message);
                return OperationResult<GetVacinaDto?>.Failure("Ocorreu um erro ao deletar a vacina", (int)HttpStatusCode.BadRequest);
            }
        }
    }
}
