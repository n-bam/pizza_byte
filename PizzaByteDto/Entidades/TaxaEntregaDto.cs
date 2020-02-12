using PizzaByteDto.Base;
using System.Text;

namespace PizzaByteDto.Entidades
{
    public class TaxaEntregaDto : BaseEntidadeDto
    {
        /// <summary>
        /// Bairro que a taxa de entrega abrange
        /// MIN.: 3 / MAX.: 50
        /// </summary>
        public string BairroCidade { get; set; }

        /// <summary>
        /// Valor de entrega para o bairro correspondente
        /// </summary>
        public float ValorTaxa { get; set; }

        /// <summary>
        /// Cidade que o bairro pertence
        /// </summary>
        public string Cidade { get; set; }

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

            // Validar o bairro
            if (string.IsNullOrWhiteSpace(BairroCidade))
            {
                sb.Append("O bairro referente à taxa de entrega é obrigatório! Por favor, informe o bairro " +
                    "no campo indicado para continuar. ");
                retorno = false;
            }
            else if (BairroCidade.Length > 101)
            {
                sb.Append("O bairro da taxa de entrega pode ter, no máximo, 50 caracteres! " +
                    $"O bairro inserido tem {BairroCidade.Length} caracteres, por favor remova ao menos {BairroCidade.Length - 101}" +
                    $" caracteres para continuar. ");
                retorno = false;
            }
            else if (BairroCidade.Length < 3)
            {
                sb.Append("O bairro da taxa de entrega deve ter, ao menos, 3 caracteres! Por favor, informe um bairro " +
                    "válido para continuar. ");
                retorno = false;
            }

            // Validar a taxa
            if (ValorTaxa < 0)
            {
                sb.Append("O valor da taxa de entrega não pode ser negativo! Por favor, informe uma " +
                "taxa válido para continuar. ");
                retorno = false;
            }

            mensagemErro = sb.ToString();
            return retorno;
        }

        #endregion
    }
}
