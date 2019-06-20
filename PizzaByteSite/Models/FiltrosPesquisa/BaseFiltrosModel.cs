using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PizzaByteSite.Models
{
    public class BaseFiltrosModel
    {
        public BaseFiltrosModel()
        {
            ListaInativo = new List<SelectListItem>();
            ListaInativo.Add(new SelectListItem() { Text = "Todos", Value = "" });
            ListaInativo.Add(new SelectListItem() { Text = "Apenas ativos", Value = "false", Selected = true });
            ListaInativo.Add(new SelectListItem() { Text = "Apenas inativos", Value = "true" });
        }

        /// <summary>
        /// Indica a página a ser exibida
        /// </summary>
        public int Pagina { get; set; }

        /// <summary>
        /// Indica se o retorno é paginado
        /// </summary>
        public bool NaoPaginaPesquisa { get; set; }

        /// <summary>
        /// Indica se deve-se obter os cadastros inativos
        /// </summary>
        [Display(Name = "Inativos")]
        public string ObterInativos { get; set; }

        /// <summary>
        /// Lista com as opções de pesquisa de entidades ativas/inativas
        /// </summary>
        public List<SelectListItem> ListaInativo { get; set; }
    }
}