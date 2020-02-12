using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa as opções de pesquisa de taxas
    /// </summary>
    public class FiltrosTaxaEntregaModel : BaseFiltrosModel
    {
        public FiltrosTaxaEntregaModel()
        {
            ListaOrdenacao = new List<SelectListItem>();
            ListaOrdenacao.Add(new SelectListItem() { Text = "Bairro", Value = "BAIRRO", Selected = true });
            ListaOrdenacao.Add(new SelectListItem() { Text = "Menor preço", Value = "VALORTAXADECRESCENTE" });
            ListaOrdenacao.Add(new SelectListItem() { Text = "Maior preço", Value = "VALORTAXACRESCENTE" });
        }

        /// <summary>
        /// Bairro que a taxa se refere
        /// </summary>
        [Display(Name = "Bairro")]
        public string Bairro { get; set; }
        
        /// <summary>
        /// Indica o campo que a pesquisa será ordenada
        /// </summary>
        [Display(Name = "Ordenar por")]
        public string CampoOrdenacao { get; set; }

        /// <summary>
        /// Faixa inicial de preço
        /// </summary>
        [Display(Name = "Taxa de")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float TaxaInicial { get; set; }

        /// <summary>
        /// Faixa inicial de preço
        /// </summary>
        [Display(Name = "até")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float TaxaFinal { get; set; }
        
        /// <summary>
        /// Lista com as opções de ordenação da pesquisa
        /// </summary>
        public List<SelectListItem> ListaOrdenacao { get; set; }
    }
}