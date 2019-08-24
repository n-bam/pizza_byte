using System.Text;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.Base
{
    public class SuporteDto : BaseEntidadeDto
    {
        /// <summary>
        /// Mensagem enviada/recebida
        /// MIN: 0 / MAX: 500
        /// </summary>
        public string Mensagem { get; set; }

        /// <summary>
        /// Indica se a mensagem é do usuário ou do atendente
        /// </summary>
        public TipoMensagemSuporte Tipo { get; set; }

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

            // Validar a mensagem
            if (string.IsNullOrWhiteSpace(Mensagem))
            {
                sb.Append("A mensagem é obrigatória! Por favor, informe a mensagem a ser enviada " +
                    "no campo indicado para continuar. ");
                retorno = false;
            }
            else if (Mensagem.Length > 500)
            {
                sb.Append("A mensagem pode ter, no máximo, 500 caracteres! " +
                    $"A mensagem inserida tem {Mensagem.Length} caracteres, por favor remova ao menos {Mensagem.Length - 500}" +
                    $" caracteres para continuar. ");
                retorno = false;
            }
            else if (Mensagem.Length < 1)
            {
                sb.Append("A mensagem deve ter, ao menos, 1 caracteres! Por favor, informe uma mensagem " +
                    "válida para continuar. ");
                retorno = false;
            }
            
            // Validar o tipo
            if (Tipo == TipoMensagemSuporte.NaoIdentificado)
            {
                sb.Append("O tipo da mensagem não foi informado! Por favor, informe um " +
                   "tipo válido para continuar. ");
                retorno = false;
            }

            mensagemErro = sb.ToString();
            return retorno;
        }
        
        #endregion
    }
}
