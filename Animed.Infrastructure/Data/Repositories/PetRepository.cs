using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Animed.Infrastructure.Data.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly ApplicationContext _context;

        public PetRepository(ApplicationContext context)
        {
            _context = context;
        }

        public PetEntity? Adicionar(PetEntity entity)
        {
            try
            {
                if (!_context.Tutor.Any(x => x.Id == entity.TutorId))
                    throw new NoContentException("Não foi encontrado o tutor do pet");

                _context.Pet.Add(entity);
                _context.SaveChanges();

                return entity;
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

        public PetEntity? Deletar(int Id)
        {
            try
            {
                var pet = _context.Pet.FirstOrDefault(x => x.Id == Id);

                if (pet is null) throw new NoContentException("Não foi encontrado o pet");

                _context.Pet.Remove(pet);
                _context.SaveChanges();

                return pet;
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

        public PetEntity? Editar(int Id, PetEntity entity)
        {
            try
            {
                var pet = _context.Pet.FirstOrDefault(x => x.Id == Id);

                if (pet is null) throw new NoContentException("Não foi encontrado o pet");

                pet.Nome = entity.Nome;
                pet.Especie = entity.Especie;
                pet.Raca = entity.Raca;
                pet.DataNascimento = entity.DataNascimento;
                pet.PesoKg = entity.PesoKg;
                pet.Castrado = entity.Castrado;

                _context.Pet.Update(pet);
                _context.SaveChanges();

                return pet;
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

        public (IEnumerable<PetEntity> data, int rowsCount) ObterTodos(int Deslocamento, int RegistroRetornado, int? TutorId, string? Especie, string? Nome, string? OrdenarPor, bool Decrescente)
        {
            try
            {
                if (Deslocamento < 0) Deslocamento = 0;

                if (RegistroRetornado <= 0) RegistroRetornado = 30;

                var consulta = _context.Pet.AsQueryable();

                //Filtros
                if (TutorId.HasValue)
                    consulta = consulta.Where(x => x.TutorId == TutorId.Value);

                if (!string.IsNullOrWhiteSpace(Especie))
                    consulta = consulta.Where(x => x.Especie == Especie.ToUpper());

                if (!string.IsNullOrWhiteSpace(Nome))
                    consulta = consulta.Where(x => x.Nome.ToUpper().Contains(Nome.ToUpper()));

                var totalRegistros = consulta.Count();

                //Ordenacao
                consulta = (OrdenarPor?.ToLower()) switch
                {
                    "nome" => Decrescente ? consulta.OrderByDescending(x => x.Nome) : consulta.OrderBy(x => x.Nome),
                    "peso" => Decrescente ? consulta.OrderByDescending(x => x.PesoKg) : consulta.OrderBy(x => x.PesoKg),
                    _ => Decrescente ? consulta.OrderByDescending(x => x.Id) : consulta.OrderBy(x => x.Id),
                };

                var resultado = consulta
                    .Include(x => x.Vacinas)
                    .Skip(Deslocamento)
                    .Take(RegistroRetornado)
                    .ToList();

                if (!resultado.Any()) throw new NoContentException("Não foi encontrado pets");

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

        public PetEntity? ObterUm(int Id)
        {
            try
            {
                var pet = _context
                    .Pet
                    .Include(x => x.Vacinas)
                    .FirstOrDefault(x => x.Id == Id);

                if (pet is null) throw new NoContentException("Não foi encontrado o pet");

                return pet;
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

        public VacinaEntity? AdicionarVacina(VacinaEntity entity, int petId)
        {
            try
            {
                if (!_context.Pet.Any(x => x.Id == petId))
                    throw new NoContentException("Não foi encontrado o pet");

                entity.PetId = petId;

                _context.Vacina.Add(entity);
                _context.SaveChanges();

                return entity;
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

        public VacinaEntity? DeletarVacina(int vacinaId, int petId)
        {
            try
            {
                var vacina = _context.Vacina.FirstOrDefault(x => x.Id == vacinaId && x.PetId == petId);

                if (vacina is null) throw new NoContentException("Não foi encontrada a vacina");

                _context.Vacina.Remove(vacina);
                _context.SaveChanges();

                return vacina;
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
