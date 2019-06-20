
using PizzaByteDto.ClassesBase;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter um CEP
    /// </summary>
    public class RequisicaoObterCepPorCepDto : BaseRequisicaoDto
    {
        /// <summary>
        /// CEP a ser obtido
        /// </summary>
        public string Cep { get; set; }
    }
}
