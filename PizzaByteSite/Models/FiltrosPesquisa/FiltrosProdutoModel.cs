using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa as opções de pesquisa de produtos
    /// </summary>
    public class FiltrosProdutoModel : BaseFiltrosModel
    {
        public FiltrosProdutoModel()
        {
            ListaTipos = Utilidades.RetornarListaTiposProduto();

            ListaOrdenacao = new List<SelectListItem>();
            ListaOrdenacao.Add(new SelectListItem() { Text = "Descrição", Value = "DESCRICAO", Selected = true });
            ListaOrdenacao.Add(new SelectListItem() { Text = "Menor preço", Value = "PRECODECRESCENTE" });
            ListaOrdenacao.Add(new SelectListItem() { Text = "Maior preço", Value = "PRECOCRESCENTE" });
        }

        /// <summary>
        /// Breve descritivo do produto
        /// </summary>
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        /// <summary>
        /// Filtrar por tipo de produto (bebida, pizza, etc.)
        /// </summary>
        [Display(Name = "Tipo")]
        public TipoProduto Tipo { get; set; }

        /// <summary>
        /// Indica o campo que a pesquisa será ordenada
        /// </summary>
        [Display(Name = "Ordenar por")]
        public string CampoOrdenacao { get; set; }

        /// <summary>
        /// Faixa inicial de preço
        /// </summary>
        [Display(Name = "Preço de")]
        public float PrecoInicial { get; set; }

        /// <summary>
        /// Faixa inicial de preço
        /// </summary>
        [Display(Name = "até")]
        public float PrecoFinal { get; set; }

        /// <summary>
        /// Lista com as opções de tipos de produtos
        /// </summary>
        public List<SelectListItem> ListaTipos { get; set; }

        /// <summary>
        /// Lista com as opções de ordenação da pesquisa
        /// </summary>
        public List<SelectListItem> ListaOrdenacao { get; set; }
    }
}