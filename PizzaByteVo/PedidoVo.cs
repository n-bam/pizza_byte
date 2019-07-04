using System;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteVo
{
    /// <summary>
    /// Classe que representa os campos de um pedido no banco de dados
    /// </summary>
    public class PedidoVo : EntidadeBaseVo
    {
        /// <summary>
        /// Indica se é entrega, retirada ou balcão
        /// MIN: 0
        /// </summary>
        public TipoPedido Tipo { get; set; }

        /// <summary>
        /// Valor total do pedido
        /// MIN: 0 
        /// </summary>
        public float Total { get; set; }

        /// <summary>
        /// Valor de troco necessário 
        /// </summary>
        public float Troco { get; set; }

        /// <summary>
        /// Valor da taxa de entrega de acordo com o bairro
        /// MIN: 0 
        /// </summary>
        public float TaxaEntrega { get; set; }

        /// <summary>
        /// Valor recebido em dinheiro
        /// </summary>
        public float RecebidoDinheiro { get; set; }

        /// <summary>
        /// Valor recebido em cartão
        /// </summary>
        public float RecebidoCartao { get; set; }

        /// <summary>
        /// Observações gerais do pedido
        /// </summary>
        public string Obs { get; set; }

        /// <summary>
        /// Identificação do cliente no qual o pedido pertence
        /// </summary>
        public Guid IdCliente { get; set; }
    }
}
