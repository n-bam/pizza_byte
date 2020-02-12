using PizzaByteDto.ClassesBase;
using System;
using System.Collections.Generic;

namespace PizzaByteDto.RetornosRequisicoes
{
    public class RequisicaoListaGuidsDto : BaseRequisicaoDto
    {
        /// <summary>
        /// Lista de guids e ser utilizada
        /// </summary>
        public List<Guid> ListaGuids { get; set; }
    }
}
