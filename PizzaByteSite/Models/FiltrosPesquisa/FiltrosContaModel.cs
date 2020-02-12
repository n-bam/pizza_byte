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
    public class FiltrosContaModel : BaseFiltrosModel
    {
        public FiltrosContaModel()
        {
            ListaStatus = Utilidades.RetornarListaStatusConta();

            ListaOrdenacao = new List<SelectListItem>();
            ListaOrdenacao.Add(new SelectListItem() { Text = "Descrição", Value = "DESCRICAO", Selected = true });
            ListaOrdenacao.Add(new SelectListItem() { Text = "Menor valor", Value = "PRECODECRESCENTE" });
            ListaOrdenacao.Add(new SelectListItem() { Text = "Maior valor", Value = "PRECOCRESCENTE" });

            ListaOpcaoData = new List<SelectListItem>();
        }

        /// <summary>
        /// Pesquisar pela data da conta
        /// </summary>
        [Display(Name = "Pesquisa com")]
        public string PesquisarPor { get; set; }

        /// <summary>
        /// Pesquisar pela data da conta
        /// </summary>
        [Display(Name = "De")]
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Pesquisar pela data da conta
        /// </summary>
        [Display(Name = "até")]
        public DateTime DataFim { get; set; }

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
        public float PrecoInicial { get; set; }

        /// <summary>
        /// Faixa final de valor da conta
        /// </summary>
        [Display(Name = "até")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float PrecoFinal { get; set; }

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
        public List<SelectListItem> ListaOrdenacao { get; set; }

        /// <summary>
        /// Lista com as opções de ordenação da pesquisa
        /// </summary>
        public List<SelectListItem> ListaOpcaoData { get; set; }
    }
}