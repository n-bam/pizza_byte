using PizzaByteDto.Base;
using System;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Dados básicos para definir a relação entre contas a pagar e receber
    /// </summary>
    public class PedidosPorBairroDto : BaseEntidadeDto
    {
        /// <summary>
        /// Bairro da entrega
        /// </summary>
        public string Bairro { get; set; }

        /// <summary>
        /// Cidade que o bairro pertence
        /// </summary>
        public string Cidade { get; set; }

        /// <summary>
        /// Valor total da taxa de entrega
        /// </summary>
        public double TaxaEntrega { get; set; }

        /// <summary>
        /// Valor total recebido
        /// </summary>
        public double Valor { get; set; }
    }
}
