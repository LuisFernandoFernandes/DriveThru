using DriveThru.Enums;
using DriveThru.Models;
using DriveThru.Services;
using Microsoft.AspNetCore.Mvc;

namespace DriveThru.Services
{
    public class PedidosService : IPedidosService
    {

        private static int _countDtforBalcao = 0;
        private static int _countDtforDelivery = 0;
        private static int _countBalcaforDelivery = 0;


        public Tuple<int, object> Visualizar(List<Pedido> pedidos)
        {
            try
            {
                if (pedidos.Count() == 0)
                {
                    return (StatusCodes.Status404NotFound, "Nenhum pedido encontrado").ToTuple<int, object>();
                }
                else
                {
                    return (StatusCodes.Status200OK, pedidos).ToTuple<int, object>();
                }

            }
            catch (Exception)
            {
                return (StatusCodes.Status500InternalServerError, "Algo deu errado, contate o administrador.").ToTuple<int, object>();
            }
        }

        public Tuple<int, object> Realizar(List<Pedido> pedidos, string origemPedido)
        {
            try
            {
                var result = Enum.TryParse(origemPedido.Replace("ã", "a", true, null), true, out eOrigemPedido origem);

                if (result && (int)origem >= 0 && (int)origem <= 2)
                {
                    Pedido pedido = new Pedido
                    {
                        Senha = pedidos.Count + 1,
                        OrigemPedido = origem,
                        StatusPedido = eStatusPedido.Aguardando
                    };

                    pedidos.Add(pedido);
                    return (StatusCodes.Status200OK, pedido).ToTuple<int, object>();
                }
                return (StatusCodes.Status404NotFound, "Escolha como seu pedido será entregue.").ToTuple<int, object>();

            }
            catch (Exception)
            {
                return (StatusCodes.Status500InternalServerError, "Algo deu errado, contate o administrador.").ToTuple<int, object>();
            }
        }

        public Tuple<int, object> Alterar(List<Pedido> pedidos, int senha)
        {
            try
            {
                var oldPedido = pedidos.Find(a => a.Senha == senha);

                if (oldPedido == null)
                {
                    return (StatusCodes.Status404NotFound, "Nenhum pedido corresponde a senha informada.").ToTuple<int, object>();
                }
                if (oldPedido.StatusPedido != eStatusPedido.Aguardando)
                {
                    return (StatusCodes.Status400BadRequest, "Esse pedido já não pode mais ser alterado.").ToTuple<int, object>();
                }

                var pedido = oldPedido;

                pedido.StatusPedido = eStatusPedido.Alterado;

                pedidos.Remove(oldPedido);
                pedidos.Add(pedido);
                return (StatusCodes.Status200OK, pedido).ToTuple<int, object>();
            }
            catch (Exception)
            {
                return (StatusCodes.Status500InternalServerError, "Algo deu errado, contate o administrador.").ToTuple<int, object>();
            }

        }

        public Tuple<int, object> Fazer(List<Pedido> pedidos, List<Pedido> fazendo)
        {
            try
            {
                if (pedidos.Count(a => a.StatusPedido == eStatusPedido.Alterado) == 0 && pedidos.Count(a => a.StatusPedido == eStatusPedido.Aguardando) == 0)
                {
                    return (StatusCodes.Status404NotFound, "Nenhum pedido está aguardando preparo.").ToTuple<int, object>();
                }

                var count = fazendo.Count;

                if (count == 3)
                {
                    return (StatusCodes.Status400BadRequest, "A cozinha já está preparando o maxímo de pedidos possível.").ToTuple<int, object>();
                }

                foreach (var pedido in pedidos)
                {
                    if (pedido.StatusPedido == eStatusPedido.Aguardando || pedido.StatusPedido == eStatusPedido.Alterado)
                    {

                        if (_countDtforBalcao == 2 && pedido.OrigemPedido != eOrigemPedido.Balcao)
                        {
                            continue;
                        }
                        else if ((_countDtforDelivery == 3 || _countBalcaforDelivery == 2) && pedido.OrigemPedido != eOrigemPedido.Delivery)
                        {
                            continue;
                        }
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
                        fazendo.Add(pedido);
                        return (StatusCodes.Status200OK, fazendo).ToTuple<int, object>();

                    }
                    continue;
                }
                return (StatusCodes.Status404NotFound, "Nenhum pedido está aguardando preparo.").ToTuple<int, object>();
            }
            catch (Exception)
            {
                return (StatusCodes.Status500InternalServerError, "Algo deu errado, contate o administrador.").ToTuple<int, object>();
            }
        }

        public Tuple<int, object> Finalizar(List<Pedido> fazendo, List<Pedido> finalizado, List<Pedido> pedidos)
        {
            try
            {

                if (fazendo.Count == 0)
                {
                    return new Tuple<int, object>(StatusCodes.Status404NotFound, "Não há pedidos para finalizar.");
                }


                var pedido = fazendo.FirstOrDefault();
                fazendo.Remove(pedido);
                pedido.StatusPedido = eStatusPedido.Pronto;
                pedidos.Find(a => a.Senha == pedido.Senha).StatusPedido = eStatusPedido.Pronto;
                finalizado.Add(pedido);

                return new Tuple<int, object>(StatusCodes.Status200OK, $"Pedido {pedido.Senha} foi finalizados.");


            }
            catch (Exception)
            {
                return (StatusCodes.Status500InternalServerError, "Algo deu errado, contate o administrador.").ToTuple<int, object>();
            }
        }

        public Tuple<int, object> Entregar(List<Pedido> finalizado, List<Pedido> pedidos)
        {
            try
            {
                if (finalizado.Count(a => a.OrigemPedido == eOrigemPedido.Delivery) == 0)
                {
                    return (StatusCodes.Status404NotFound, "Não há pedidos prontos para serem entreges.").ToTuple<int, object>();
                }

                List<Pedido> entregando = new List<Pedido>();
                foreach (var pedido in finalizado)
                {
                    if (pedido.OrigemPedido == eOrigemPedido.Delivery)
                    {
                        entregando.Add(pedido);

                        if (entregando.Count == 3)
                        {
                            foreach (var item in entregando)
                            {
                                finalizado.Remove(item);
                                pedidos.Find(a => a.Senha == item.Senha).StatusPedido = eStatusPedido.Entregue;
                            }
                            return (StatusCodes.Status200OK, entregando).ToTuple<int, object>();
                        }
                    }

                }
                return (StatusCodes.Status404NotFound, "Não há pedidos prontos o suficiente para realizar a entrega.").ToTuple<int, object>();
            }
            catch (Exception)
            {
                return (StatusCodes.Status500InternalServerError, "Algo deu errado, contate o administrador.").ToTuple<int, object>();
            }
        }

        public Tuple<int, object> Retirar(int senha, List<Pedido> finalizado, List<Pedido> pedidos)
        {
            try
            {

                var pedido = finalizado.Find(a => a.Senha == senha && a.OrigemPedido != eOrigemPedido.Delivery);

                if (pedido == null)
                {
                    return (StatusCodes.Status404NotFound, "A senha informada não corresponde a de um pedido pronto para retirada.").ToTuple<int, object>();
                }

                pedidos.Find(a => a.Senha == senha && a.OrigemPedido != eOrigemPedido.Delivery).StatusPedido = eStatusPedido.Entregue;
                finalizado.Remove(pedido);
                return (StatusCodes.Status200OK, $"Pedido {senha} entregue.").ToTuple<int, object>();

            }
            catch (Exception)
            {
                return (StatusCodes.Status500InternalServerError, "Algo deu errado, contate o administrador.").ToTuple<int, object>();
            }
        }

    }
}
