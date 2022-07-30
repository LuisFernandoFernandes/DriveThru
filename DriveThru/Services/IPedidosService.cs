using DriveThru.Models;

namespace DriveThru.Services
{
    public interface IPedidosService
    {
        public Tuple<int, object> Visualizar(List<Pedido> pedidos);
        public Tuple<int, object> Realizar(List<Pedido> pedidos, string origemPedido);
        public Tuple<int, object> Alterar(List<Pedido> pedidos, int senha);
        public Tuple<int, object> Fazer(List<Pedido> pedidos, List<Pedido> fazendo);
        public Tuple<int, object> Finalizar(List<Pedido> fazendo, List<Pedido> finalizado, List<Pedido> pedidos);
        public Tuple<int, object> Entregar(List<Pedido> finalizado, List<Pedido> pedidos);
        public Tuple<int, object> Retirar(int senha, List<Pedido> finalizado, List<Pedido> pedidos);
    }
}