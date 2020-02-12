using PizzaByteDto.Base;
using System;
using System.Collections.Generic;
using System.Text;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.Entidades
{
    public class PedidoDto : BaseEntidadeDto
    {
        public PedidoDto()
        {
            ListaItens = new List<PedidoItemDto>();
            Cliente = new ClienteDto();
        }

        /// <summary>
        /// Indica se é entrega, retirada ou balcão
        /// </summary>
        public TipoPedido Tipo { get; set; }

        /// <summary>
        /// Valor total do pedido
        /// </summary>
        public float Total { get; set; }

        /// <summary>
        /// Valor de troco necessário 
        /// </summary>
        public float Troco { get; set; }

        /// <summary>
        /// Valor da taxa de entrega de acordo com o bairro
        /// </summary>
        public float TaxaEntrega { get; set; }

        /// <summary>
        /// Valor recebido em dinheiro
        /// </summary>
        public float RecebidoDinheiro { get; set; }

        /// <summary>
        /// Valor recebido em cartão de crédito
        /// </summary>
        public float RecebidoCredito { get; set; }

        /// <summary>
        /// Valor recebido em cartão de débito
        /// </summary>
        public float RecebidoDebito { get; set; }

        /// <summary>
        /// Observações gerais do pedido
        /// MIN.: - MAX.: 2000
        /// </summary>
        public string Obs { get; set; }

        /// <summary>
        /// Indica se o pedido foi registrado pelo IFood
        /// </summary>
        public bool PedidoIfood { get; set; }

        /// <summary>
        /// Caso o pedido seja cancelado, informar o motivo
        /// MIN.: - MAX.: 100
        /// </summary>
        public string JustificativaCancelamento { get; set; }

        /// <summary>
        /// Nome do cliente do pedido
        /// </summary>
        public string NomeCliente { get; set; }

        /// <summary>
        /// Identificação do cliente do pedido
        /// </summary>
        public Guid? IdCliente { get; set; }

        /// <summary>
        /// Cliente do pedido
        /// </summary>
        public ClienteDto Cliente { get; set; }

        /// <summary>
        /// Dados de entrega do pedido
        /// </summary>
        public PedidoEntregaDto Entrega { get; set; }

        /// <summary>
        /// A lista de itens do pedido
        /// </summary>
        public List<PedidoItemDto> ListaItens { get; set; }

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

            if (!string.IsNullOrWhiteSpace(JustificativaCancelamento))
            {
                if (JustificativaCancelamento.Length > 100)
                {
                    sb.Append("A justificativa do cancelamento pode ter, no máximo, 100 caracteres! " +
                        $"A justificativa inserida tem {JustificativaCancelamento.Length} caracteres, por favor remova ao menos {JustificativaCancelamento.Length - 100}" +
                        $" caracteres para continuar. ");
                    retorno = false;
                }
                else if (JustificativaCancelamento.Length < 3)
                {
                    sb.Append("A justificativa do cancelamento deve ter, ao menos, 3 caracteres! Por favor, informe uma justificativa " +
                        "válido para continuar. ");
                    retorno = false;
                }
            }

            if (!string.IsNullOrWhiteSpace(Obs))
            {
                if (Obs.Length > 2000)
                {
                    sb.Append("As observações podem ter, no máximo, 2000 caracteres! " +
                        $"A justificativa inserida tem {Obs.Length} caracteres, por favor remova ao menos {Obs.Length - 2000}" +
                        $" caracteres para continuar. ");
                    retorno = false;
                }
            }

            if (Troco < 0)
            {
                sb.Append("O valor do troco do pedido não pode ser negativo! Por favor, informe um " +
                                "troco válido para continuar. ");
                retorno = false;
            }

            if (TaxaEntrega < 0)
            {
                sb.Append("A taxa de entrega do pedido não pode ser negativo! Por favor, informe uma " +
                                "taxa válido para continuar. ");
                retorno = false;
            }

            if (RecebidoDinheiro < 0)
            {
                sb.Append("O valor em dinheiro no pedido não pode ser negativo! Por favor, informe uma " +
                                "valor válido para continuar. ");
                retorno = false;
            }

            if (RecebidoCredito < 0)
            {
                sb.Append("O valor em cartão de crédito no pedido não pode ser negativo! Por favor, informe uma " +
                                "valor válido para continuar. ");
                retorno = false;
            }

            if (RecebidoDebito < 0)
            {
                sb.Append("O valor em cartão de débito pedido no pedido não pode ser negativo! Por favor, informe uma " +
                                "valor válido para continuar. ");
                retorno = false;
            }

            if (Total < 0)
            {
                sb.Append("O valor total do pedido não pode ser negativo! Por favor, informe um " +
                "total válido para continuar. ");
                retorno = false;
            }

            if (Tipo == TipoPedido.NaoIdentificado)
            {
                sb.Append("O tipo do pedido não foi informado! Por favor, informe um " +
                   "tipo válido para continuar. ");
                retorno = false;
            }

            if (Tipo == TipoPedido.Entrega &&
                (Entrega == null || Cliente == null || string.IsNullOrWhiteSpace(Cliente.Nome)
                || Entrega.ClienteEndereco == null || Entrega.ClienteEndereco.Endereco == null
                || string.IsNullOrWhiteSpace(Entrega.ClienteEndereco.NumeroEndereco)
                || string.IsNullOrWhiteSpace(Entrega.ClienteEndereco.Endereco.Logradouro)))
            {
                sb.Append("Pedidos para entrega precisam do endereço completo informado. Por favor, corrija o endereço de entrega. ");
                retorno = false;
            }

            mensagemErro = sb.ToString();
            return retorno;
        }

        #endregion
    }
}
