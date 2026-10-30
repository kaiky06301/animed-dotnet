using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Animed.Infrastructure.Data.Repositories
{
    public class TutorRepository : ITutorRepository
    {
        private readonly ApplicationContext _context;

        public TutorRepository(ApplicationContext context)
        {
            _context = context;
        }

        public TutorEntity? Adicionar(TutorEntity entity)
        {
            try
            {
                if (_context.Tutor.Any(x => x.Email == entity.Email))
                    throw new RegraDeNegocioException("Já existe um tutor com este e-mail");

                entity.PontosTotais = 0;
                entity.Nivel = NivelTutor.Basico;

                _context.Tutor.Add(entity);
                _context.SaveChanges();

                return entity;
            }
            catch (RegraDeNegocioException ex)
            {
                throw new RegraDeNegocioException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public TutorEntity? Deletar(int Id)
        {
            try
            {
                var tutor = _context.Tutor.FirstOrDefault(x => x.Id == Id);

                if (tutor is null) throw new NoContentException("Não foi encontrado o tutor");

                _context.Tutor.Remove(tutor);
                _context.SaveChanges();

                return tutor;
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

        public TutorEntity? Editar(int Id, TutorEntity entity)
        {
            try
            {
                var tutor = _context.Tutor.FirstOrDefault(x => x.Id == Id);

                if (tutor is null) throw new NoContentException("Não foi encontrado o tutor");

                if (_context.Tutor.Any(x => x.Email == entity.Email && x.Id != Id))
                    throw new RegraDeNegocioException("Já existe um tutor com este e-mail");

                tutor.Nome = entity.Nome;
                tutor.Email = entity.Email;
                tutor.Cpf = entity.Cpf;
                tutor.Telefone = entity.Telefone;
                tutor.Cep = entity.Cep;

                _context.Tutor.Update(tutor);
                _context.SaveChanges();

                return tutor;
            }
            catch (NoContentException ex)
            {
                throw new NoContentException(ex.Message);
            }
            catch (RegraDeNegocioException ex)
            {
                throw new RegraDeNegocioException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public (IEnumerable<TutorEntity> data, int rowsCount) ObterTodos(int Deslocamento, int RegistroRetornado, string? Nome, string? Nivel, string? OrdenarPor, bool Decrescente)
        {
            try
            {
                if (Deslocamento < 0) Deslocamento = 0;

                if (RegistroRetornado <= 0) RegistroRetornado = 30;

                var consulta = _context.Tutor.AsQueryable();

                //Filtros
                if (!string.IsNullOrWhiteSpace(Nome))
                    consulta = consulta.Where(x => x.Nome.ToUpper().Contains(Nome.ToUpper()));

                if (!string.IsNullOrWhiteSpace(Nivel))
                    consulta = consulta.Where(x => x.Nivel == Nivel.ToUpper());

                var totalRegistros = consulta.Count();

                //Ordenacao
                consulta = (OrdenarPor?.ToLower()) switch
                {
                    "nome" => Decrescente ? consulta.OrderByDescending(x => x.Nome) : consulta.OrderBy(x => x.Nome),
                    "pontos" => Decrescente ? consulta.OrderByDescending(x => x.PontosTotais) : consulta.OrderBy(x => x.PontosTotais),
                    _ => Decrescente ? consulta.OrderByDescending(x => x.Id) : consulta.OrderBy(x => x.Id),
                };

                var resultado = consulta
                    .Include(x => x.Pets)
                    .Skip(Deslocamento)
                    .Take(RegistroRetornado)
                    .ToList();

                if (!resultado.Any()) throw new NoContentException("Não foi encontrado tutores");

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

        public TutorEntity? ObterUm(int Id)
        {
            try
            {
                var tutor = _context
                    .Tutor
                    .Include(x => x.Pets)
                    .FirstOrDefault(x => x.Id == Id);

                if (tutor is null) throw new NoContentException("Não foi encontrado o tutor");

                return tutor;
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

        public TutorEntity? AdicionarPontos(int Id, int pontos)
        {
            try
            {
                var tutor = _context.Tutor.FirstOrDefault(x => x.Id == Id);

                if (tutor is null) throw new NoContentException("Não foi encontrado o tutor");

                tutor.AdicionarPontos(pontos);

                _context.Tutor.Update(tutor);
                _context.SaveChanges();

                return tutor;
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
