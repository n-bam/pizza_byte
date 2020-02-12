using PizzaByteDto.RetornosRequisicoes;
using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    public class FiltrosListagemTaxaEntregaModel : BaseFiltrosRelatorioModel
    {
        public FiltrosListagemTaxaEntregaModel()
        {
            ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoTaxaEntrega();
        }

        /// <summary>
        /// Procurar com fragmentos do nome
        /// </summary>
        [Display(Name = "Bairro")]
        public string BairroCidade { get; set; }

        /// <summary>
        /// Procurar pelo telefone
        /// </summary>
        [Display(Name = "Valor inicial")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float ValorInicio { get; set; }

        /// <summary>
        /// Procurar pelo telefone
        /// </summary>
        [Display(Name = "Valor Final")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float ValorFim { get; set; }

        /// <summary>
        /// Converte de model para DTO
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref RequisicaoObterListagemTaxaEntregaDto requisicaoDto, ref string mensagemErro)
        {
            if (!base.ConverterModelParaDto(requisicaoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                requisicaoDto.BairroCidade = string.IsNullOrWhiteSpace(BairroCidade) ? "" : BairroCidade.Trim();
                requisicaoDto.ValorInicio = ValorInicio;
                requisicaoDto.ValorFim = ValorFim;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }
    }
}