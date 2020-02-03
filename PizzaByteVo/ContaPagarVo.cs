using PizzaByteVo.Base;
using System;

namespace PizzaByteVo
{
    public class ContaPagarVo : ContaVo
    {
        /// <summary>
        /// Data em que a conta foi paga
        /// </summary>
        public DateTime? DataPagamento { get; set; }

        /// <summary>
        /// Identificação do fornecedor no qual a conta pertence
        /// </summary>
        public Guid? IdFornecedor { get; set; }
    }
}
