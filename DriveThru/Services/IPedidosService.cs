using DriveThru.Models;

namespace DriveThru.Services
{
    public interface IPedidosService
    {
        public List<Pedido> Visualizar(List<Pedido> pedidos);
        public Pedido Realizar(List<Pedido> pedidos, string origemPedido);
        public Pedido Alterar(List<Pedido> pedidos, int senha);
        public List<Pedido> Fazer(List<Pedido> pedidos, List<Pedido> fazendo);
        public string Finalizar(List<Pedido> fazendo, List<Pedido> finalizado, List<Pedido> pedidos);
        public List<Pedido> Entregar(List<Pedido> finalizado, List<Pedido> pedidos);
        public string Retirar(int senha, List<Pedido> finalizado, List<Pedido> pedidos);
    }
}