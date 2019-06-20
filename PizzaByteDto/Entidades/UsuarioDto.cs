using PizzaByteDto.Base;
using System;
using System.Text;

namespace PizzaByteDto.Entidades
{
    public class UsuarioDto : BaseEntidadeDto
    {
        /// <summary>
        /// Nome do usuário
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// E-mail para autenticação no software
        /// MIN.: 5 / MAX.: 100
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Senha criptografada de autenticação do usuário
        /// MIN.: Criptografia / MAX.: 50
        /// </summary>
        public string Senha { get; set; }
        
        /// <summary>
        /// Senha criptografada do usuário antes das alterações
        /// </summary>
        public string SenhaAntiga { get; set; }

        /// <summary>
        /// Indica se o usuário é administrador
        /// </summary>
        public bool Administrador { get; set; }

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
                sb.Append("O nome do usuário é obrigatório! Por favor, informe o nome do usuário " +
                    "no campo indicado para continuar. ");
                retorno = false;
            }
            else if (Nome.Length > 150)
            {
                sb.Append("O nome do usuário pode ter, no máximo, 150 caracteres! " +
                    $"O nome inserido tem {Nome.Length} caracteres, por favor remova ao menos {Nome.Length - 150}" +
                    $" caracteres para continuar. ");
                retorno = false;
            }
            else if (Nome.Length < 3)
            {
                sb.Append("O nome do usuário deve ter, ao menos, 3 caracteres! Por favor, informe um nome " +
                    "válido para continuar. ");
                retorno = false;
            }

            // Validar a senha
            if (!string.IsNullOrWhiteSpace(Senha))
            {
                if (Senha.Length > 50)
                {
                    sb.Append("A senha do usuário deve ter, no máximo, 50 caracteres! " +
                    $"senha inserida tem {Senha.Length} caracteres, por favor remova ao " +
                    $"menos {Senha.Length - 50} caracteres para continuar. ");
                    retorno = false;
                }
                else if (Senha.Length < 6)
                {
                    sb.Append("A senha do usuário deve ter, ao menos, 6 caracteres! Por favor, informe uma " +
                    "senha válido para continuar. ");
                    retorno = false;
                }
            }

            // Valida o CPF
            try
            {
                System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(Email);
            }
            catch (Exception)
            {
                sb.Append("O Email informado é inválido. Por favor, informe um email válido para continuar. ");
                retorno = false;
            }

            mensagemErro = sb.ToString();
            return retorno;
        }

        #endregion
    }
}
