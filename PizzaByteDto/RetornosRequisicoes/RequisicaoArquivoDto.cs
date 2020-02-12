using PizzaByteDto.ClassesBase;

namespace PizzaByteDto.RetornosRequisicoes
{
    public class RequisicaoArquivoDto : BaseRequisicaoDto
    {
        /// <summary>
        /// Arquivo em formato base64
        /// </summary>
        public string ArquivoBase64 { get; set; }
    }
}
