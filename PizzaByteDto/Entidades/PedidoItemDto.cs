using PizzaByteDto.Base;
using System;
using System.Text;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.Entidades
{
    public class PedidoItemDto : BaseEntidadeDto
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
        
        #region Métodos

        /// <summary>
        /// Valida se os dados estão corretos
        /// </summary>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public override bool ValidarEntidade(ref string mensagemErro)
        {
            bool retorno = base.ValidarEntidade(ref mensagemErro);
            StringBuilder sb = new StringBuilder();

            sb.Append(mensagemErro);
            mensagemErro = "";

            if (!string.IsNullOrWhiteSpace(DescricaoProduto))
            {
                if (DescricaoProduto.Length > 300)
                {
                    sb.Append("A descrição do item pode ter, no máximo, 300 caracteres! " +
                        $"A descrição inserida tem {DescricaoProduto.Length} caracteres, por favor remova ao menos {DescricaoProduto.Length - 300}" +
                        $" caracteres para continuar. ");
                    retorno = false;
                }
                else if (DescricaoProduto.Length < 3)
                {
                    sb.Append("A descrição do item deve ter, ao menos, 3 caracteres! Por favor, informe uma decrição " +
                        "válido para continuar. ");
                    retorno = false;
                }
            }

            if (PrecoProduto < 0)
            {
                sb.Append("O valor do item não pode ser negativo! Por favor, informe um " +
                                "valor válido para continuar. ");
                retorno = false;
            }

            if (Quantidade < 0)
            {
                sb.Append("A quantidade do item não pode ser negativo! Por favor, informe uma " +
                                "quantidade válido para continuar. ");
                retorno = false;
            }

            if (Quantidade == 0.5 && (IdProdutoComposto == null && IdProdutoComposto == Guid.Empty))
            {
                sb.Append($"O segundo sabor do item {DescricaoProduto} não foi informado! Por favor, informe outro " +
                                "sabora para continuar. ");
                retorno = false;
            }

            if (IdProduto == Guid.Empty)
            {
                sb.Append("O id do produto deve ser informado! Por favor, refaça a operação. ");
                retorno = false;
            }

            if (IdPedido == Guid.Empty)
            {
                sb.Append("O id do pedido deve ser informado! Por favor, refaça a operação. ");
                retorno = false;
            }

            mensagemErro = sb.ToString();
            return retorno;
        }

        #endregion
    }
}
