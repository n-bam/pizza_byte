using System;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Retorna a identificação com as informações criptografadas do login
    /// </summary>
    public class RetornoFazerLoginDto : RetornoDto
    {
        /// <summary>
        /// String com as informações criptografadas do login
        /// </summary>
        public string Identificacao { get; set; }

        /// <summary>
        /// Nome do usuário do login
        /// </summary>
        public string NomeUsuario { get; set; }

        /// <summary>
        /// Id do usuário de login
        /// </summary>
        public Guid IdUsuario { get; set; }

        /// <summary>
        /// Retorna se o usuário é adm ou não
        /// </summary>
        public bool UsuarioAdministrador { get; set; }
    }
}
