using PizzaByteDto.Entidades;
using System.Collections.Generic;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Classe que leva as informações a serem exibidas no dashboard
    /// </summary>
    public class RetornoObterInformacoesDashboardDto : RetornoDto
    {
        public RetornoObterInformacoesDashboardDto()
        {
            ListaProdutosVendidosPorDiaSemana = new List<ProdutosVendidosPorDiaSemanaDto>();
            ListaPedidosPorMes = new List<PedidosPorMesDto>();
        }

        /// <summary>
        /// Número de clientes cadastrados no mês
        /// </summary>
        public int QuantidadeNovosClientes { get; set; }

        /// <summary>
        /// Quantidade de pedidos do mês
        /// </summary>
        public int QuantidadePedidos { get; set; }

        /// <summary>
        /// Quantidade de pedidos que foram cancelados
        /// </summary>
        public int QuantidadePedidosCancelados { get; set; }

        /// <summary>
        /// Percentual das contas do mês que estão quitadas
        /// </summary>
        public double PercentualContasQuitadas { get; set; }

        /// <summary>
        /// Percentual dos pedidos para entrega da semana
        /// </summary>
        public double PercentualPedidosEntregaSemana { get; set; }

        /// <summary>
        /// Percentual dos pedidos para retirada da semana
        /// </summary>
        public double PercentualPedidosRetiradaSemana { get; set; }

        /// <summary>
        /// Percentual dos pedidos feitos no balcão na ultima semana
        /// </summary>
        public double PercentualPedidosBalcaoSemana { get; set; }

        /// <summary>
        /// Resumo dos produtos vendidos nos dias da semana
        /// </summary>
        public List<ProdutosVendidosPorDiaSemanaDto> ListaProdutosVendidosPorDiaSemana { get; set; }

        /// <summary>
        /// Pedidos feitos a cada mês do ano corrente
        /// </summary>
        public List<PedidosPorMesDto> ListaPedidosPorMes { get; set; }
    }
}
