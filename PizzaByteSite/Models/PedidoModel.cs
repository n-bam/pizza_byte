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
    public class PedidoModel : BaseModel
    {
        public PedidoModel()
        {
            ListaTipos = Utilidades.RetornarListaTiposPedido();
            ListaItens = new List<PedidoItemModel>();
            ClienteEndereco = new ClienteEnderecoModel();
        }

        /// <summary>
        /// Indica se é entrega, retirada ou balcão
        /// </summary>
        [Required(ErrorMessage = "Por favor, selecione qual o tipo do produto")]
        [Display(Name = "Tipo")]
        public TipoPedido Tipo { get; set; }

        /// <summary>
        /// Valor total dos pedidos (soma dos itens + frete)
        /// </summary>
        [Required(ErrorMessage = "Por favor, adicione ao menos um produto")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0.01, 999999.99, ErrorMessage = "Informe o valor total do pedido")]
        [Display(Name = "Total")]
        public double Total { get; set; }

        /// <summary>
        /// Valor de troco necessário 
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0, 999999.99, ErrorMessage = "Informe o valor do troco entre 0 e 999999,99")]
        [Display(Name = "Troco")]
        public double Troco { get; set; }

        /// <summary>
        /// Valor da taxa de entrega de acordo com o bairro
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0, 999999.99, ErrorMessage = "Informe o valor do troco entre 0 e 999999,99")]
        [Display(Name = "Taxa de entrega")]
        public double TaxaEntrega { get; set; }

        /// <summary>
        /// Valor recebido em dinheiro
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0, 999999.99, ErrorMessage = "Informe o valor em dinheiro entre 0 e 999999,99")]
        [Display(Name = "Dinheiro")]
        public double RecebidoDinheiro { get; set; }

        /// <summary>
        /// Valor recebido em cartão
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0, 999999.99, ErrorMessage = "Informe o valor em cartão entre 0 e 999999,99")]
        [Display(Name = "Cartão")]
        public double RecebidoCartao { get; set; }

        /// <summary>
        /// Observações gerais do pedido
        /// </summary>
        [Display(Name = "Observações")]
        [StringLength(2000, ErrorMessage = "Informe uma observação de 0 a 2000 letras.")]
        [DataType(DataType.MultilineText)]
        public string Obs { get; set; }

        /// <summary>
        /// Identificação do cliente no qual o pedido pertence
        /// </summary>
        [Required(ErrorMessage = "Por favor, informe o cliente do pedido")]
        public Guid IdCliente { get; set; }

        [Display(Name = "Cliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Lista de itens do pedido
        /// </summary>
        public List<PedidoItemModel> ListaItens { get; set; }

        /// <summary>
        /// Lista com as opções de tipos de pedido
        /// </summary>
        public List<SelectListItem> ListaTipos { get; set; }

        /// <summary>
        /// Dados do endereço do cliente
        /// </summary>
        public ClienteEnderecoModel ClienteEndereco { get; set; }
    }
}