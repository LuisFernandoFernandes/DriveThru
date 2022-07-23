using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

namespace DriveThru.Enums
{
    public enum eOrigemPedido
    {
        [Description("DriveThru")]
        DriveThru = 0,

        [Description("Balcão")]
        Balcao = 1,

        [Description("Delivery")]
        Delivery = 2
    }
}
