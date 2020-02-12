using PizzaByteDto.RetornosRequisicoes;
using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    public class FiltrosListagemEntregaModel : BaseFiltrosRelatorioModel
    {
        public FiltrosListagemEntregaModel()
        {
            ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoEntrega();
        }

        /// <summary>
        /// Procurar com fragmentos do endereço
        /// </summary>
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }

        /// <summary>
        /// Procurar com fragmentos do funcionario
        /// </summary>
        [Display(Name = "Funcionario")]
        public string Funcionario { get; set; }

             /// <summary>
        /// Procurar pelo telefone
        /// </summary>
        [Display(Name = "Taxa de")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float ValorInicio { get; set; }

        /// <summary>
        /// Procurar pelo telefone
        /// </summary>
        [Display(Name = "Até")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float ValorFim { get; set; }

        /// <summary>
        /// Converte de model para DTO
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref RequisicaoObterListagemEntregasDto requisicaoDto, ref string mensagemErro)
        {
            if (!base.ConverterModelParaDto(requisicaoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                requisicaoDto.Endereco = string.IsNullOrWhiteSpace(Endereco) ? "" : Endereco.Trim();
                requisicaoDto.Funcionario = string.IsNullOrWhiteSpace(Funcionario) ? "" : Funcionario.Trim();
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