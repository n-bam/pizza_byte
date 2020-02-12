using PizzaByteDto.Base;
using System;
using System.Text;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.Entidades
{
    public class ContaReceberDto : ContaDto
    {

        /// <summary>
        /// Id do pedido da conta
        /// </summary>
        public Guid? IdPedido { get; set; }

        #region Métodos


        #endregion
    }
}
