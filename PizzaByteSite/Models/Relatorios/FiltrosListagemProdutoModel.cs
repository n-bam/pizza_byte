using PizzaByteDto.RetornosRequisicoes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    public class FiltrosListagemProdutoModel : BaseFiltrosRelatorioModel
    {

        public FiltrosListagemProdutoModel()

        {
            ListaTipos = Utilidades.RetornarListaTiposProduto();
            ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoProduto();

            ListaTipos.Insert(0, new SelectListItem()
            {
                Text = "Todos",
                Value = "0",
                Selected = true
            });
        }

        /// <summary>
        /// Breve descritivo do produto
        /// </summary>
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        /// <summary>
        /// Preço de venda do produto
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Preço inicial")]
        public float PrecoInicio { get; set; }

        /// <summary>
        /// Preço de venda do produto
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Preço final")]
        public float PrecoFim { get; set; }

        /// <summary>
        /// Indica o tipo do produto (bebida, pizza, etc.)
        /// </summary>
        [Display(Name = "Tipo")]
        public TipoProduto Tipo { get; set; }

        /// <summary>
        /// Lista com as opções de tipos de produtos
        /// </summary>
        public List<SelectListItem> ListaTipos { get; set; }

        public bool ConverterModelParaDto(ref RequisicaoObterListagemProdutoDto requisicaoDto, ref string mensagemErro)
        {
            if (!base.ConverterModelParaDto(requisicaoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                requisicaoDto.Descricao = string.IsNullOrWhiteSpace(Descricao) ? "" : Descricao.Trim();
                requisicaoDto.PrecoInicio = PrecoInicio;
                requisicaoDto.PrecoFim = PrecoFim;
                requisicaoDto.Tipo = Tipo;

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