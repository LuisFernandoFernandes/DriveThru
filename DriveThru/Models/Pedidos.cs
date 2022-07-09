using DriveThru.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriveThru.Models
{
    public class Pedidos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Senha { get; set; }
        public eOrigemPedido OrigemPedido { get; set; }
        public eStatusPedido StatusPedido { get; set; }
    }
}
