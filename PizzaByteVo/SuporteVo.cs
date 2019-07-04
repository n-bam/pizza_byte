using static PizzaByteEnum.Enumeradores;

namespace PizzaByteVo
{
    /// <summary>
    /// Classe que representa os campos do suporte no banco de dados
    /// </summary>
    public class SuporteVo : EntidadeBaseVo
    {
        /// <summary>
        /// Mensagem enviada/recebida
        /// MIN: 3 / MAX: 2000
        /// </summary>
        public string Mensagem { get; set; }

        /// <summary>
        /// Indica se a mensagem é do usuário ou do atendente
        /// MIN: 0
        /// </summary>
        public TipoMensagemSuporte Tipo { get; set; }
    }
}