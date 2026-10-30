namespace Animed.Application.Dtos
{
    public class GetConsultaDto
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string? Diagnostico { get; set; }
        public string? Prescricao { get; set; }
        public int PontosGanhos { get; set; }
    }
}
