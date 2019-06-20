using PizzaByteDto.Base;
using System.Collections.Generic;

namespace PizzaByteDto.RetornosRequisicoes
{
    public class RetornoObterListaDto<T> : RetornoDto where T : BaseEntidadeDto
    {
        public RetornoObterListaDto()
        {
            this.ListaEntidades = new List<T>();
        }

        /// <summary>
        /// Lista de entidades do resultado
        /// </summary>
        public List<T> ListaEntidades { get; set; }

        /// <summary>
        /// Número total de páginas da pesquisa
        /// </summary>
        public int NumeroPaginas { get; set; }
    }
}
