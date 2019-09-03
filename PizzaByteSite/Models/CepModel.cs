using PizzaByteDto.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um CEP
    /// </summary>
    public class CepModel : BaseModel
    {
        public CepModel()
        {
            Cidade = "Americana";
        }

        /// <summary>
        /// Rua ou avenida correspondente ao CEP
        /// </summary>
        [Display(Name = "Logradouro")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Informe um logradouro de 3 a 150 letras.")]
        public string Logradouro { get; set; }

        /// <summary>
        /// Cidade correspondente ao CEP
        /// </summary>
        [Display(Name = "Cidade")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Informe uma cidade de 2 a 50 letras.")]
        public string Cidade { get; set; }

        /// <summary>
        /// Bairro correspondente ao CEP
        /// </summary>
        [Display(Name = "Bairro")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Informe uma cidade de 2 a 50 letras.")]
        public string Bairro { get; set; }

        /// <summary>
        /// Chave que identifica unicamente o CEP
        /// </summary>
        [Display(Name = "CEP")]
        [StringLength(9, MinimumLength = 8, ErrorMessage = "Informe um CEP válido")]
        public string Cep { get; set; }

        /// <summary>
        /// Converte um cep de DTO para Model
        /// </summary>
        /// <param name="cepDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(CepDto cepDto, ref string mensagemErro)
        {
            try
            {
                this.Bairro = string.IsNullOrWhiteSpace(cepDto.Bairro) ? "" : cepDto.Bairro.Trim().Replace(".", "").Replace("-", "").Replace("-", "");
                this.Cep = string.IsNullOrWhiteSpace(cepDto.Cep) ? "" : cepDto.Cep.Trim();
                this.Cidade = string.IsNullOrWhiteSpace(cepDto.Cidade) ? "" : cepDto.Cidade.Trim();
                this.Logradouro = string.IsNullOrWhiteSpace(cepDto.Logradouro) ? "" : cepDto.Logradouro.Trim();
                this.DataAlteracao = cepDto.DataAlteracao;
                this.DataInclusao = cepDto.DataInclusao;
                this.Id = cepDto.Id;
                this.Inativo = cepDto.Inativo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um cep de Model para Dto
        /// </summary>
        /// <param name="cepDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref CepDto cepDto, ref string mensagemErro)
        {
            try
            {
                cepDto.Bairro = string.IsNullOrWhiteSpace(this.Bairro) ? "" : this.Bairro.Trim();
                cepDto.Cep = string.IsNullOrWhiteSpace(this.Cep) ? "" : this.Cep.Trim().Replace("-", "");
                cepDto.Cidade = string.IsNullOrWhiteSpace(this.Cidade) ? "" : this.Cidade.Trim();
                cepDto.Logradouro = string.IsNullOrWhiteSpace(this.Logradouro) ? "" : this.Logradouro.Trim();
                cepDto.DataAlteracao = this.DataAlteracao;
                cepDto.DataInclusao = this.DataInclusao;
                cepDto.Id = this.Id;
                cepDto.Inativo = this.Inativo;

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