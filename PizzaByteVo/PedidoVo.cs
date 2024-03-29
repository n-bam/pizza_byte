﻿using System;
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
        /// Identificação do cliente do pedido
        /// </summary>
        public Guid? IdCliente { get; set; }

        /// <summary>
        /// Cliente do pedido
        /// </summary>
        public virtual ClienteVo Cliente { get; set; }
    }
}
