namespace PizzaByteDto.RetornosRequisicoes
{
    public class RetornoObterResumoCaixaDto : RetornoDto
    {
        /// <summary>
        /// Valor recebido em dinheiro
        /// </summary>
        public double RecebidoDinheiro { get; set; }

        /// <summary>
        /// Valor recebido em cartão de crédito
        /// </summary>
        public double RecebidoCredito { get; set; }

        /// <summary>
        /// Valor recebido em cartão de débito
        /// </summary>
        public double RecebidoDebito { get; set; }

        /// <summary>
        /// Valor pago em taxa de entrega
        /// </summary>
        public double TaxaEntrega { get; set; }

        /// <summary>
        /// Valor total de troco 
        /// </summary>
        public double Troco { get; set; }

        /// <summary>
        /// Soma das formas de pagamento, totalizando as vendas
        /// </summary>
        public double TotalVendas { get; set; }
    }
}
