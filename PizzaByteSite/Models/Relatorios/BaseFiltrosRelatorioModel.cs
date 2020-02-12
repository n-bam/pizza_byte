using PizzaByteDto.RetornosRequisicoes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PizzaByteSite.Models
{
    public class BaseFiltrosRelatorioModel
    {
        public BaseFiltrosRelatorioModel()
        {
            ListaInativo = Utilidades.RetornarListaOpcaoInativo();
        }

        /// <summary>
        /// Indica se deve-se obter os cadastros inativos
        /// </summary>
        [Display(Name = "Inativos")]
        public string ObterInativos { get; set; }

        /// <summary>
        /// Cadastrados entre, data inicial
        /// </summary>
        [Display(Name = "Cadastrados de")]
        public DateTime? DataCadastroInicial { get; set; }

        /// <summary>
        /// Cadastrados entre, data final
        /// </summary>
        [Display(Name = "até")]
        public DateTime? DataCadastroFinal { get; set; }

        /// <summary>
        /// Campo de ordem dos registros
        /// </summary>
        [Display(Name = "Ordenar por")]
        public string CampoOrdem { get; set; }

        /// <summary>
        /// Lista com as opções de pesquisa de entidades ativas/inativas
        /// </summary>
        public List<SelectListItem> ListaInativo { get; set; }

        /// <summary>
        /// Lista com as opções de ordenação das entidades
        /// </summary>
        public List<SelectListItem> ListaCampoOrdem { get; set; }

        /// <summary>
        /// Converte para uma requisição DTO
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="menagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(RequisicaoObterRelatorioListagemDto requisicaoDto, ref string menagemErro)
        {
            try
            {
                requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(CampoOrdem) ? "" : CampoOrdem.Trim().ToUpper();
                requisicaoDto.DataCadastroFinal = DataCadastroFinal;
                requisicaoDto.DataCadastroInicial = DataCadastroInicial;
                requisicaoDto.ObterInativos = string.IsNullOrWhiteSpace(ObterInativos) ? "" : ObterInativos.Trim().ToUpper();

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