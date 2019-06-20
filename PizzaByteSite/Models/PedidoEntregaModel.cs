using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um produto no site
    /// </summary>
    public class PedidoEntregaModel : BaseModel
    {
        /// <summary>
        /// Indica se o retorno da entrega já teve os valores conferidos
        /// </summary>
        [Display(Name = "Conferido")]
        public bool Conferido { get; set; }

        /// <summary>
        /// Valor em dinheiro retornado na entrega
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0.01, 999999.99, ErrorMessage = "Informe o preço de venda")]
        [Display(Name = "Valor retornado")]
        public double ValorRetorno { get; set; }

        /// <summary>
        /// Observações gerais da entrega
        /// </summary>
        [Display(Name = "Observações")]
        [StringLength(2000, ErrorMessage = "Informe uma observação de 0 a 2000 letras.")]
        public string Obs { get; set; }

        /// <summary>
        /// Identificação do endereço de entrega
        /// </summary>
        public Guid IdEndereco { get; set; }

        /// <summary>
        /// Identifica o funcionário que fez a entrega
        /// </summary>
        public Guid IdFuncionario { get; set; }
    }
}