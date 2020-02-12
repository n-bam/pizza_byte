using PizzaByteDto.RetornosRequisicoes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    public class FiltrosRelacaoContasModel : BaseFiltrosRelatorioModel
    {
        public FiltrosRelacaoContasModel()
        {
            ListaStatus = Utilidades.RetornarListaStatusConta();
            ListaStatus.Insert(0, new SelectListItem()
            {
                Text = "Todos",
                Value = "0"
            });

            ListaOpcoesPesquisa = Utilidades.RetornarOpcoesPesquisaContas();
        }

        /// <summary>
        /// Indica qual a data de pesquisa
        /// </summary>
        [Display(Name = "Pesquisa com")]
        public string PesquisarPor { get; set; }

        /// <summary>
        /// Indica se é para trazer as contas estornadas
        /// </summary>
        [Display(Name = "Obter estornadas")]
        public bool IndicadorEstornadas { get; set; }

        /// <summary>
        /// Indica se é para trazer as contas perdidas
        /// </summary>
        [Display(Name = "Obter perdidas")]
        public bool IndicadorPerdida { get; set; }

        /// <summary>
        /// Filtrar por tipo de conta (Paga, Aberta, Perdida, etc)
        /// </summary>
        [Display(Name = "Status")]
        public StatusConta Status { get; set; }

        /// <summary>
        /// Lista com as opções de pesquisa
        /// </summary>
        public List<SelectListItem> ListaOpcoesPesquisa { get; set; }

        /// <summary>
        /// Lista com as opções de tipos de conta
        /// </summary>
        public List<SelectListItem> ListaStatus { get; set; }

        /// <summary>
        /// Converte de model para DTO
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref RequisicaoObterRelacaoContasDto requisicaoDto, ref string mensagemErro)
        {
            if (!base.ConverterModelParaDto(requisicaoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                requisicaoDto.PesquisarPor = string.IsNullOrWhiteSpace(PesquisarPor) ? "" : PesquisarPor.Trim().ToUpper();
                requisicaoDto.IndicadorEstornadas = IndicadorEstornadas;
                requisicaoDto.IndicadorPerdida = IndicadorPerdida;
                requisicaoDto.Status = Status;

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