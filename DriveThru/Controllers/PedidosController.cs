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
        private static List<Pedidos> retirar = new List<Pedidos>();

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
        public async Task<ActionResult<Pedidos>> AlterarPedido(int senha, [FromBody] eOrigemPedido origemPedido)
        {
            var oldPedido = pedidos.Find(a => a.Senha == senha);

            if (oldPedido == null) return NotFound();

            var pedido = oldPedido;

            if (origemPedido is eOrigemPedido.DriveThru || origemPedido is eOrigemPedido.Balcao || origemPedido is eOrigemPedido.Delivery)
            {


                pedido.OrigemPedido = origemPedido;
                pedido.StatusPedido = eStatusPedido.Alterado;

                pedidos.Remove(oldPedido);
                pedidos.Add(pedido);
                return Ok(pedido);
            }
            return BadRequest();
        }

        [HttpPatch("fazer")]
        public async Task<ActionResult<List<Pedidos>>> FazerPedido()
        {

            var count = fazendo.Count;
            if (count == 3) return BadRequest();
            foreach (var pedido in pedidos)
            {
                if (pedido.StatusPedido == eStatusPedido.Aguardando || pedido.StatusPedido == eStatusPedido.Alterado)
                {
                    pedido.StatusPedido = eStatusPedido.Fazendo;
                    fazendo.Add(pedido);
                    return Ok(fazendo);
                }
            }
            return NotFound();
        }

        [HttpPatch("finalizar")]
        public async Task<ActionResult> FinalizarPedido()
        {
            if (fazendo.Count == 0) return NotFound();
            var fazendoAux = new List<Pedidos>();
            foreach (var pedido in fazendo)
            {
                if (pedido.OrigemPedido == eOrigemPedido.Delivery)
                {
                    pedidos.Find(a => a.Senha == pedido.Senha).StatusPedido = eStatusPedido.Finalizado;
                    finalizado.Add(pedido);
                    fazendoAux.Remove(pedido);
                }
                else
                {
                    pedidos.Find(a => a.Senha == pedido.Senha).StatusPedido = eStatusPedido.Pronto;
                    retirar.Add(pedido);
                    fazendoAux.Remove(pedido);
                }
            }
            fazendo = fazendoAux;
            return Ok("Todos os pedidos foram finalizados.");
        }

        [HttpPatch("entregar")]
        public async Task<ActionResult<List<Pedidos>>> EntregaPedido()
        {
            List<Pedidos> entregando = new List<Pedidos>();
            List<Pedidos> finalizadoAux = new List<Pedidos>();
            foreach (var pedido in finalizado)
            {
                pedido.StatusPedido = eStatusPedido.Pronto;
                entregando.Add(pedido);

                if (entregando.Count == 3)
                {
                    foreach (var item in entregando)
                    {
                        finalizado.Remove(item);
                        pedidos.Find(a => a.Senha == item.Senha).StatusPedido = eStatusPedido.Pronto;
                    }

                    return Ok(entregando);
                }
            }
            return NotFound();
        }

        [HttpDelete("{senha}")]
        public async Task<ActionResult<List<Pedidos>>> RetirarPedido(int senha)
        {

            var pedido = retirar.Find(a => a.Senha == senha);

            if (pedido == null) return NotFound();

            retirar.Remove(pedido);

            return Ok($"Pedido {senha} entregue.");

        }
    }
}