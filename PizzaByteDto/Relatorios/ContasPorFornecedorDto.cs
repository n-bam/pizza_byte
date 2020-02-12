using PizzaByteDto.Base;
using System;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Dados básicos para definir a relação entre contas a pagar e receber
    /// </summary>
    public class ContasPorFornecedorDto : BaseEntidadeDto
    {
        /// <summary>
        /// Nome fantasia do fornecedor
        /// </summary>
        public string NomeFantasia { get; set; }

        /// <summary>
        /// Descrição da conta
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Id do fornecedor
        /// </summary>
        public Guid? IdFornecedor { get; set; }

        /// <summary>
        /// Quantidade de contas feitas
        /// </summary>
        public double Valor { get; set; }

        /// <summary>
        /// Data de vencimento da conta
        /// </summary>
        public DateTime DataVencimento { get; set; }

        /// <summary>
        /// Data de pagamento da conta
        /// </summary>
        public DateTime? DataPagamento { get; set; }

        /// <summary>
        /// Status da conta
        /// </summary>
        public StatusConta Status { get; set; }
    }
}
