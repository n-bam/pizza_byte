using PizzaByteDto.RetornosRequisicoes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um pedido no site
    /// </summary>
    public class FiltrosListagemPedidoModel : BaseFiltrosRelatorioModel
    {
        public FiltrosListagemPedidoModel()
        {
            ListaTipos = Utilidades.RetornarListaTiposPedido();
            ListaTipos.Insert(0, new SelectListItem()
            {
                Selected = true,
                Text = "Todos",
                Value = "0"
            });

            ListaInativo[1].Text = "Apenas não cancelados";
            ListaInativo[2].Text = "Apenas cancelados";

            ListaOpcoesIFood = Utilidades.RetornarListaOpcaoIFood();
            ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoPedido();
        }

        /// <summary>
        /// Indica se é entrega, retirada ou balcão
        /// </summary>
        [Display(Name = "Tipo")]
        public TipoPedido Tipo { get; set; }

        /// <summary>
        /// Filtrar por taxa maior que
        /// </summary>
        [Display(Name = "Taxa de entrega de")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float TaxaEntregaInicial { get; set; }

        /// <summary>
        /// Filtrar por taxa menor que
        /// </summary>
        [Display(Name = "até")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float TaxaEntregaFinal { get; set; }

        /// <summary>
        /// Filtrar por valor total maior que
        /// </summary>
        [Display(Name = "Total de")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float TotalInicial { get; set; }

        /// <summary>
        /// Filtrar por valor total menor que
        /// </summary>
        [Display(Name = "até")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float TotalFinal { get; set; }

        /// <summary>
        /// Filtrar por valor de troco maior que
        /// </summary>
        [Display(Name = "Troco de")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float TrocoInicial { get; set; }

        /// <summary>
        /// Filtrar por valor de troco menor que
        /// </summary>
        [Display(Name = "até")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float TrocoFinal { get; set; }

        /// <summary>
        /// Valor recebido em dinheiro
        /// </summary>
        [Display(Name = "Com recebimento em dinheiro")]
        public bool IndicadorDinheiro { get; set; }

        /// <summary>
        /// Valor recebido em cartão de crédito
        /// </summary>
        [Display(Name = "Com recebimento em crédito")]
        public bool IndicadorCredito { get; set; }

        /// <summary>
        /// Valor recebido em cartão de débito
        /// </summary>
        [Display(Name = "Com recebimento em débito")]
        public bool IndicadorDebito { get; set; }

        /// <summary>
        /// Procurar com fragmentos de observações
        /// </summary>
        [Display(Name = "Observações")]
        public string Obs { get; set; }

        /// <summary>
        /// Nome do cliente
        /// </summary>
        [Display(Name = "Cliente")]
        public string NomeCliente { get; set; }
        public Guid? IdCliente { get; set; }

        /// <summary>
        /// Indica se o pedido foi registrado pelo IFood
        /// </summary>
        [Display(Name = "Origem")]
        public string PedidoIfood { get; set; }

        /// <summary>
        /// Caso o pedido seja cancelado, informar o motivo
        /// </summary>
        [Display(Name = "Justificativa")]
        public string JustificativaCancelamento { get; set; }

        /// <summary>
        /// Indica se é dia de promoção (quinta-feira)
        /// </summary>
        [Display(Name = "Promoção")]
        public bool IndicadorPromocao { get; set; }

        /// <summary>
        /// Lista com as opções de tipos de pedido
        /// </summary>
        public List<SelectListItem> ListaTipos { get; set; }

        /// <summary>
        /// Lista com as opções de filtro de pedidos IFood
        /// </summary>
        public List<SelectListItem> ListaOpcoesIFood { get; set; }

        /// <summary>
        /// Converte um pedido de DTO para Model
        /// </summary>
        /// <param name="pedidoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref RequisicaoObterListagemPedidosDto requisicaoDto, ref string mensagemErro)
        {
            if (!base.ConverterModelParaDto(requisicaoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                requisicaoDto.Obs = string.IsNullOrWhiteSpace(Obs) ? "" : Obs.Trim();
                requisicaoDto.JustificativaCancelamento = string.IsNullOrWhiteSpace(JustificativaCancelamento) ? "" : JustificativaCancelamento.Trim();
                requisicaoDto.Tipo = Tipo;
                requisicaoDto.IdCliente = IdCliente == Guid.Empty ? null : IdCliente;
                requisicaoDto.IndicadorCredito = IndicadorCredito;
                requisicaoDto.IndicadorDebito = IndicadorDebito;
                requisicaoDto.IndicadorDinheiro = IndicadorDinheiro;
                requisicaoDto.IndicadorPromocao = IndicadorPromocao;
                requisicaoDto.PedidoIfood = PedidoIfood;
                requisicaoDto.TaxaEntregaInicial = TaxaEntregaInicial;
                requisicaoDto.TaxaEntregaFinal = TaxaEntregaFinal;
                requisicaoDto.TotalInicial = TotalInicial;
                requisicaoDto.TotalFinal = TotalFinal;
                requisicaoDto.TrocoFinal = TrocoFinal;
                requisicaoDto.TrocoInicial = TrocoInicial;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }
    }
}