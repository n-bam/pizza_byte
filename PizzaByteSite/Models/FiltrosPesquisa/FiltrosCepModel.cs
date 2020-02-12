using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa as opções de pesquisa de CEPS
    /// </summary>
    public class FiltrosCepModel : BaseFiltrosModel
    {
        /// <summary>
        /// Pesquisar um endereço por CEP
        /// </summary>
        [Display(Name = "CEP")]
        public string Cep { get; set; }

        /// <summary>
        /// Pesquisar um endereço pelo logradouro
        /// </summary>
        [Display(Name = "Logradouro")]
        public string Logradouro { get; set; }

        /// <summary>
        /// Pesquisar um endereço pelo bairro
        /// </summary>
        [Display(Name = "Bairro")]
        public string Bairro { get; set; }

        /// <summary>
        /// Pesquisar um endereço pela cidade
        /// </summary>
        [Display(Name = "Cidade")]
        public string Cidade { get; set; }
    }
}