namespace Animed.Application.Dtos
{
    public class GetTutorDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string? Cep { get; set; }
        public int PontosTotais { get; set; }
        public string Nivel { get; set; } = string.Empty;
        public int QuantidadePets { get; set; }

        public List<LinkDto> Links { get; set; } = new();
    }
}
