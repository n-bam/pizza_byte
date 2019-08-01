using PizzaByteDto.Base;
using System.Text;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.Entidades
{
    public class FuncionarioDto : BaseEntidadeDto
    {
        /// <summary>
        /// Nome do funcionário 
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Telefone de contato do funcionário
        /// MIN.: 8 / MAX.: 20
        /// </summary>
        public string Telefone { get; set; }

        /// <summary>
        /// Tipo de atuação do funcionário (Motoboy, atendente, etc)
        /// MIN.: 1
        /// </summary>
        public TipoFuncionario Tipo { get; set; }

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
            if (string.IsNullOrWhiteSpace(Nome))
            {
                sb.Append("O nome do funcionário é obrigatório! Por favor, informe o nome do funcionário " +
                    "no campo indicado para continuar. ");
                retorno = false;
            }
            else if (Nome.Length > 150)
            {
                sb.Append("O nome do funcionário pode ter, no máximo, 150 caracteres! " +
                    $"O nome inserido tem {Nome.Length} caracteres, por favor remova ao menos {Nome.Length - 150}" +
                    $" caracteres para continuar. ");
                retorno = false;
            }
            else if (Nome.Length < 3)
            {
                sb.Append("O nome do funcionário deve ter, ao menos, 3 caracteres! Por favor, informe um nome " +
                    "válido para continuar. ");
                retorno = false;
            }

            // Validar a senha
            if (!string.IsNullOrWhiteSpace(Telefone))
            {
                if (Telefone.Length > 20)
                {
                    sb.Append("O telefone do funcionário deve ter, no máximo, 20 caracteres! " +
                    $"O telefone inserido tem {Telefone.Length} caracteres, por favor remova ao " +
                    $"menos {Telefone.Length - 20} caracteres para continuar. ");
                    retorno = false;
                }
                else if (Telefone.Length < 8)
                {
                    sb.Append("O telefone do funcionário deve ter, ao menos, 8 caracteres! Por favor, informe um " +
                    "telefone válido para continuar. ");
                    retorno = false;
                }
            }

            // Validar o tipo
            if (Tipo == TipoFuncionario.NaoIdentificado)
            {
                sb.Append("O tipo do funcionário não foi informado! Por favor, informe um " +
                   "tipo válido para continuar. ");
                retorno = false;
            }

            mensagemErro = sb.ToString();
            return retorno;
        }

        #endregion
    }
}
