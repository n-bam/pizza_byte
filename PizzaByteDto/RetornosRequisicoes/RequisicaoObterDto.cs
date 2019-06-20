
using PizzaByteDto.ClassesBase;
using System;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma entidade
    /// </summary>
    public class RequisicaoObterDto : BaseRequisicaoDto
    {
        /// <summary>
        /// Id a ser obtido
        /// </summary>
        public Guid Id { get; set; }
    }
}
