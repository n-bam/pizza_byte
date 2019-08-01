using PizzaByteDto.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de uma taxa de entrega no site
    /// </summary>
    public class TaxaEntregaModel : BaseModel
    {
        /// <summary>
        /// Bairro que a taxa de entrega abrange
        /// MIN.: 3 / MAX.: 50
        /// </summary>
        [Required(ErrorMessage = "Por favor, preencha o bairro da taxa de entrega. Ex.: ")]
        [Display(Name = "Bairro")]
        [StringLength(50, ErrorMessage = "Informe o bairro para a taxa de entrega de 3 a 150 letras.", MinimumLength = 3)]
        public string Bairro { get; set; }

        /// <summary>
        /// Valor de entrega para o bairro correspondente
        /// </summary>
        [Required(ErrorMessage = "Por favor, preencha o valor da taxa de entrega")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0, 999999.99, ErrorMessage = "Informe o valor da taxa de entrega")]
        [Display(Name = "Valor")]
        public float ValorTaxa { get; set; }

        /// <summary>
        /// Cidade que o bairro pertence
        /// </summary>
        [Display(Name = "Cidade")]
        public string Cidade { get; set; }
        
        /// <summary>
        /// Converte uma taxa de entrega de DTO para Model
        /// </summary>
        /// <param name="taxaDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(TaxaEntregaDto taxaDto, ref string mensagemErro)
        {
            try
            {
                Bairro = string.IsNullOrWhiteSpace(taxaDto.Bairro) ? "" : taxaDto.Bairro.Trim();
                Cidade = string.IsNullOrWhiteSpace(taxaDto.Cidade) ? "" : taxaDto.Cidade.Trim();
                ValorTaxa = taxaDto.ValorTaxa;
                DataAlteracao = taxaDto.DataAlteracao;
                DataInclusao = taxaDto.DataInclusao;
                Id = taxaDto.Id;
                Inativo = taxaDto.Inativo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte uma taxa de entrega de Model para Dto
        /// </summary>
        /// <param name="taxaDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref TaxaEntregaDto taxaDto, ref string mensagemErro)
        {
            try
            {
                taxaDto.Bairro = string.IsNullOrWhiteSpace(Bairro) ? "" : Bairro.Trim();
                taxaDto.Cidade = string.IsNullOrWhiteSpace(Cidade) ? "" : Cidade.Trim();
                taxaDto.ValorTaxa = ValorTaxa;
                taxaDto.DataAlteracao = this.DataAlteracao;
                taxaDto.DataInclusao = this.DataInclusao;
                taxaDto.Id = this.Id;
                taxaDto.Inativo = this.Inativo;

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