using PizzaByteDto.Base;
using System.Text;
using System.Diagnostics;
using System;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.Base
{
    public class LogDto : BaseEntidadeDto
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
        /// Diz qual a mensagem de erro foi exibida para o usuário
        /// </summary>
        public string Mensagem { get; set; }
    }
}
