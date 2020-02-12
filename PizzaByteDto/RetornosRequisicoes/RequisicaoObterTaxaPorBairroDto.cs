
using PizzaByteDto.ClassesBase;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma taxa de entrega 
    /// </summary>
    public class RequisicaoObterTaxaPorBairroDto : BaseRequisicaoDto
    {
        /// <summary>
        /// Bairro da taxa a ser obtida
        /// </summary>
        public string BairroCidade { get; set; }
    }
}
