using System;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteVo.Base
{
    /// <summary>
    /// Classe que representa os campos de logs no banco de dados
    /// </summary>
    public class LogVo : EntidadeBaseVo
    {
        /// <summary>
        /// Indica o recurso que foi utilizado
        /// </summary>
        public LogRecursos Recurso { get; set; }

        /// <summary>
        /// Indica qual o usuário que utilizou o recurso
        /// </summary>
        public Guid IdUsuario { get; set; }

        /// <summary>
        /// Indica qual entidade foi modificada
        /// </summary>
        public Guid IdEntidade { get; set; }

        /// <summary>
        /// Armazena a mensagem de retorno dada pelo sistema
        /// </summary>
        public string Mensagem { get; set; }


    }
}
