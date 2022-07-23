using DriveThru.Enums;
using DriveThru.Models;
using Microsoft.AspNetCore.Mvc;

namespace DriveThru.Controllers
{
    [Route("api/pedidos")]
    [ApiController]
    public class PedidosController : ControllerBase
    {

        private static List<Pedido> _pedidos = new List<Pedido>();
        private static List<Pedido> _fazendo = new List<Pedido>();
        private static List<Pedido> _finalizado = new List<Pedido>();


        private static int _countDtforBalcao = 0;
        private static int _countDtforDelivery = 0;
        private static int _countBalcaforDelivery = 0;


        [HttpGet]
        public ActionResult<List<Pedido>> RealizarPedido()
        {
            try
            {
                if (_pedidos.Count() == 0)
                {
                    return NotFound("Nenhum pedido encontrado");
                }
                else
                {
                    return Ok(_pedidos);
                }

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
                var values = Enum.GetValues(typeof(eOrigemPedido));
                int numberEnumOrigem;
                var result = int.TryParse(origemPedido, out numberEnumOrigem);
                if (result)
                {
                    if (values.Length <= numberEnumOrigem || numberEnumOrigem < 0)
                    {
                        return NotFound("Escolha uma forma válida de entrega.");
                    }
                }
                else
                {
                    var upper = origemPedido.ToUpper();
                    numberEnumOrigem = values.Length;
                    foreach (eOrigemPedido item in values)
                    {
                        var description = EnumDescription.GetEnumDescription(item).ToUpper();
                        if (upper == description || upper == "BALCAO")
                        {
                            numberEnumOrigem = (int)item;
                            break;
                        }
                    }
                }

                if (numberEnumOrigem == values.Length)
                {
                    return NotFound("Escolha uma forma válida de entrega.");
                }


                if (Enum.IsDefined(typeof(eOrigemPedido), (eOrigemPedido)numberEnumOrigem))
                {
                    Pedido pedido = new Pedido
                    {
                        Senha = _pedidos.Count + 1,
                        OrigemPedido = (eOrigemPedido)numberEnumOrigem,
                        StatusPedido = eStatusPedido.Aguardando
                    };

                    _pedidos.Add(pedido);
                    return Ok(pedido);
                }
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
                var oldPedido = _pedidos.Find(a => a.Senha == senha);

                if (oldPedido == null) return NotFound("Nenhum pedido corresponde a senha informada.");

                var pedido = oldPedido;

                pedido.StatusPedido = eStatusPedido.Alterado;

                _pedidos.Remove(oldPedido);
                _pedidos.Add(pedido);
                return Ok(pedido);
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


                var count = _fazendo.Count;
                if (count == 3) return BadRequest("A cozinha já está preparando o maxímo de pedidos possível.");
                foreach (var pedido in _pedidos)
                {
                    if (pedido.StatusPedido == eStatusPedido.Aguardando || pedido.StatusPedido == eStatusPedido.Alterado)
                    {

                        if (_countDtforBalcao == 2 && pedido.OrigemPedido != eOrigemPedido.Balcao) continue;
                        else if ((_countDtforDelivery == 3 || _countBalcaforDelivery == 2) && pedido.OrigemPedido != eOrigemPedido.Delivery) continue;
                        else
                        {
                            if (pedido.OrigemPedido == eOrigemPedido.DriveThru)
                            {
                                _countDtforBalcao++;
                                _countDtforDelivery++;
                            }
                            else if (pedido.OrigemPedido == eOrigemPedido.Balcao)
                            {
                                _countDtforBalcao = 0;
                                _countBalcaforDelivery++;
                            }
                            else
                            {
                                _countDtforDelivery = 0;
                                _countBalcaforDelivery = 0;
                            }
                        }

                        pedido.StatusPedido = eStatusPedido.Fazendo;
                        _fazendo.Add(pedido);
                        return Ok(_fazendo);
                    }
                    continue;
                }
                return NotFound();
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

                if (_fazendo.Count == 0) return NotFound("Todos os pedidos em preparo já foram finalizados.");

                var pedido = _fazendo.FirstOrDefault();
                _fazendo.Remove(pedido);
                pedido.StatusPedido = eStatusPedido.Pronto;
                _finalizado.Add(pedido);

                return Ok("Todos os pedidos foram finalizados.");

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
                List<Pedido> entregando = new List<Pedido>();
                List<Pedido> finalizadoAux = new List<Pedido>();
                foreach (var pedido in _finalizado)
                {
                    pedido.StatusPedido = eStatusPedido.Pronto;
                    entregando.Add(pedido);

                    if (entregando.Count == 3)
                    {
                        foreach (var item in entregando)
                        {
                            _finalizado.Remove(item);
                            _pedidos.Find(a => a.Senha == item.Senha).StatusPedido = eStatusPedido.Entregue;
                        }

                        return Ok(entregando);
                    }
                }
                return NotFound("Não há pedidos prontos o suficiente para realizar a entrega.");
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

                var pedido = _finalizado.Find(a => a.Senha == senha && a.OrigemPedido != eOrigemPedido.Delivery);

                if (pedido == null) return NotFound("A senha informada não corresponde a de um pedido pronto para retirada.");

                _finalizado.Remove(pedido);

                return Ok($"Pedido {senha} entregue.");

            }
            catch (Exception)
            {
                return Problem("Algo deu errado, contate o administrador.");
            }
        }
    }
}