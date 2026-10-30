namespace Animed.Application.Dtos
{
    public class GetPetDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty;
        public string? Raca { get; set; }
        public DateTime? DataNascimento { get; set; }
        public double? PesoKg { get; set; }
        public bool Castrado { get; set; }
        public int TutorId { get; set; }

        public IEnumerable<GetVacinaDto> Vacinas { get; set; } = [];

        public List<LinkDto> Links { get; set; } = new();
    }
}
