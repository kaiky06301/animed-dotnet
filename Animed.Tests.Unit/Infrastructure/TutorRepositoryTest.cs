using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Infrastructure.Data;
using Animed.Infrastructure.Data.Repositories;
using Animed.Tests.Unit.Fixtures;

namespace Animed.Tests.Unit.Infrastructure
{
    [Collection("BancoEmMemoria")]
    public class TutorRepositoryTest
    {
        private readonly ApplicationContext _context;
        private readonly TutorRepository _tutorRepository;

        public TutorRepositoryTest(BancoEmMemoriaFixture fixture)
        {
            _context = fixture.NovoContexto();
            _tutorRepository = new TutorRepository(_context);

            _context.Tutor.AddRange(
                new TutorEntity { Nome = "Carlos Mendes", Email = "carlos@email.com", PontosTotais = 720, Nivel = NivelTutor.Premium },
                new TutorEntity { Nome = "Marina Oliveira", Email = "marina@email.com", PontosTotais = 350, Nivel = NivelTutor.Cuidador },
                new TutorEntity { Nome = "Juliana Santos", Email = "juliana@email.com", PontosTotais = 50, Nivel = NivelTutor.Basico }
            );
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Repository", "Tutor")]
        public void ObterTodos_OrdenarPorPontosDecrescente_RetornaMaiorPrimeiro()
        {
            // Act
            var resultado = _tutorRepository.ObterTodos(0, 10, null, null, "pontos", true);

            // Assert
            Assert.Equal(3, resultado.rowsCount);
            Assert.Collection(resultado.data,
                t => Assert.Equal("Carlos Mendes", t.Nome),
                t => Assert.Equal("Marina Oliveira", t.Nome),
                t => Assert.Equal("Juliana Santos", t.Nome));
        }

        [Fact]
        [Trait("Repository", "Tutor")]
        public void ObterTodos_FiltroPorNome_RetornaSoQuemCombina()
        {
            // Act
            var resultado = _tutorRepository.ObterTodos(0, 10, "mari", null, null, false);

            // Assert
            Assert.Equal(1, resultado.rowsCount);
            Assert.Equal("Marina Oliveira", resultado.data.Single().Nome);
        }

        [Fact]
        [Trait("Repository", "Tutor")]
        public void ObterTodos_Paginacao_PulaEPegaCerto()
        {
            // Act
            var resultado = _tutorRepository.ObterTodos(1, 1, null, null, "nome", false);

            // Assert
            Assert.Equal(3, resultado.rowsCount);
            Assert.Equal("Juliana Santos", resultado.data.Single().Nome);
        }

        [Fact]
        [Trait("Repository", "Tutor")]
        public void Adicionar_EmailRepetido_LancaRegraDeNegocioException()
        {
            // Arrange
            var repetido = new TutorEntity { Nome = "Outra Marina", Email = "marina@email.com" };

            // Act / Assert
            Assert.Throws<RegraDeNegocioException>(() => _tutorRepository.Adicionar(repetido));
        }

        [Fact]
        [Trait("Repository", "Tutor")]
        public void AdicionarPontos_TutorBasico_SalvaNovoNivel()
        {
            // Arrange
            var juliana = _context.Tutor.Single(x => x.Email == "juliana@email.com");

            // Act
            _tutorRepository.AdicionarPontos(juliana.Id, 260);

            // Assert
            var noBanco = _context.Tutor.Single(x => x.Id == juliana.Id);
            Assert.Equal(310, noBanco.PontosTotais);
            Assert.Equal(NivelTutor.Cuidador, noBanco.Nivel);
        }

        [Fact]
        [Trait("Repository", "Tutor")]
        public void ObterUm_IdInexistente_LancaNoContentException()
        {
            // Act / Assert
            Assert.Throws<NoContentException>(() => _tutorRepository.ObterUm(999));
        }
    }
}
