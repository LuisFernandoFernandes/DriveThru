using DriveThru.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DriveThru.Models
{
    public class Pedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Senha { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public eOrigemPedido OrigemPedido { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public eStatusPedido StatusPedido { get; set; }
    }
}
