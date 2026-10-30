using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Animed.Domain.Entities
{
    /// <summary>
    /// Linha do tempo de cuidados de um pet, guardada no MongoDB.
    /// Cada ação (vacina, consulta, pesagem...) vira um evento embutido no documento.
    /// </summary>
    [Table("historico_cuidado")]
    public class HistoricoCuidadoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public int PetId { get; set; }
        public int TutorId { get; set; }
        public string NomePet { get; set; } = string.Empty;

        public ICollection<EventoCuidadoEntity> Eventos { get; set; } = [];
    }
}
