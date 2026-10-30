namespace Animed.Application.Dtos
{
    public class GetDescontoDto
    {
        public string Nivel { get; set; } = string.Empty;
        public decimal ValorBruto { get; set; }
        public decimal Moedas { get; set; }
        public decimal ValorFinal { get; set; }
        public decimal Desconto { get; set; }
    }
}
