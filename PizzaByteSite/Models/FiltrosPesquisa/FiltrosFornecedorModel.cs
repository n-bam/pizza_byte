using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa as opções de pesquisa de fornecedores
    /// </summary>
    public class FiltrosFornecedorModel : BaseFiltrosModel
    {
        /// <summary>
        /// Pesquisar um fornecedor por nome fantasia
        /// </summary>
        [Display(Name = "Nome fantasia")]
        public string NomeFantasia { get; set; }

        /// <summary>
        /// Pesquisar um fornecedor por razão social
        /// </summary>
        [Display(Name = "Razão social")]
        public string RazaoSocial { get; set; }

        /// <summary>
        /// Pesquisar um fornecedor pelo cnpj
        /// </summary>
        [Display(Name = "CNPJ")]
        public string Cnpj { get; set; }
    }
}