namespace Animed.Application.Dtos
{
    /// <summary>Parâmetros de paginação, filtro e ordenação recebidos na query string.</summary>
    public class FiltroDto
    {
        public int Deslocamento { get; set; } = 0;
        public int RegistroRetornado { get; set; } = 10;
        public string? Nome { get; set; }
        public string? OrdenarPor { get; set; }
        public bool Decrescente { get; set; }
    }
}
