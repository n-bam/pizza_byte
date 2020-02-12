using PizzaByteDto.Base;
using System.IO;

namespace PizzaByteDto.RetornosRequisicoes
{
    public class RetornoObterArquivoDto : RetornoDto 
    {
        /// <summary>
        /// Arquivo em formato base64
        /// </summary>
        public string ArquivoBase64 { get; set; }
    }
}
