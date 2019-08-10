using System.Collections.Generic;

namespace PizzaByteDto.RetornosRequisicoes
{
    public class RetornoObterDicionarioDto<T, Y> : RetornoDto
    {
        public RetornoObterDicionarioDto()
        {
            this.ListaEntidades = new Dictionary<T, Y>();
        }

        /// <summary>
        /// Lista de entidades do resultado
        /// </summary>
        public Dictionary<T, Y> ListaEntidades { get; set; }

    }
}
