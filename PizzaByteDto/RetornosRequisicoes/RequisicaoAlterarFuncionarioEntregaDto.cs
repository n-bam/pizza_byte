using System;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para definir o profissional que fará a entrega
    /// </summary>
    public class RequisicaoAlterarFuncionarioEntregaDto : RequisicaoObterDto
    {
        /// <summary>
        /// Código do funcionário que fará a entrega
        /// </summary>
        public Guid IdFuncionario { get; set; }
    }
}
