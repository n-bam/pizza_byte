using PizzaByteDto.Base;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Dados básicos para definir os melhores cadastros
    /// </summary>
    public class PedidoDetalhadoDto : BaseEntidadeDto
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
        /// Observações gerais do pedido
        /// MIN.: - MAX.: 2000
        /// </summary>
        public string Obs { get; set; }

        /// <summary>
        /// Indica se o pedido foi registrado pelo IFood
        /// </summary>
        public bool PedidoIfood { get; set; }

        /// <summary>
        /// Caso o pedido seja cancelado, informar o motivo
        /// MIN.: - MAX.: 100
        /// </summary>
        public string JustificativaCancelamento { get; set; }

        /// <summary>
        /// Nome do cliente do pedido
        /// </summary>
        public string NomeCliente { get; set; }

        /// <summary>
        /// Descrição do item no momento da venda
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string DescricaoProduto { get; set; }

        /// <summary>
        /// Preço do item no momento da venda
        /// </summary>
        public float PrecoProduto { get; set; }

        /// <summary>
        /// Tipo do item no momento da venda
        /// </summary>
        public TipoProduto TipoProduto { get; set; }

        /// <summary>
        /// Quantidade do produto adicionado ao pedido
        /// </summary>
        public float Quantidade { get; set; }
        
        /// <summary>
        /// Informa se o item está inativo
        /// </summary>
        public bool ItemInativo { get; set; }
    }
}
