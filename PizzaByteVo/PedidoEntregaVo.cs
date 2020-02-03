using System;

namespace PizzaByteVo
{
    /// <summary>
    /// Classe que representa os campos de pedidos de entrega no banco de dados
    /// </summary>
    public class PedidoEntregaVo : EntidadeBaseVo
    {
        /// <summary>
        /// Indica se o retorno da entrega já teve os valores conferidos
        /// </summary>
        public bool Conferido { get; set; }

        /// <summary>
        /// Valor em dinheiro retornado na entrega
        /// </summary>
        public float ValorRetorno { get; set; }

        /// <summary>
        /// Observações gerais
        /// </summary>
        public string Obs { get; set; }

        /// <summary>
        /// Identificação do endereço de entrega
        /// </summary>
        public Guid IdEndereco { get; set; }

        /// <summary>
        /// Identifica o funcionário que fez a entrega
        /// </summary>
        public Guid? IdFuncionario { get; set; }

        /// <summary>
        /// Identificação do pedido que gerou a entrega
        /// </summary>
        public Guid IdPedido { get; set; }

        /// <summary>
        /// Pedido que gerou a entrega
        /// </summary>
        public virtual PedidoVo Pedido { get; set; }

        /// <summary>
        /// Endereço da entrega
        /// </summary>
        public virtual ClienteEnderecoVo ClienteEndereco { get; set; }
     
        /// <summary>
        /// Funcionário que fez a entrega
        /// </summary>
        public virtual FuncionarioVo Funcionario { get; set; }
    }
}
