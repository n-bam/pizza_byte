using System;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para cancelar um pedido
    /// </summary>
    public class RequisicaoCancelarPedidoDto : RequisicaoObterDto
    {
        /// <summary>
        /// Id a ser obtido
        /// </summary>
        public string Justificativa { get; set; }
    }
}
