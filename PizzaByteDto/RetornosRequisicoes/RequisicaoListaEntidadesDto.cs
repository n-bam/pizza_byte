using PizzaByteDto.Base;
using PizzaByteDto.ClassesBase;
using System.Collections.Generic;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para incluir/editar uma lista de entidades
    /// </summary>
    public class RequisicaoListaEntidadesDto<T> : BaseRequisicaoDto where T : BaseEntidadeDto
    {
        public RequisicaoListaEntidadesDto()
        {
            ListaEntidadesDto = new List<T>();
        }

        /// <summary>
        /// Lista de entidades a ser processada
        /// </summary>
        public List<T> ListaEntidadesDto { get; set; }
    }
}
