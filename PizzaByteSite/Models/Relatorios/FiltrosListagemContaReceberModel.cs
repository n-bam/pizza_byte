using PizzaByteDto.RetornosRequisicoes;
using System.Web.Mvc;

namespace PizzaByteSite.Models
{
    public class FiltrosListagemContaReceberModel : FiltrosListagemContasModel
    {
        public FiltrosListagemContaReceberModel()
        {
            ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoContaPagar();
        }

        /// <summary>
        /// Converte de model para DTO
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref RequisicaoObterListagemContaReceberDto requisicaoDto, ref string mensagemErro)
        {
            return base.ConverterModelParaDto(requisicaoDto, ref mensagemErro);
        }
    }
}