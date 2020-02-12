using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PizzaByteSite.Models
{
    public class BaseFiltrosModel
    {
        public BaseFiltrosModel()
        {
            ListaInativo = Utilidades.RetornarListaOpcaoInativo();
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
        /// Número de itens que terá em cada página
        /// </summary>
        public int NumeroItensPagina { get; set; }

        /// <summary>
        /// Lista com as opções de pesquisa de entidades ativas/inativas
        /// </summary>
        public List<SelectListItem> ListaInativo { get; set; }
    }
}