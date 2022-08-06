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


        public List<Pedido> Visualizar(List<Pedido> pedidos)
        {
            if (pedidos.Count() == 0)
            {
                throw new ArgumentNullException();
            }
            else
            {
                return pedidos;
            }
        }

        public Pedido Realizar(List<Pedido> pedidos, string origemPedido)
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
                return pedido;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public Pedido Alterar(List<Pedido> pedidos, int senha)
        {

            var oldPedido = pedidos.Find(a => a.Senha == senha);

            if (oldPedido == null)
            {
                throw new ArgumentNullException();
            }
            if (oldPedido.StatusPedido != eStatusPedido.Aguardando)
            {
                throw new ArgumentException();
            }

            var pedido = oldPedido;

            pedido.StatusPedido = eStatusPedido.Alterado;

            pedidos.Remove(oldPedido);
            pedidos.Add(pedido);
            return pedido;
        }

        public List<Pedido> Fazer(List<Pedido> pedidos, List<Pedido> fazendo)
        {

            if (pedidos.Count(a => a.StatusPedido == eStatusPedido.Alterado) == 0 && pedidos.Count(a => a.StatusPedido == eStatusPedido.Aguardando) == 0)
            {
                throw new ArgumentNullException();
            }

            var count = fazendo.Count;

            if (count == 3)
            {
                throw new ArgumentException();
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
                    return fazendo;
                }
                continue;
            }
            throw new KeyNotFoundException();
        }

        public string Finalizar(List<Pedido> fazendo, List<Pedido> finalizado, List<Pedido> pedidos)
        {
            if (fazendo.Count == 0)
            {
                throw new ArgumentNullException();
            }

            var pedido = fazendo.FirstOrDefault();
            fazendo.Remove(pedido);
            pedido.StatusPedido = eStatusPedido.Pronto;
            pedidos.Find(a => a.Senha == pedido.Senha).StatusPedido = eStatusPedido.Pronto;
            finalizado.Add(pedido);

            return $"Pedido {pedido.Senha} foi finalizados.";
        }

        public List<Pedido> Entregar(List<Pedido> finalizado, List<Pedido> pedidos)
        {

            if (finalizado.Count(a => a.OrigemPedido == eOrigemPedido.Delivery) == 0)
            {
                throw new ArgumentNullException();
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
                        return entregando;
                    }
                }

            }
            throw new ArgumentException();
        }

        public string Retirar(int senha, List<Pedido> finalizado, List<Pedido> pedidos)
        {

            var pedido = finalizado.Find(a => a.Senha == senha && a.OrigemPedido != eOrigemPedido.Delivery);

            if (pedido == null)
            {
                throw new ArgumentNullException();
            }

            pedidos.Find(a => a.Senha == senha && a.OrigemPedido != eOrigemPedido.Delivery).StatusPedido = eStatusPedido.Entregue;
            finalizado.Remove(pedido);
            return $"Pedido {senha} entregue.";
        }
    }
}

