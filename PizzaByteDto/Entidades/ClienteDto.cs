using PizzaByteDto.Base;
using System.Text;

namespace PizzaByteDto.Entidades
{
    public class ClienteDto : BaseEntidadeDto
    {
        /// <summary>
        /// Nome completo do cliente (obrigatório)
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Telefone de contato do cliente
        /// MIN.: 8 / MAX.: 20
        /// </summary>
        public string Telefone { get; set; }

        /// <summary>
        /// CPF do cliente
        /// MIN/MAX.: 11
        /// </summary>
        public string Cpf { get; set; }

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

            // Validar o nome do cliente
            if (string.IsNullOrWhiteSpace(Nome))
            {
                sb.Append("O nome do cliente é obrigatório! Por favor, informe o nome do cliente " +
                    "no campo indicado para continuar. ");
                retorno = false;
            }
            else if (Nome.Length > 150)
            {
                sb.Append("O nome do cliente pode ter, no máximo, 150 caracteres! " +
                    $"O nome inserido tem {Nome.Length} caracteres, por favor remova ao menos {Nome.Length - 150}" +
                    $" caracteres para continuar. ");
                retorno = false;
            }
            else if (Nome.Length < 3)
            {
                sb.Append("O nome do cliente deve ter, ao menos, 3 caracteres! Por favor, informe um nome " +
                    "válido para continuar. ");
                retorno = false;
            }

            // Validar o telefone
            if (!string.IsNullOrWhiteSpace(Telefone))
            {
                if (Telefone.Length > 20)
                {
                    sb.Append("O telefone do cliente deve ter, no máximo, 20 caracteres! " +
                    $"O telefonr inserido tem {Telefone.Length} caracteres, por favor remova ao " +
                    $"menos {Telefone.Length - 20} caracteres para continuar. ");
                    retorno = false;
                }
                else if (Telefone.Length < 8)
                {
                    sb.Append("O telefone do cliente deve ter, ao menos, 8 caracteres! Por favor, informe um " +
                    "telefone válido para continuar. ");
                    retorno = false;
                }
            }

            // Valida o CPF
            if (!string.IsNullOrWhiteSpace(Cpf))
            {
                if (Cpf.Replace("-", "").Replace(".", "").Length != 11)
                {
                    sb.Append("O CPF do cliente deve ter 11 caracteres! Por favor, informe um " +
                    "CPF válido para continuar. ");
                    retorno = false;
                }

                if (!ValidarCpf(Cpf))
                {
                    sb.Append("O CPF informado não é válido.");
                    retorno = false;
                }
            }

            mensagemErro = sb.ToString();
            return retorno;
        }

        #endregion
    }
}
