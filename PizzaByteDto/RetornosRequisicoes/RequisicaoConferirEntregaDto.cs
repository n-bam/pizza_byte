namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para conferir uma entrega
    /// </summary>
    public class RequisicaoConferirEntregaDto : RequisicaoObterDto
    {
        /// <summary>
        /// Valor retornado da entrega
        /// </summary>
        public float ValorRetornado { get; set; }
    }
}
