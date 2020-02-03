using PizzaByteVo.Base;
using System;

namespace PizzaByteVo
{
    public class ContaReceberVo : ContaVo
    {
        /// <summary>
        /// Identificação do pedido no qual a conta pertence
        /// </summary>
        public Guid? IdPedido { get; set; }
    }
}
