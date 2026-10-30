using Animed.Domain.Entities;
using Animed.Domain.Errors;
using Animed.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Animed.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Executa os packages PL/SQL pela mesma conexão Oracle do EF Core.
    /// Os erros de negócio do package (RAISE_APPLICATION_ERROR) viram
    /// os erros customizados da aplicação.
    /// </summary>
    public class PackageAnimedRepository : IPackageAnimedRepository
    {
        private const int PetInexistente = 20010;
        private const int DataInvalida = 20011;

        private readonly ApplicationContext _context;

        public PackageAnimedRepository(ApplicationContext context)
        {
            _context = context;
        }

        private OracleCommand NovoComando(string nome, CommandType tipo)
        {
            var conexao = (OracleConnection)_context.Database.GetDbConnection();
            if (conexao.State != ConnectionState.Open) conexao.Open();

            return new OracleCommand(nome, conexao) { CommandType = tipo, BindByName = true };
        }

        public VacinaPackageModel RegistrarVacina(int petId, string nomeVacina, DateTime aplicacao, DateTime? proximaDose, string? veterinario)
        {
            try
            {
                using var comando = NovoComando("PKG_ANIMED_NEGOCIO.PRC_REGISTRAR_VACINA", CommandType.StoredProcedure);

                comando.Parameters.Add("p_id_pet", OracleDbType.Int32).Value = petId;
                comando.Parameters.Add("p_nome_vacina", OracleDbType.Varchar2).Value = nomeVacina;
                comando.Parameters.Add("p_aplicacao", OracleDbType.Date).Value = aplicacao.Date;
                comando.Parameters.Add("p_proxima", OracleDbType.Date).Value = (object?)proximaDose?.Date ?? DBNull.Value;
                comando.Parameters.Add("p_veterinario", OracleDbType.Varchar2).Value = (object?)veterinario ?? DBNull.Value;
                var idVacina = comando.Parameters.Add("p_id_vacina", OracleDbType.Int32, ParameterDirection.Output);
                var pontos = comando.Parameters.Add("p_pontos", OracleDbType.Int32, ParameterDirection.Output);

                comando.ExecuteNonQuery();

                return new VacinaPackageModel
                {
                    IdVacina = ((OracleDecimal)idVacina.Value).ToInt32(),
                    PontosCreditados = ((OracleDecimal)pontos.Value).ToInt32()
                };
            }
            catch (OracleException ex) when (ex.Number == PetInexistente)
            {
                throw new NoContentException(MensagemDoPackage(ex));
            }
            catch (OracleException ex) when (ex.Number == DataInvalida)
            {
                throw new RegraDeNegocioException(MensagemDoPackage(ex));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public decimal? CalcularDesconto(string nivel, decimal valorBruto, decimal moedas)
        {
            try
            {
                using var comando = NovoComando("SELECT PKG_ANIMED_NEGOCIO.FN_CALCULA_DESCONTO(:p_nivel, :p_valor, :p_moedas) FROM DUAL", CommandType.Text);

                comando.Parameters.Add("p_nivel", OracleDbType.Varchar2).Value = nivel;
                comando.Parameters.Add("p_valor", OracleDbType.Decimal).Value = valorBruto;
                comando.Parameters.Add("p_moedas", OracleDbType.Decimal).Value = moedas;

                var resultado = comando.ExecuteScalar();

                // A função devolve NULL quando o valor é inválido (exceção tratada no package)
                if (resultado is null || resultado == DBNull.Value)
                    throw new RegraDeNegocioException("Valor bruto deve ser maior que zero");

                return Convert.ToDecimal(resultado);
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

        public string NivelPorPontos(int pontos)
        {
            try
            {
                using var comando = NovoComando("SELECT PKG_ANIMED_NEGOCIO.FN_CALCULA_NIVEL(:p_pontos) FROM DUAL", CommandType.Text);
                comando.Parameters.Add("p_pontos", OracleDbType.Int32).Value = pontos;

                return comando.ExecuteScalar()?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public string ExportarDatasetJson()
        {
            try
            {
                using var comando = NovoComando("SELECT PKG_ANIMED_JSON.FN_DATASET_JSON FROM DUAL", CommandType.Text);
                comando.InitialLONGFetchSize = -1;

                using var leitor = comando.ExecuteReader();
                if (!leitor.Read()) throw new NoContentException("O package não devolveu o dataset");

                var json = leitor.GetOracleClob(0).Value;

                if (json == "[]") throw new NoContentException("Nenhum tutor para exportar");

                return json;
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

        // "ORA-20010: Pet 999 nao encontrado.\nORA-06512: ..." -> "Pet 999 nao encontrado."
        private static string MensagemDoPackage(OracleException ex)
        {
            var primeiraLinha = ex.Message.Split('\n')[0];
            var separador = primeiraLinha.IndexOf(": ", StringComparison.Ordinal);
            return separador >= 0 ? primeiraLinha[(separador + 2)..] : primeiraLinha;
        }
    }
}
