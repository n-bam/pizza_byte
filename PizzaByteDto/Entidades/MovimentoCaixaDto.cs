using PizzaByteDto.Base;
using System.Text;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.Entidades
{
    public class MovimentoCaixaDto : BaseEntidadeDto
    {
        /// <summary>
        /// Breve descritivo da movimentação
        /// MIN.: 3 / MAX.: 100
        /// </summary>
        public string Justificativa { get; set; }

        /// <summary>
        /// Valor da movimentação
        /// </summary>
        public float Valor { get; set; }

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

            // Validar a justificativa
            if (string.IsNullOrWhiteSpace(Justificativa))
            {
                sb.Append("A justificativa da movimentação de caixa é obrigatória! Por favor, informe a justificativa " +
                    "no campo indicado para continuar. ");
                retorno = false;
            }
            else if (Justificativa.Length > 100)
            {
                sb.Append("A justificativa da movimentação pode ter, no máximo, 100 caracteres! " +
                    $"A justificativa inserida tem {Justificativa.Length} caracteres, por favor remova ao menos {Justificativa.Length - 100}" +
                    $" caracteres para continuar. ");
                retorno = false;
            }
            else if (Justificativa.Length < 3)
            {
                sb.Append("A justificativa da movimentação deve ter, ao menos, 3 caracteres! Por favor, informe uma justificativa " +
                    "válida para continuar. ");
                retorno = false;
            }

            // Validar o valor
            if (Valor == 0)
            {
                sb.Append("O valor da movimentação não pode ser 0 (zero)! Por favor, informe um " +
                "valor válido para continuar. ");
                retorno = false;
            }
            
            mensagemErro = sb.ToString();
            return retorno;
        }

        #endregion
    }
}
