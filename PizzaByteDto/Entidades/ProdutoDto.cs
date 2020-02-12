using PizzaByteDto.Base;
using System.Text;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.Entidades
{
    public class ProdutoDto : BaseEntidadeDto
    {
        /// <summary>
        /// Breve descritivo do produto
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Breve descritivo do produto
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public float Preco { get; set; }

        /// <summary>
        /// Detalhes do produto
        /// </summary>
        public string Detalhes { get; set; }

        /// <summary>
        /// Indica o tipo do produto (bebida, pizza, etc.)
        /// Maior que 0
        /// </summary>
        public TipoProduto Tipo { get; set; }

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

            // Validar o nome
            if (string.IsNullOrWhiteSpace(Descricao))
            {
                sb.Append("A descrição do produto é obrigatória! Por favor, informe a descrição do produto " +
                    "no campo indicado para continuar. ");
                retorno = false;
            }
            else if (Descricao.Length > 150)
            {
                sb.Append("A descrição do produto pode ter, no máximo, 150 caracteres! " +
                    $"A descrição inserida tem {Descricao.Length} caracteres, por favor remova ao menos {Descricao.Length - 150}" +
                    $" caracteres para continuar. ");
                retorno = false;
            }
            else if (Descricao.Length < 3)
            {
                sb.Append("A descrição do produto deve ter, ao menos, 3 caracteres! Por favor, informe uma descrição " +
                    "válido para continuar. ");
                retorno = false;
            }

            // Validar o preço
            if (Preco < 0)
            {
                sb.Append("O preço do produto não pode ser negativo! Por favor, informe um " +
                "preço válido para continuar. ");
                retorno = false;
            }

            // Validar o tipo
            if (Tipo == TipoProduto.NaoIdentificado)
            {
                sb.Append("O tipo do produto não foi informado! Por favor, informe um " +
                   "tipo válido para continuar. ");
                retorno = false;
            }

            if (!string.IsNullOrWhiteSpace(Detalhes) && Detalhes.Length > 200)
            {
                sb.Append("Os detalhes do produto podem ter, no máximo, 200 caracteres! " +
                   $"O detalhe inserida tem {Detalhes.Length} caracteres, por favor remova ao menos {Detalhes.Length - 200}" +
                   $" caracteres para continuar. ");
                retorno = false;
            }

            mensagemErro = sb.ToString();
            return retorno;
        }

        #endregion
    }
}
