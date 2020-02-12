using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa as opções de pesquisa de clientes
    /// </summary>
    public class FiltrosClienteModel : BaseFiltrosModel
    {
        /// <summary>
        /// Pesquisar um cliente por nome
        /// </summary>
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Pesquisar um cliente pelo CPF
        /// </summary>
        [Display(Name = "CPF")]
        public string Cpf { get; set; }

        /// <summary>
        /// Obter um cliente por telefone
        /// </summary>
        [Display(Name = "Telefone")]
        public string Telefone { get; set; }
    }
}