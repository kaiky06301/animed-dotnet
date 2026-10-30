using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Animed.Domain.Entities
{
    [Table("tb_net_usuario")]
    [Index(nameof(Email), IsUnique = true, Name = "IDX_NET_USUARIO_EMAIL")]
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>tutor, veterinario ou admin</summary>
        public string Role { get; set; } = string.Empty;
    }
}
