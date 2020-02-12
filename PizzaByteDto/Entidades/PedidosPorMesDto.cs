namespace PizzaByteDto.Entidades
{
    public class PedidosPorMesDto
    {
        /// <summary>
        /// Representa o mês (Jan, Fev, Mar, Abr, Mai, Jun, Jul, Ago, Set, Out, Nov, Dez)
        /// </summary>
        public string Mes { get; set; }

        /// <summary>
        /// Quantidade de pedidos
        /// </summary>
        public int Pedidos { get; set; }
        
    }
}
