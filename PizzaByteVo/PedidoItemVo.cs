﻿using System;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteVo
{
    /// <summary>
    /// Classe que representa os campos pedidos do item no banco de dados
    /// </summary>
    public class PedidoItemVo : EntidadeBaseVo
    {
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
        /// Indica qual é o outro sabor de uma pizza meio a meio
        /// </summary>
        public Guid? IdProdutoComposto { get; set; }

        /// <summary>
        /// Identifica o produto adicionado no pedido
        /// </summary>
        public Guid IdProduto { get; set; }

        /// <summary>
        /// Identifica o pedido que o item pertence
        /// </summary>
        public Guid IdPedido { get; set; }

        /// <summary>
        /// O produto que é item do pedido
        /// </summary>
        public virtual ProdutoVo Produto { get; set; }

        /// <summary>
        /// Item que faz meio a meio
        /// </summary>
        public virtual ProdutoVo ProdutoComposto { get; set; }

        /// <summary>
        /// Pedido que o item pertence
        /// </summary>
        public virtual PedidoVo Pedido { get; set; }
    }
}
