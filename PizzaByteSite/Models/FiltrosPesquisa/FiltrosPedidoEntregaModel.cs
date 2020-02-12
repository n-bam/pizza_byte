using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa as opções de pesquisa de entregas
    /// </summary>
    public class FiltrosPedidoEntregaModel : BaseFiltrosModel
    {
        public FiltrosPedidoEntregaModel()
        {
            ListaFuncionarios = Utilidades.RetornarListaFuncionarios(TipoFuncionario.Motoboy);
            ListaInativo = Utilidades.RetornarListaOpcaoInativo();

            ListaConferidos = new List<SelectListItem>();
            ListaConferidos.Add(new SelectListItem() { Text = "Todos", Value = "" });
            ListaConferidos.Add(new SelectListItem() { Text = "Apenas conferidos", Value = "true", Selected = true });
            ListaConferidos.Add(new SelectListItem() { Text = "Apenas não conferidos", Value = "false" });
        }

        /// <summary>
        /// Filtrar por valor retornado
        /// </summary>
        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float ValorRetorno { get; set; }
        
        /// <summary>
        /// Filtrar por funcionário
        /// </summary>
        [Display(Name = "Funcionário")]
        public Guid? IdFuncionario { get; set; }

        /// <summary>
        /// Pesquisar entregas conferidas
        /// </summary>
        [Display(Name = "Conferido")]
        public string Conferido { get; set; }

        /// <summary>
        /// Pesquisar pela data de inclusão
        /// </summary>
        [Display(Name = "De")]
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Pesquisar pela data de inclusão
        /// </summary>
        [Display(Name = "até")]
        public DateTime DataFim { get; set; }
        
        /// <summary>
        /// Lista com as opções de tipos de pedidos
        /// </summary>
        public List<SelectListItem> ListaFuncionarios { get; set; }

        /// <summary>
        /// Lista com as opções de pedidos do IFood
        /// </summary>
        public List<SelectListItem> ListaConferidos { get; set; }
    }
}