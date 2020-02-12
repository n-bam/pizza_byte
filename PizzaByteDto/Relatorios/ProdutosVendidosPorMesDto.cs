using PizzaByteDto.Base;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Dados para a evolução mensal de contas
    /// </summary>
    public class ProdutosVendidosPorMesDto : BaseEntidadeDto
    {
        /// <summary>
        /// Mês do pedido
        /// </summary>
        public string Mes { get; set; }

        /// <summary>
        /// Total de pizza no mês
        /// </summary>
        public int Pizza { get; set; }

        /// <summary>
        /// Total de bebidas no mês
        /// </summary>
        public int Bebida { get; set; }
    }
}
