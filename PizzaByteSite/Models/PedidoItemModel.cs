using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um pedido no site
    /// </summary>
    public class PedidoItemModel : BaseModel
    {
        public PedidoItemModel()
        {
            ListaTipos = Utilidades.RetornarListaTiposPedido();
        }

        /// <summary>
        /// Breve descritivo do produto
        /// </summary>
        [Required(ErrorMessage = "Por favor, preencha a descrição do produto. Ex.: Pizza de frango")]
        [Display(Name = "Descrição")]
        [StringLength(150, ErrorMessage = "Informe a descrição para o produto de 3 a 150 letras.", MinimumLength = 3)]
        public string DescricaoProduto { get; set; }

        /// <summary>
        /// Preço de venda do produto
        /// </summary>
        [Required(ErrorMessage = "Por favor, preencha o preço de venda do produto")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0.01, 999999.99, ErrorMessage = "Informe o preço de venda")]
        [Display(Name = "Preço")]
        public double PrecoProduto { get; set; }

        /// <summary>
        /// Indica o tipo do produto (bebida, pizza, etc.)
        /// </summary>
        [Required(ErrorMessage = "Por favor, selecione qual o tipo do produto")]
        [Display(Name = "Tipo")]
        public TipoProduto TipoProduto { get; set; }

        [Display(Name = "Quantidade")]
        [Required(ErrorMessage = "Por favor, informe a quantidade de cada item")]
        [Range(0.5, 999999.99, ErrorMessage = "Informe uma quantidade válida")]
        public double Quantidade { get; set; }

        /// <summary>
        /// Indica qual é o outro sabor de uma pizza meio a meio
        /// </summary>
        public Guid IdPedidoItem { get; set; }

        /// <summary>
        /// Identifica o produto adicionado no pedido
        /// </summary>
        public Guid IdProduto { get; set; }

        /// <summary>
        /// Identifica o pedido que o item pertence
        /// </summary>
        public Guid IdPedido { get; set; }

        /// <summary>
        /// Lista com as opções de tipos de pedido
        /// </summary>
        public List<SelectListItem> ListaTipos { get; set; }

    }
}