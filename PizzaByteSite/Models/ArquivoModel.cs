using PizzaByteDto.Base;
using PizzaByteDto.RetornosRequisicoes;
using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um CEP
    /// </summary>
    public class ArquivoModel
    {
        /// <summary>
        /// Chave que identifica unicamente o CEP
        /// </summary>
        [Required]
        [Display(Name = "Arquivo")]
        public string ArquivoBase64 { get; set; }

        /// <summary>
        /// Converte o arquivo para dto
        /// </summary>
        /// <param name="arquivoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(RequisicaoArquivoDto arquivoDto, ref string mensagemErro)
        {
            try
            {
                ArquivoBase64 = string.IsNullOrWhiteSpace(arquivoDto.ArquivoBase64) ? "" : arquivoDto.ArquivoBase64.Trim();

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte o arquivo para dto
        /// </summary>
        /// <param name="arquivoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref RequisicaoArquivoDto arquivoDto, ref string mensagemErro)
        {
            try
            {
                arquivoDto.ArquivoBase64 = string.IsNullOrWhiteSpace(this.ArquivoBase64) ? "" : this.ArquivoBase64.Trim();

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