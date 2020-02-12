using PizzaByteDto.Base;
using System;
using System.Text;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.Entidades
{
    public class PedidoEntregaDto : BaseEntidadeDto
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
        /// Endereço do cliente
        /// </summary>
        public ClienteEnderecoDto ClienteEndereco { get; set; }

        #region Métodos

        /// <summary>
        /// Valida se os dados estão corretos
        /// </summary>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public override bool ValidarEntidade(ref string mensagemErro)
        {
            bool retorno = base.ValidarEntidade(ref mensagemErro);
            StringBuilder sb = new StringBuilder();

            sb.Append(mensagemErro);
            mensagemErro = "";

            // Validar o nome
            if (!string.IsNullOrWhiteSpace(Obs))
            {
                if (Obs.Length > 2000)
                {
                    sb.Append("As observações da entrega podem ter, no máximo, 2000 caracteres! " +
                        $"As observações inseridas têm {Obs.Length} caracteres, por favor remova ao menos {Obs.Length - 2000}" +
                        $" caracteres para continuar. ");
                    retorno = false;
                }
            }

            // Validar o preço
            if (ValorRetorno < 0)
            {
                sb.Append("O valor retornadao da entrega não pode ser negativo! Por favor, informe um " +
                "valor válido para continuar. ");
                retorno = false;
            }
            
            mensagemErro = sb.ToString();
            return retorno;
        }

        #endregion
    }
}
