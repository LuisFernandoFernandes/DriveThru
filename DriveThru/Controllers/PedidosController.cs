using DriveThru.Enums;
using DriveThru.Models;
using DriveThru.Services;
using Microsoft.AspNetCore.Mvc;

namespace DriveThru.Controllers
{
    [Route("api/pedidos")]
    [ApiController]
    public class PedidosController : ControllerBase
    {

        private readonly IPedidosService _service;

        private static List<Pedido> _pedidos = new List<Pedido>();
        private static List<Pedido> _fazendo = new List<Pedido>();
        private static List<Pedido> _finalizado = new List<Pedido>();

        public PedidosController()
        {
            _service = new PedidosService();
        }

        [HttpGet]
        public ActionResult<List<Pedido>> VisualizarPedidos()
        {

            var result = _service.Visualizar(_pedidos);
            return StatusCode(result.Item1, result.Item2);
        }

        [HttpPost]
        public ActionResult<Pedido> RealizarPedido(string origemPedido)
        {

            var result = _service.Realizar(_pedidos, origemPedido);
            return StatusCode(result.Item1, result.Item2);
        }

        [HttpPatch("{senha}")]
        public ActionResult<Pedido> AlterarPedido(int senha)
        {

            var result = _service.Alterar(_pedidos, senha);
            return StatusCode(result.Item1, result.Item2);
        }

        [HttpPatch("fazer")]
        public ActionResult<List<Pedido>> FazerPedido()
        {

            var result = _service.Fazer(_pedidos, _fazendo);
            return StatusCode(result.Item1, result.Item2);
        }


        [HttpPatch("finalizar")]
        public ActionResult FinalizarPedido()
        {

            var result = _service.Finalizar(_fazendo, _finalizado, _pedidos);
            return StatusCode(result.Item1, result.Item2);
        }

        [HttpDelete("entregar")]
        public ActionResult<List<Pedido>> EntregaPedido()
        {

            var result = _service.Entregar(_finalizado, _pedidos);
            return StatusCode(result.Item1, result.Item2);
        }

        [HttpDelete("{senha}")]
        public ActionResult<List<Pedido>> RetirarPedido(int senha)
        {

            var result = _service.Retirar(senha, _finalizado, _pedidos);
            return StatusCode(result.Item1, result.Item2);
        }
    }
}