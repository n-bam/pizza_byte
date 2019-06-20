using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa as opções de pesquisa de pedidos
    /// </summary>
    public class FiltrosPedidoModel : BaseFiltrosModel
    {
        public FiltrosPedidoModel()
        {
            ListaTipos = Utilidades.RetornarListaTiposPedido();
        }

        /// <summary>
        /// Filtrar por valor total
        /// </summary>
        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }

        /// <summary>
        /// Filtrar por tipo de pedido (balcão, retirada ou entrega)
        /// </summary>
        [Display(Name = "Tipo")]
        public TipoPedido Tipo { get; set; }

        /// <summary>
        /// Filtrar por cliente
        /// </summary>
        [Display(Name = "Cliente")]
        public Guid IdCliente { get; set; }

        /// <summary>
        /// Pesquisar um cliente para filtrar
        /// </summary>
        [Display(Name = "Cliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Pesquisar pedido por CEP
        /// </summary>
        [Display(Name = "CEP")]
        public string Cep { get; set; }

        /// <summary>
        /// Pesquisar pela data do pedido
        /// </summary>
        [Display(Name = "De")]
        public DateTime? DataInicio { get; set; }

        /// <summary>
        /// Pesquisar pela data do pedido
        /// </summary>
        [Display(Name = "até")]
        public DateTime? DataFim { get; set; }

        /// <summary>
        /// Lista com as opções de tipos de pedidos
        /// </summary>
        public List<SelectListItem> ListaTipos { get; set; }
    }
}