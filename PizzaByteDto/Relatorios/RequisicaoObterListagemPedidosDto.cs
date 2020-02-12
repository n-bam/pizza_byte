using System;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma lista de entidades
    /// </summary>
    public class RequisicaoObterListagemPedidosDto : RequisicaoObterRelatorioListagemDto
    {
        /// <summary>
        /// Indica se é entrega, retirada ou balcão
        /// </summary>
        public TipoPedido Tipo { get; set; }

        /// <summary>
        /// Filtrar por taxa maior que
        /// </summary>
        public float TaxaEntregaInicial { get; set; }

        /// <summary>
        /// Filtrar por taxa menor que
        /// </summary>
        public float TaxaEntregaFinal { get; set; }

        /// <summary>
        /// Filtrar por valor total maior que
        /// </summary>
        public float TotalInicial { get; set; }

        /// <summary>
        /// Filtrar por valor total menor que
        /// </summary>
        public float TotalFinal { get; set; }

        /// <summary>
        /// Filtrar por valor de troco maior que
        /// </summary>
        public float TrocoInicial { get; set; }

        /// <summary>
        /// Filtrar por valor de troco menor que
        /// </summary>
        public float TrocoFinal { get; set; }

        /// <summary>
        /// Valor recebido em dinheiro
        /// </summary>
        public bool IndicadorDinheiro { get; set; }

        /// <summary>
        /// Valor recebido em cartão de crédito
        /// </summary>
        public bool IndicadorCredito { get; set; }

        /// <summary>
        /// Valor recebido em cartão de débito
        /// </summary>
        public bool IndicadorDebito { get; set; }

        /// <summary>
        /// Procurar com fragmentos de observações
        /// </summary>
        public string Obs { get; set; }

        /// <summary>
        /// Nome do cliente
        /// </summary>
        public string NomeCliente { get; set; }
        public Guid? IdCliente { get; set; }

        /// <summary>
        /// Indica se o pedido foi registrado pelo IFood
        /// </summary>
        public string PedidoIfood { get; set; }

        /// <summary>
        /// Caso o pedido seja cancelado, informar o motivo
        /// </summary>
        public string JustificativaCancelamento { get; set; }

        /// <summary>
        /// Indica se é dia de promoção (quinta-feira)
        /// </summary>
        public bool IndicadorPromocao { get; set; }
    }
}
