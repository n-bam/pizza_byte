using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa as opções de pesquisa de funcionarios
    /// </summary>
    public class FiltrosfuncionarioModel : BaseFiltrosModel
    {
        /// <summary>
        /// Pesquisar um funcionario por nome
        /// </summary>
        [Display(Name = "Nome")]
        public string Nome { get; set; }
    }
}