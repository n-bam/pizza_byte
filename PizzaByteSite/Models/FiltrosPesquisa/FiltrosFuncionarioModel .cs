using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;
namespace PizzaByteSite.Models
{

    /// <summary>
    /// Classe que representa as opções de pesquisa de funcionarios
    /// </summary>
    public class FiltrosFuncionarioModel : BaseFiltrosModel
        {
            public FiltrosFuncionarioModel()
            {
                ListaTipos = Utilidades.RetornarListaTiposFuncionario();
               
            }

            /// <summary>
            /// Pesquisar um funcionario por nome
            /// </summary>
            [Display(Name = "Nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Filtrar por tipo de produto (Motoboy, Atendente, Cozinheiro, Gestor)
        /// </summary>
        [Display(Name = "Tipo")]
        public TipoFuncionario Tipo { get; set; }
        
        /// <summary>
        /// Indica o campo que a pesquisa será ordenada
        /// </summary>
        [Display(Name = "Ordenar por")]
        public string CampoOrdenacao { get; set; }


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