using PizzaByteDto.Entidades;
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
    public class PedidoModel : BaseModel
    {
        public PedidoModel()
        {
            ListaTipos = Utilidades.RetornarListaTiposPedido();
            ListaItens = new List<PedidoItemModel>();
            Cliente = new ClienteModel();
            Entrega = new PedidoEntregaModel();
        }

        /// <summary>
        /// Indica se é entrega, retirada ou balcão
        /// </summary>
        [Required(ErrorMessage = "Por favor, selecione qual o tipo do produto")]
        [Display(Name = "Tipo")]
        public TipoPedido Tipo { get; set; }

        /// <summary>
        /// Valor total dos pedidos (soma dos itens + frete)
        /// </summary>
        [Required(ErrorMessage = "Por favor, adicione ao menos um produto")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0, 999999.99, ErrorMessage = "Informe o valor total do pedido")]
        [Display(Name = "Total")]
        public float Total { get; set; }

        /// <summary>
        /// Valor de troco necessário 
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0, 999999.99, ErrorMessage = "Informe o valor do troco entre 0 e 999999,99")]
        [Display(Name = "Troco")]
        public float Troco { get; set; }

        /// <summary>
        /// Valor da taxa de entrega de acordo com o bairro
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0, 999999.99, ErrorMessage = "Informe o valor do troco entre 0 e 999999,99")]
        [Display(Name = "Entrega")]
        public float TaxaEntrega { get; set; }

        /// <summary>
        /// Valor recebido em dinheiro
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0, 999999.99, ErrorMessage = "Informe o valor em dinheiro entre 0 e 999999,99")]
        [Display(Name = "Dinheiro")]
        public float RecebidoDinheiro { get; set; }

        /// <summary>
        /// Valor recebido em cartão de crédito
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0, 999999.99, ErrorMessage = "Informe o valor em cartão entre 0 e 999999,99")]
        [Display(Name = "Crédito")]
        public float RecebidoCredito { get; set; }

        /// <summary>
        /// Valor recebido em cartão de débito
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0, 999999.99, ErrorMessage = "Informe o valor em cartão entre 0 e 999999,99")]
        [Display(Name = "Débito")]
        public float RecebidoDebito { get; set; }

        /// <summary>
        /// Observações gerais do pedido
        /// </summary>
        [Display(Name = "Observações")]
        [StringLength(2000, ErrorMessage = "Informe uma observação de 0 a 2000 letras.")]
        [DataType(DataType.MultilineText)]
        public string Obs { get; set; }

        /// <summary>
        /// Identificação do cliente no qual o pedido pertence
        /// </summary>
        public Guid? IdCliente { get; set; }

        /// <summary>
        /// Nome do cliente
        /// </summary>
        [Display(Name = "Cliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Indica se o pedido foi registrado pelo IFood
        /// </summary>
        [Display(Name = "Pedido via IFood")]
        public bool PedidoIfood { get; set; }

        /// <summary>
        /// Caso o pedido seja cancelado, informar o motivo
        /// MIN.: - MAX.: 100
        /// </summary>
        [Display(Name = "Justificativa")]
        [StringLength(100, ErrorMessage = "Informe uma justificativa de 0 a 100 letras.")]
        [DataType(DataType.MultilineText)]
        public string JustificativaCancelamento { get; set; }

        /// <summary>
        /// Indica se é dia de promoção (quinta-feira)
        /// </summary>
        public bool DiaPromocao { get; set; }

        /// <summary>
        /// Lista de itens do pedido
        /// </summary>
        public List<PedidoItemModel> ListaItens { get; set; }

        /// <summary>
        /// Lista com as opções de tipos de pedido
        /// </summary>
        public List<SelectListItem> ListaTipos { get; set; }

        /// <summary>
        /// Cliente do pedido
        /// </summary>
        public ClienteModel Cliente { get; set; }

        /// <summary>
        /// Dados de entrega do pedido
        /// </summary>
        public PedidoEntregaModel Entrega { get; set; }

        /// <summary>
        /// Converte um pedido de DTO para Model
        /// </summary>
        /// <param name="pedidoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(PedidoDto pedidoDto, ref string mensagemErro)
        {
            try
            {
                Obs = string.IsNullOrWhiteSpace(pedidoDto.Obs) ? "" : pedidoDto.Obs.Trim();
                JustificativaCancelamento = string.IsNullOrWhiteSpace(pedidoDto.JustificativaCancelamento) ? "" : pedidoDto.JustificativaCancelamento.Trim().Replace("-", "");
                NomeCliente = string.IsNullOrWhiteSpace(pedidoDto.NomeCliente) ? "" : pedidoDto.NomeCliente.Trim().Replace("-", "");
                Tipo = pedidoDto.Tipo;
                Total = pedidoDto.Total;
                Troco = pedidoDto.Troco;
                TaxaEntrega = pedidoDto.TaxaEntrega;
                RecebidoDinheiro = pedidoDto.RecebidoDinheiro;
                RecebidoCredito = pedidoDto.RecebidoCredito;
                RecebidoDebito = pedidoDto.RecebidoDebito;
                IdCliente = pedidoDto.IdCliente;
                PedidoIfood = pedidoDto.PedidoIfood;
                DataAlteracao = pedidoDto.DataAlteracao;
                DataInclusao = pedidoDto.DataInclusao;
                Id = pedidoDto.Id;
                Inativo = pedidoDto.Inativo;

                // Converter os itens
                foreach (var item in pedidoDto.ListaItens)
                {
                    if (item.Quantidade > 0) // Adicionar apenas se houver quantidade preenchida
                    {
                        PedidoItemModel itemModel = new PedidoItemModel();
                        if (!itemModel.ConverterDtoParaModel(item, ref mensagemErro))
                        {
                            return false;
                        }

                        ListaItens.Add(itemModel);
                    }
                }

                // Converter cliente
                if (pedidoDto.Cliente != null)
                {
                    ClienteModel clienteModel = new ClienteModel();
                    if (!clienteModel.ConverterDtoParaModel(pedidoDto.Cliente, ref mensagemErro))
                    {
                        return false;
                    }

                    Cliente = clienteModel;
                }

                // Converter endereço
                if (pedidoDto.Entrega != null)
                {
                    PedidoEntregaModel pedidoEntregaModel = new PedidoEntregaModel();
                    if (!pedidoEntregaModel.ConverterDtoParaModel(pedidoDto.Entrega, ref mensagemErro))
                    {
                        return false;
                    }

                    Entrega = pedidoEntregaModel;
                }

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um pedido de Model para Dto
        /// </summary>
        /// <param name="pedidoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref PedidoDto pedidoDto, ref string mensagemErro)
        {
            try
            {
                pedidoDto.Obs = string.IsNullOrWhiteSpace(Obs) ? "" : Obs.Trim();
                pedidoDto.JustificativaCancelamento = string.IsNullOrWhiteSpace(JustificativaCancelamento) ? "" : JustificativaCancelamento.Trim().Replace("-", "");
                pedidoDto.NomeCliente = string.IsNullOrWhiteSpace(NomeCliente) ? "" : NomeCliente.Trim().Replace("-", "");
                pedidoDto.Tipo = Tipo;
                pedidoDto.Total = Total;
                pedidoDto.Troco = Troco;
                pedidoDto.TaxaEntrega = TaxaEntrega;
                pedidoDto.RecebidoDinheiro = RecebidoDinheiro;
                pedidoDto.RecebidoCredito = RecebidoCredito;
                pedidoDto.RecebidoDebito = RecebidoDebito;
                pedidoDto.IdCliente = IdCliente;
                pedidoDto.PedidoIfood = PedidoIfood;
                pedidoDto.DataAlteracao = DataAlteracao;
                pedidoDto.DataInclusao = DataInclusao;
                pedidoDto.Id = Id;
                pedidoDto.Inativo = Inativo;

                // Converter os itens
                foreach (var item in ListaItens)
                {
                    if (item.Quantidade > 0) // Adicionar apenas se houver quantidade preenchida
                    {
                        PedidoItemDto itemDto = new PedidoItemDto();
                        if (!item.ConverterModelParaDto(ref itemDto, ref mensagemErro))
                        {
                            return false;
                        }

                        pedidoDto.ListaItens.Add(itemDto);
                    }
                }

                // Converter cliente
                ClienteDto clienteDto = new ClienteDto();
                if (!Cliente.ConverterModelParaDto(ref clienteDto, ref mensagemErro))
                {
                    return false;
                }

                // Converter endereço
                PedidoEntregaDto pedidoEntregaDto = new PedidoEntregaDto();
                if (!Entrega.ConverterModelParaDto(ref pedidoEntregaDto, ref mensagemErro))
                {
                    return false;
                }

                pedidoDto.Cliente = clienteDto;
                pedidoDto.Entrega = pedidoEntregaDto;

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