using PizzaByteDto.Base;
using System;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Dados básicos para definir a relação entre contas a pagar e receber
    /// </summary>
    public class RelacaoContasDto : BaseEntidadeDto
    {
        /// <summary>
        /// Data por extenso
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Data dos registros
        /// </summary>
        public DateTime DataOriginal { get; set; }

        /// <summary>
        /// Valor de contas a pagar
        /// </summary>
        public double ValorPagar { get; set; }

        /// <summary>
        /// Valor total recebido
        /// </summary>
        public double ValorReceber { get; set; }

        /// <summary>
        /// Valor a receber - valor a pagar
        /// </summary>
        public double Saldo { get; set; }
    }
}
