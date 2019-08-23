using PizzaByteDto.Base;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.RetornosRequisicoes

{
    public class BackupDto : BaseEntidadeDto
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
    }
}
