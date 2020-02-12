using PizzaByteDto.RetornosRequisicoes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa as opções de pesquisa de uma conta
    /// </summary>
    public class FiltrosListagemContasModel : BaseFiltrosRelatorioModel
    {
        public FiltrosListagemContasModel()
        {
            ListaStatus = Utilidades.RetornarListaStatusConta();
            ListaStatus.Add(new SelectListItem()
            {
                Text = "Todos",
                Value = "0"
            });

            ListaCampoOrdem = new List<SelectListItem>();
            ListaCampoOrdem.Add(new SelectListItem() { Text = "Descrição", Value = "DESCRICAO", Selected = true });
            ListaCampoOrdem.Add(new SelectListItem() { Text = "Menor valor", Value = "PRECODECRESCENTE" });
            ListaCampoOrdem.Add(new SelectListItem() { Text = "Maior valor", Value = "PRECOCRESCENTE" });

            ListaOpcaoPesquisa = new List<SelectListItem>();
            ListaOpcaoPesquisa.Add(new SelectListItem() { Text = "Vencimento", Value = "DATAVENCIMENTO", Selected = true });
            ListaOpcaoPesquisa.Add(new SelectListItem() { Text = "Competência", Value = "DATACOMPETENCIA" });
        }

        /// <summary>
        /// Pesquisar pela data da conta
        /// </summary>
        [Display(Name = "Pesquisa com")]
        public string PesquisarPor { get; set; }

        /// <summary>
        /// Pesquisar uma conta por descrição
        /// </summary>
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        /// <summary>
        /// Faixa inicial de valor da conta
        /// </summary>
        [Display(Name = "Valor de")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float PrecoInicio { get; set; }

        /// <summary>
        /// Faixa final de valor da conta
        /// </summary>
        [Display(Name = "até")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float PrecoFim { get; set; }

        /// <summary>
        /// Filtrar por tipo de conta (Paga, Aberta, Perdida, etc)
        /// </summary>
        [Display(Name = "Status")]
        public StatusConta Status { get; set; }

        /// <summary>
        /// Lista com as opções de tipos de conta
        /// </summary>
        public List<SelectListItem> ListaStatus { get; set; }

        /// <summary>
        /// Lista com as opções de ordenação da pesquisa
        /// </summary>
        public List<SelectListItem> ListaOpcaoPesquisa { get; set; }

        /// <summary>
        /// Converte de model para dto
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="menagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(RequisicaoObterListagemContasDto requisicaoDto, ref string menagemErro)
        {
            try
            {
                requisicaoDto.PesquisarPor = string.IsNullOrWhiteSpace(PesquisarPor) ? "" : PesquisarPor.Trim().ToUpper();
                requisicaoDto.Descricao = string.IsNullOrWhiteSpace(Descricao) ? "" : Descricao.Trim().ToUpper();
                requisicaoDto.DataCadastroFinal = DataCadastroFinal;
                requisicaoDto.DataCadastroInicial = DataCadastroInicial;
                requisicaoDto.PrecoInicial = PrecoInicio;
                requisicaoDto.PrecoFinal = PrecoFim;
                requisicaoDto.Status = Status;
                requisicaoDto.CampoOrdem = CampoOrdem;

                return true;
            }
            catch (Exception ex)
            {
                menagemErro = ex.Message;
                return false;
            }

        }

    }
}