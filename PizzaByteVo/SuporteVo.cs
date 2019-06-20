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
        /// </summary>
        public string Mensagem { get; set; }

        /// <summary>
        /// Indica se a mensagem é do usuário ou do atendente
        /// </summary>
        public TipoMensagemSuporte Tipo { get; set; }
    }
}