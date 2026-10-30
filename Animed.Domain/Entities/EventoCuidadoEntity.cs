namespace Animed.Domain.Entities
{
    public class EventoCuidadoEntity
    {
        /// <summary>VACINA, CONSULTA, PESAGEM, MEDICAMENTO</summary>
        public string Tipo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int Pontos { get; set; }
        public DateTime Data { get; set; } = DateTime.UtcNow;
    }
}
