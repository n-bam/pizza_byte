using PizzaByteDto.Base;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.Entidades
{
    public class PedidoResumidoDto : BaseEntidadeDto
    {
        /// <summary>
        /// Indica se é entrega, retirada ou balcão
        /// </summary>
        public TipoPedido Tipo { get; set; }

        /// <summary>
        /// Valor total do pedido
        /// </summary>
        public float Total { get; set; }

        /// <summary>
        /// Valor de troco necessário 
        /// </summary>
        public float Troco { get; set; }

        /// <summary>
        /// Valor da taxa de entrega de acordo com o bairro
        /// </summary>
        public float TaxaEntrega { get; set; }

        /// <summary>
        /// Valor recebido em dinheiro
        /// </summary>
        public float RecebidoDinheiro { get; set; }

        /// <summary>
        /// Valor recebido em cartão de crédito
        /// </summary>
        public float RecebidoCredito { get; set; }

        /// <summary>
        /// Valor recebido em cartão de débito
        /// </summary>
        public float RecebidoDebito { get; set; }

        /// <summary>
        /// Indica se o pedido foi registrado pelo IFood
        /// </summary>
        public bool PedidoIfood { get; set; }

        /// <summary>
        /// Nome do cliente do pedido
        /// </summary>
        public string NomeCliente { get; set; }
    }
}
