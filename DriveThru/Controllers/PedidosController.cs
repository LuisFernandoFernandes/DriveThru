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
        private static List<Pedidos> fazendo = new List<Pedidos>();
        private static List<Pedidos> finalizado = new List<Pedidos>();

        private static int countDtforBalcao = 0;
        private static int countDtforDelivery = 0;
        private static int countBalcaforDelivery = 0;

        [HttpPost]
        public async Task<ActionResult<Pedidos>> RealizarPedido(eOrigemPedido origemPedido)
        {
            if (countDtforBalcao == 2 && origemPedido != eOrigemPedido.Balcao) return BadRequest();
            if ((countDtforDelivery == 3 || countBalcaforDelivery == 2) && origemPedido != eOrigemPedido.Delivery) return BadRequest();
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

            Pedidos pedido = new Pedidos
            {
                Senha = pedidos.Count,
                OrigemPedido = origemPedido,
                StatusPedido = eStatusPedido.Aguardando
            };

            pedidos.Add(pedido);
            return Ok(pedido);
        }

        [HttpPatch("{senha}")]
        public async Task<ActionResult<Pedidos>> AlterarPedido(int senha, [FromBody] Pedidos pedido)
        {
            var oldPedido = pedidos.Find(a => a.Senha == senha);

            if (oldPedido == null) return BadRequest();

            pedidos.Remove(oldPedido);

            pedido.StatusPedido = eStatusPedido.Alterado;

            pedidos.Add(pedido);
            return Ok(pedido);
        }

        [HttpPatch("fazer")]
        public async Task<ActionResult<List<Pedidos>>> FazerPedido()
        {

            var count = 0;
            foreach (var pedido in pedidos)
            {

                if (count == 3) return BadRequest();

                if (pedido.StatusPedido == eStatusPedido.Aguardando || pedido.StatusPedido == eStatusPedido.Alterado)
                {
                    pedido.StatusPedido = eStatusPedido.Fazendo;
                    fazendo.Add(pedido);
                    return Ok(fazendo);
                }
                if (pedido.StatusPedido == eStatusPedido.Fazendo)
                {

                    fazendo.Add(pedido);

                    count++;
                }

            }
            return NotFound();
        }

        [HttpPatch("finalizar")]
        public async Task<ActionResult<List<Pedidos>>> FinalizarPedido()
        {
            if (fazendo.Count == 0) return NotFound();
            foreach (var pedido in fazendo)
            {
                var pedidosAux = pedidos.Find(a => a.Senha == pedido.Senha);
                pedidosAux.StatusPedido = eStatusPedido.Finalizado;
                fazendo.Remove(pedido);
                finalizado.Add(pedidosAux);
            }
            return Ok(finalizado);
        }

        [HttpPatch("entregar")]
        public async Task<ActionResult<List<Pedidos>>> EntregaPedido()
        {
            var count = 0;
            List<Pedidos> entregando = new List<Pedidos>();
            List<Pedidos> finalizadoAux = finalizado;
            foreach (var pedido in finalizadoAux)
            {
                if (pedido.OrigemPedido != eOrigemPedido.Delivery) continue;

                entregando.Add(pedido);
                finalizadoAux.Remove(pedido);
                count++;

                if (count == 3)
                {

                    finalizado = finalizadoAux;
                    //foreach (var item in collection)
                    //{

                    //}

                    return Ok(entregando);
                }
            }
            return NotFound();
        }
    }
}