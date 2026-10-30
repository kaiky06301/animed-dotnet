namespace Animed.Application.Dtos
{
    public class GetVacinaDto
    {
        public int Id { get; set; }
        public string NomeVacina { get; set; } = string.Empty;
        public DateTime DataAplicacao { get; set; }
        public DateTime? DataProximaDose { get; set; }
        public bool Vencida { get; set; }
        public int PontosGanhos { get; set; }
    }
}
