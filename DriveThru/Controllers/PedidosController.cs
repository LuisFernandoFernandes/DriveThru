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
            try
            {
                return Ok(_service.Visualizar(_pedidos));
            }
            catch (ArgumentNullException)
            {
                return NotFound("Nenhum pedido encontrado");
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }

        }

        [HttpPost]
        public ActionResult<Pedido> RealizarPedido(string origemPedido)
        {
            try
            {
                return Ok(_service.Realizar(_pedidos, origemPedido));
            }
            catch (ArgumentException)
            {
                return BadRequest("Escolha como seu pedido será entregue.");
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }

        [HttpPatch("{senha}")]
        public ActionResult<Pedido> AlterarPedido(int senha)
        {
            try
            {
                return Ok(_service.Alterar(_pedidos, senha));
            }
            catch (ArgumentNullException)
            {

                return NotFound("Nenhum pedido corresponde a senha informada.");
            }
            catch (ArgumentException)
            {
                return BadRequest("Esse pedido já não pode mais ser alterado.");
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }

        [HttpPatch("fazer")]
        public ActionResult<List<Pedido>> FazerPedido()
        {

            try
            {
                return Ok(_service.Fazer(_pedidos, _fazendo));
            }
            catch (ArgumentNullException)
            {
                return NotFound("Nenhum pedido está aguardando preparo.");
            }
            catch (ArgumentException)
            {
                return BadRequest("A cozinha já está preparando o maxímo de pedidos possível.");
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }


        [HttpPatch("finalizar")]
        public ActionResult FinalizarPedido()
        {
            try
            {
                return Ok(_service.Finalizar(_fazendo, _finalizado, _pedidos));
            }
            catch (ArgumentNullException)
            {
                return NotFound("Não há pedidos para finalizar.");
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }

        [HttpDelete("entregar")]
        public ActionResult<List<Pedido>> EntregaPedido()
        {
            try
            {
                return Ok(_service.Entregar(_finalizado, _pedidos));
            }
            catch (ArgumentNullException)
            {
                return NotFound("Não há pedidos prontos para serem entreges.");
            }
            catch (ArgumentException)
            {
                return BadRequest("Não há pedidos prontos o suficiente para realizar a entrega.");
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }

        [HttpDelete("{senha}")]
        public ActionResult<List<Pedido>> RetirarPedido(int senha)
        {
            try
            {
                return Ok(_service.Retirar(senha, _finalizado, _pedidos));
            }
            catch (ArgumentNullException)
            {
                return NotFound("A senha informada não corresponde a de um pedido pronto para retirada.");
            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }

        }
    }
}