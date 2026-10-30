namespace Animed.Domain.Errors
{
    /// <summary>
    /// Lançada quando a operação fere uma regra do Animed
    /// (ex.: e-mail já cadastrado, vacina com data no futuro).
    /// </summary>
    public class RegraDeNegocioException : Exception
    {
        public RegraDeNegocioException()
        {

        }

        public RegraDeNegocioException(string? message) : base(message)
        {

        }
    }
}
