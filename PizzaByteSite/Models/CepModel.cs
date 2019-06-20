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
        /// <param name="fornecedorDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(CepDto fornecedorDto, ref string mensagemErro)
        {
            try
            {
                this.Bairro = string.IsNullOrWhiteSpace(fornecedorDto.Bairro) ? "" : fornecedorDto.Bairro.Trim().Replace(".", "").Replace("-", "").Replace("-", "");
                this.Cep = string.IsNullOrWhiteSpace(fornecedorDto.Cep) ? "" : fornecedorDto.Cep.Trim();
                this.Cidade = string.IsNullOrWhiteSpace(fornecedorDto.Cidade) ? "" : fornecedorDto.Cidade.Trim();
                this.Logradouro = string.IsNullOrWhiteSpace(fornecedorDto.Logradouro) ? "" : fornecedorDto.Logradouro.Trim();
                this.DataAlteracao = fornecedorDto.DataAlteracao;
                this.DataInclusao = fornecedorDto.DataInclusao;
                this.Id = fornecedorDto.Id;
                this.Inativo = fornecedorDto.Inativo;

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
        /// <param name="fornecedorDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref CepDto fornecedorDto, ref string mensagemErro)
        {
            try
            {
                fornecedorDto.Bairro = string.IsNullOrWhiteSpace(this.Bairro) ? "" : this.Bairro.Trim();
                fornecedorDto.Cep = string.IsNullOrWhiteSpace(this.Cep) ? "" : this.Cep.Trim().Replace("-", "");
                fornecedorDto.Cidade = string.IsNullOrWhiteSpace(this.Cidade) ? "" : this.Cidade.Trim();
                fornecedorDto.Logradouro = string.IsNullOrWhiteSpace(this.Logradouro) ? "" : this.Logradouro.Trim();
                fornecedorDto.DataAlteracao = this.DataAlteracao;
                fornecedorDto.DataInclusao = this.DataInclusao;
                fornecedorDto.Id = this.Id;
                fornecedorDto.Inativo = this.Inativo;

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