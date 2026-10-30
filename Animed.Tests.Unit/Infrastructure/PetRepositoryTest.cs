using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Infrastructure.Data;
using Animed.Infrastructure.Data.Repositories;
using Animed.Tests.Unit.Fixtures;

namespace Animed.Tests.Unit.Infrastructure
{
    [Collection("BancoEmMemoria")]
    public class PetRepositoryTest
    {
        private readonly ApplicationContext _context;
        private readonly PetRepository _petRepository;
        private readonly int _tutorId;

        public PetRepositoryTest(BancoEmMemoriaFixture fixture)
        {
            _context = fixture.NovoContexto();
            _petRepository = new PetRepository(_context);

            var tutor = new TutorEntity { Nome = "Marina Oliveira", Email = "marina@email.com" };
            _context.Tutor.Add(tutor);
            _context.SaveChanges();
            _tutorId = tutor.Id;

            _context.Pet.AddRange(
                new PetEntity { Nome = "Thor", Especie = "CACHORRO", PesoKg = 28, TutorId = _tutorId },
                new PetEntity { Nome = "Mia", Especie = "GATO", PesoKg = 4, TutorId = _tutorId }
            );
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Repository", "Pet")]
        public void ObterTodos_FiltroPorEspecie_RetornaSoGatos()
        {
            // Act
            var resultado = _petRepository.ObterTodos(0, 10, null, "gato", null, null, false);

            // Assert
            Assert.Equal(1, resultado.rowsCount);
            Assert.Equal("Mia", resultado.data.Single().Nome);
        }

        [Fact]
        [Trait("Repository", "Pet")]
        public void Adicionar_TutorInexistente_LancaNoContentException()
        {
            // Arrange
            var pet = new PetEntity { Nome = "Rex", Especie = "CACHORRO", TutorId = 999 };

            // Act / Assert
            Assert.Throws<NoContentException>(() => _petRepository.Adicionar(pet));
        }

        [Fact]
        [Trait("Repository", "Pet")]
        public void AdicionarVacina_PetExiste_VacinaApareceNoPet()
        {
            // Arrange
            var thor = _context.Pet.Single(x => x.Nome == "Thor");

            // Act
            _petRepository.AdicionarVacina(new VacinaEntity { NomeVacina = "V10", DataAplicacao = DateTime.Today }, thor.Id);

            // Assert
            var pet = _petRepository.ObterUm(thor.Id);
            Assert.Single(pet!.Vacinas!);
            Assert.Equal("V10", pet.Vacinas!.First().NomeVacina);
        }
    }
}
