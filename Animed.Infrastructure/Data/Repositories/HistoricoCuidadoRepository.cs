using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Domain.Interfaces;
using MongoDB.Bson;

namespace Animed.Infrastructure.Data.Repositories
{
    public class HistoricoCuidadoRepository : IHistoricoCuidadoRepository
    {
        private readonly MongoApplicationContext _context;

        public HistoricoCuidadoRepository(MongoApplicationContext context)
        {
            _context = context;
        }

        public HistoricoCuidadoEntity? Adicionar(HistoricoCuidadoEntity entity)
        {
            try
            {
                _context.HistoricoCuidado.Add(entity);
                _context.SaveChanges();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public HistoricoCuidadoEntity? RegistrarEvento(int petId, int tutorId, string nomePet, EventoCuidadoEntity evento)
        {
            try
            {
                var historico = _context
                    .HistoricoCuidado
                    .FirstOrDefault(x => x.PetId == petId);

                //Primeiro evento do pet cria o documento
                if (historico is null)
                {
                    historico = new HistoricoCuidadoEntity
                    {
                        PetId = petId,
                        TutorId = tutorId,
                        NomePet = nomePet,
                        Eventos = [evento]
                    };

                    _context.HistoricoCuidado.Add(historico);
                }
                else
                {
                    historico.Eventos ??= [];
                    historico.Eventos.Add(evento);
                }

                _context.SaveChanges();

                return historico;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public HistoricoCuidadoEntity? Deletar(string Id)
        {
            try
            {
                var historico = _context
                    .HistoricoCuidado
                    .FirstOrDefault(x => x.Id == ObjectId.Parse(Id));

                if (historico is null) throw new NoContentException("Não foi encontrado o histórico");

                _context.HistoricoCuidado.Remove(historico);
                _context.SaveChanges();

                return historico;
            }
            catch (NoContentException ex)
            {
                throw new NoContentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public (IEnumerable<HistoricoCuidadoEntity> data, int rowsCount) ObterTodos(int Deslocamento, int RegistroRetornado, int? TutorId)
        {
            try
            {
                if (Deslocamento < 0) Deslocamento = 0;

                if (RegistroRetornado <= 0) RegistroRetornado = 30;

                var consulta = _context.HistoricoCuidado.AsQueryable();

                if (TutorId.HasValue)
                    consulta = consulta.Where(x => x.TutorId == TutorId.Value);

                var totalRegistros = consulta.Count();

                var resultado = consulta
                    .OrderBy(x => x.Id)
                    .Skip(Deslocamento)
                    .Take(RegistroRetornado)
                    .ToList();

                if (!resultado.Any()) throw new NoContentException("Não foi encontrado históricos");

                return (resultado, totalRegistros);
            }
            catch (NoContentException ex)
            {
                throw new NoContentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public HistoricoCuidadoEntity? ObterPorPet(int petId)
        {
            try
            {
                var historico = _context
                    .HistoricoCuidado
                    .FirstOrDefault(x => x.PetId == petId);

                if (historico is null) throw new NoContentException("Não foi encontrado histórico para este pet");

                return historico;
            }
            catch (NoContentException ex)
            {
                throw new NoContentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public HistoricoCuidadoEntity? ObterUm(string Id)
        {
            try
            {
                if (!ObjectId.TryParse(Id, out var objectId))
                    throw new NoContentException("Id do histórico inválido");

                var historico = _context
                    .HistoricoCuidado
                    .FirstOrDefault(x => x.Id == objectId);

                if (historico is null) throw new NoContentException("Não foi encontrado o histórico");

                return historico;
            }
            catch (NoContentException ex)
            {
                throw new NoContentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
