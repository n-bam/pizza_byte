using PizzaByteDto.RetornosRequisicoes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    public class FiltrosMelhoresProdutosModel : BaseFiltrosRelatorioModel
    {
        public FiltrosMelhoresProdutosModel()
        {
            ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoMelhoresProdutos();
            ListaTipos = Utilidades.RetornarListaTiposProduto();

            ListaTipos.Insert(0, new SelectListItem()
            {
                Text = "Todos",
                Value = "0",
                Selected = true
            });
        }

        /// <summary>
        /// Procurar por um tipo de produto
        /// </summary>
        [Display(Name = "Tipo")]
        public TipoProduto Tipo { get; set; }

        /// <summary>
        /// Lista com os tipos de produtos disponíveis
        /// </summary>
        public List<SelectListItem> ListaTipos { get; set; }

        /// <summary>
        /// Converte de model para DTO
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref RequisicaoObterMelhoresProdutosDto requisicaoDto, ref string mensagemErro)
        {
            if (!base.ConverterModelParaDto(requisicaoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
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