using DriveThru.Enums;
using DriveThru.Models;
using Microsoft.AspNetCore.Mvc;

namespace DriveThru.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {

        private static List<Pedidos> pedidos = new List<Pedidos>();

        private static int countDtforBalcao = 0;
        private static int countDtforDelivery = 0;
        private static int countBalcaforDelivery = 0;

        //private static int countPedidoDt = 0;
        //private static int countPedidoDt = 0;
        //private static int countPedidoBalcao = 0;

        [HttpPost]
        public async Task<ActionResult<Pedidos>> RealizarPedido(eOrigemPedido origemPedido)
        {
            //a cada dois pedidos do dt um do balcao precisa ser atendido


            if (countDtforBalcao == 2 && origemPedido != eOrigemPedido.Balcao) return BadRequest();
            if (countDtforDelivery == 3 || countBalcaforDelivery == 2 && origemPedido != eOrigemPedido.Delivery) return BadRequest();
            else
            {
                if (origemPedido == eOrigemPedido.DriveThru)
                {
                    countDtforBalcao++;
                    countDtforDelivery++;
                }
                else if (origemPedido == eOrigemPedido.Balcao)
                {
                    countDtforBalcao = 0;
                    countBalcaforDelivery++;
                }
                else
                {
                    countDtforDelivery = 0;
                    countBalcaforDelivery = 0;
                }



            }


            //if (countPedidoDt == 2 && countPedidoBalcao == 0 && origemPedido != eOrigemPedido.Balcao) return BadRequest();
            //else if (countPedidoDt == 3 || countPedidoBalcao == 2 && origemPedido != eOrigemPedido.Delivery) return BadRequest();

            //if (origemPedido == eOrigemPedido.DriveThru) countPedidoDt++;
            //else if (origemPedido == eOrigemPedido.Balcao) countPedidoBalcao++;
            //else
            //{
            //    countPedidoDt = 0;
            //    countPedidoBalcao = 0;
            //}

            Pedidos pedido = new Pedidos
            {
                Senha = pedidos.Count,
                OrigemPedido = origemPedido,
                StatusPedido = eStatusPedido.Aguardando
            };

            pedidos.Add(pedido);
            return Ok(pedido);
        }

        [HttpPatch]
        public async Task<ActionResult<Pedidos>> AlterarPedido(int senha, [FromBody] Pedidos pedido)
        {
            var oldPedido = pedidos.Find(a => a.Senha == senha);

            if (oldPedido == null) return BadRequest();

            pedidos.Remove(oldPedido);

            pedidos.Add(pedido);
            return Ok(pedido);
        }

    }
}
