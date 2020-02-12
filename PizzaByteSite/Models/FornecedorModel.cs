using PizzaByteDto.Entidades;
using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um fornecedor
    /// </summary>
    public class FornecedorModel : BaseModel
    {
        public FornecedorModel()
        {
            Endereco = new CepModel();
        }

        /// <summary>
        /// Nome popular do fornecedor
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        [Required(ErrorMessage = "O nome fantasia do fornecedor é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O nome fantasia do fornecedor deve ter até 150 caracteres")]
        [MinLength(3, ErrorMessage = "O nome fantasia do fornecedor deve ter pelo menos 3 caracteres")]
        [Display(Name = "Nome fantasia")]
        public string NomeFantasia { get; set; }

        /// <summary>
        /// Nome de registro da empresa
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        [MaxLength(150, ErrorMessage = "A razão social do fornecedor deve ter até 150 caracteres")]
        [MinLength(3, ErrorMessage = "A razão social do fornecedor deve ter pelo menos 3 caracteres")]
        [Display(Name = "Razão social")]
        public string RazaoSocial { set; get; }

        /// <summary>
        /// Telefone para contato
        /// MIN.: 8 / MAX.: 20
        /// </summary>
        [MaxLength(20, ErrorMessage = "O telefone deve ter até 20 caracteres")]
        [MinLength(8, ErrorMessage = "O telefone deve ter pelo menos 8 caracteres")]
        [Display(Name = "Telefone")]
        public string Telefone { set; get; }

        /// <summary>
        /// CNPJ do fornecedor
        /// MIN/MAX: 14
        /// </summary>
        [MaxLength(18, ErrorMessage = "O CNPJ do fornecedor deve ter 18 caracteres")]
        [Display(Name = "CNPJ")]
        public string Cnpj { set; get; }

        /// <summary>
        /// Numero do estabelecimento do fornecedor
        /// MIN: 1 / MAX: 10
        /// </summary>
        [MaxLength(10, ErrorMessage = "O número deve ter até 10 caracteres")]
        [MinLength(1, ErrorMessage = "O número deve ter pelo menos 1 caracteres")]
        [Display(Name = "Nº")]
        public string NumeroEndereco { set; get; }

        /// <summary>
        /// Pontos de referência do endereço do fornecedor
        /// MIN: - / MAX:50
        /// </summary>
        [MaxLength(50, ErrorMessage = "O complemento deve ter até 50 caracteres")]
        [Display(Name = "Complemento")]
        public string ComplementoEndereco { get; set; }

        /// <summary>
        /// Observações gerais
        /// MIN: - / MAX: 2000
        /// </summary>
        [MaxLength(2000, ErrorMessage = "As observações devem ter até 2000 caracteres")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Observações")]
        public string Obs { get; set; }

        /// <summary>
        /// Dados do endereço do fornecedor
        /// </summary>
        public CepModel Endereco { get; set; }

        /// <summary>
        /// Converte um fornecedor de DTO para Model
        /// </summary>
        /// <param name="fornecedorDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(FornecedorDto fornecedorDto, ref string mensagemErro)
        {
            try
            {
                this.Cnpj = string.IsNullOrWhiteSpace(fornecedorDto.Cnpj) ? "" : fornecedorDto.Cnpj.Trim().Replace(".", "").Replace("-", "").Replace("-", "");
                this.ComplementoEndereco = string.IsNullOrWhiteSpace(fornecedorDto.ComplementoEndereco) ? "" : fornecedorDto.ComplementoEndereco.Trim();
                this.NomeFantasia = string.IsNullOrWhiteSpace(fornecedorDto.NomeFantasia) ? "" : fornecedorDto.NomeFantasia.Trim();
                this.NumeroEndereco = string.IsNullOrWhiteSpace(fornecedorDto.NumeroEndereco) ? "" : fornecedorDto.NumeroEndereco.Trim();
                this.Obs = string.IsNullOrWhiteSpace(fornecedorDto.Obs) ? "" : fornecedorDto.Obs.Trim();
                this.RazaoSocial = string.IsNullOrWhiteSpace(fornecedorDto.RazaoSocial) ? "" : fornecedorDto.RazaoSocial.Trim();
                this.Telefone = string.IsNullOrWhiteSpace(fornecedorDto.Telefone) ? "" : fornecedorDto.Telefone.Trim();
                this.DataAlteracao = fornecedorDto.DataAlteracao;
                this.DataInclusao = fornecedorDto.DataInclusao;
                this.Id = fornecedorDto.Id;
                this.Inativo = fornecedorDto.Inativo;

                Endereco.Bairro = string.IsNullOrWhiteSpace(fornecedorDto.Endereco.Bairro) ? "" : fornecedorDto.Endereco.Bairro.Trim();
                Endereco.Cep = string.IsNullOrWhiteSpace(fornecedorDto.Endereco.Cep) ? "" : fornecedorDto.Endereco.Cep.Trim();
                Endereco.Cidade = string.IsNullOrWhiteSpace(fornecedorDto.Endereco.Cidade) ? "" : fornecedorDto.Endereco.Cidade.Trim();
                Endereco.Logradouro = string.IsNullOrWhiteSpace(fornecedorDto.Endereco.Logradouro) ? "" : fornecedorDto.Endereco.Logradouro.Trim();
                Endereco.Id = Endereco.Id;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um fornecedor de Model para Dto
        /// </summary>
        /// <param name="fornecedorDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref FornecedorDto fornecedorDto, ref string mensagemErro)
        {
            try
            {
                fornecedorDto.Cnpj = string.IsNullOrWhiteSpace(Cnpj) ? "" : Cnpj.Trim().Replace(".", "").Replace("-", "").Replace("-", "");
                fornecedorDto.ComplementoEndereco = string.IsNullOrWhiteSpace(ComplementoEndereco) ? "" : ComplementoEndereco.Trim();
                fornecedorDto.NomeFantasia = string.IsNullOrWhiteSpace(NomeFantasia) ? "" : NomeFantasia.Trim();
                fornecedorDto.NumeroEndereco = string.IsNullOrWhiteSpace(NumeroEndereco) ? "" : NumeroEndereco.Trim();
                fornecedorDto.Obs = string.IsNullOrWhiteSpace(Obs) ? "" : Obs.Trim();
                fornecedorDto.RazaoSocial = string.IsNullOrWhiteSpace(RazaoSocial) ? "" : RazaoSocial.Trim();
                fornecedorDto.Telefone = string.IsNullOrWhiteSpace(Telefone) ? "" : Telefone.Trim();
                fornecedorDto.DataAlteracao = this.DataAlteracao;
                fornecedorDto.DataInclusao = this.DataInclusao;
                fornecedorDto.Id = this.Id;
                fornecedorDto.Inativo = this.Inativo;

                fornecedorDto.Endereco.Bairro = string.IsNullOrWhiteSpace(Endereco.Bairro) ? "" : Endereco.Bairro.Trim();
                fornecedorDto.Endereco.Cep = string.IsNullOrWhiteSpace(Endereco.Cep) ? "" : Endereco.Cep.Trim().Replace("-", "");
                fornecedorDto.Endereco.Cidade = string.IsNullOrWhiteSpace(Endereco.Cidade) ? "" : Endereco.Cidade.Trim();
                fornecedorDto.Endereco.Logradouro = string.IsNullOrWhiteSpace(Endereco.Logradouro) ? "" : Endereco.Logradouro.Trim();
                fornecedorDto.Endereco.Id = Endereco.Id;

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