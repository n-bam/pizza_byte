using static PizzaByteEnum.Enumeradores;

namespace PizzaByteVo.Base
{
    /// <summary>
    /// Classe que representa os campos do suporte no banco de dados
    /// </summary>
    public class SuporteVo : EntidadeBaseVo
    {
        /// <summary>
        /// Mensagem enviada/recebida
        /// MIN: 0 / MAX: 2000
        /// </summary>
        public string Mensagem { get; set; }

        /// <summary>
        /// Indica se a mensagem é do usuário ou do atendente
        /// </summary>
        public TipoMensagemSuporte Tipo { get; set; }
    }
}