using PizzaByteDto.RetornosRequisicoes;
using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    public class FiltrosListagemFornecedorModel : BaseFiltrosRelatorioModel
    {
        public FiltrosListagemFornecedorModel()
        {
            ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoFornecedores();
        }

        /// <summary>
        /// Procurar com fragmentos do nome
        /// </summary>
        [Display(Name = "Nome Fantasia")]
        public string NomeFantasia { get; set; }

        /// <summary>
        /// Procurar com fragmentos da razao social
        /// </summary>
        [Display(Name = "Razão social")]
        public string RazaoSocial { get; set; }

        /// <summary>
        /// Procurar pelo telefone
        /// </summary>
        [Display(Name = "Telefone")]
        public string Telefone { get; set; }

        /// <summary>
        /// Procurar pelo Cnpj
        /// </summary>
        [Display(Name = "Cnpj")]
        public string Cnpj { get; set; }

        /// <summary>
        /// Converte de model para DTO
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref RequisicaoObterListagemFornecedoresDto requisicaoDto, ref string mensagemErro)
        {
            if (!base.ConverterModelParaDto(requisicaoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                requisicaoDto.Cnpj = string.IsNullOrWhiteSpace(Cnpj) ? "" : Cnpj.Trim().Replace("-", "").Replace(".", "").Replace("/", "");
                requisicaoDto.Telefone = string.IsNullOrWhiteSpace(Telefone) ? "" : Telefone.Trim().Replace("-", "").Replace("(", "").Replace(")", "");
                requisicaoDto.NomeFantasia = string.IsNullOrWhiteSpace(NomeFantasia) ? "" : NomeFantasia.Trim();
                requisicaoDto.RazaoSocial = string.IsNullOrWhiteSpace(RazaoSocial) ? "" : RazaoSocial.Trim();

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