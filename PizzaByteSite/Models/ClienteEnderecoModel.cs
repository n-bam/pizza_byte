using PizzaByteDto.Entidades;
using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um endereço
    /// </summary>
    public class ClienteEnderecoModel : BaseModel
    {
        public ClienteEnderecoModel()
        {
            Endereco = new CepModel();
        }

        /// <summary>
        /// Numero do endereço do cliente
        /// MIN: 1 / MAX: 10
        /// </summary>
        [Required]
        [Display(Name = "Nº")]
        [StringLength(10, ErrorMessage = "Informe um número para o endereço com até 10 letras.")]
        public string NumeroEndereco { get; set; }

        /// <summary>
        /// Pontos de referência do endereço do cliente
        /// MIN: 0 / MAX: 50
        /// </summary>
        [Display(Name = "Complemento")]
        [StringLength(50, ErrorMessage = "Informe um complemente com até 50 letras.")]
        public string ComplementoEndereco { get; set; }

        /// <summary>
        /// Id do cliente que possui o endereço
        /// </summary>
        [Required]
        [Display(Name = "Cliente")]
        public Guid IdCliente { get; set; }

        /// <summary>
        /// Id do endereço relacionando a tabela CEP
        /// </summary>
        [Required]
        [Display(Name = "CEP")]
        public Guid IdCep { get; set; }

        /// <summary>
        /// Nome do cliente que o endereço pertence
        /// </summary>
        [Display(Name = "Cliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Endereço completo do cep
        /// </summary>
        public CepModel Endereco { get; set; }

        /// <summary>
        /// Converte um endereço de cliente de DTO para Model
        /// </summary>
        /// <param name="enderecoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(ClienteEnderecoDto enderecoDto, ref string mensagemErro)
        {
            try
            {
                this.NumeroEndereco = string.IsNullOrWhiteSpace(enderecoDto.NumeroEndereco) ? "" : enderecoDto.NumeroEndereco.Trim();
                this.ComplementoEndereco = string.IsNullOrWhiteSpace(enderecoDto.ComplementeEndereco) ? "" : enderecoDto.ComplementeEndereco.Trim();
                this.DataAlteracao = enderecoDto.DataAlteracao;
                this.DataInclusao = enderecoDto.DataInclusao;
                this.Id = enderecoDto.Id;
                this.Inativo = enderecoDto.Inativo;
                this.IdCep = enderecoDto.IdCep;
                this.IdCliente = enderecoDto.IdCliente;

                Endereco.Bairro = string.IsNullOrWhiteSpace(enderecoDto.Endereco.Bairro) ? "" : enderecoDto.Endereco.Bairro.Trim();
                Endereco.Cep = string.IsNullOrWhiteSpace(enderecoDto.Endereco.Cep) ? "" : enderecoDto.Endereco.Cep.Trim();
                Endereco.Cidade = string.IsNullOrWhiteSpace(enderecoDto.Endereco.Cidade) ? "" : enderecoDto.Endereco.Cidade.Trim();
                Endereco.Logradouro = string.IsNullOrWhiteSpace(enderecoDto.Endereco.Logradouro) ? "" : enderecoDto.Endereco.Logradouro.Trim();
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
        /// Converte um endereço de cliente de Model para Dto
        /// </summary>
        /// <param name="enderecoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref ClienteEnderecoDto enderecoDto, ref string mensagemErro)
        {
            try
            {
                enderecoDto.NumeroEndereco = string.IsNullOrWhiteSpace(NumeroEndereco) ? "" : NumeroEndereco.Trim();
                enderecoDto.ComplementeEndereco = string.IsNullOrWhiteSpace(ComplementoEndereco) ? "" : ComplementoEndereco.Trim();
                enderecoDto.DataAlteracao = DataAlteracao;
                enderecoDto.DataInclusao = DataInclusao;
                enderecoDto.Id = Id;
                enderecoDto.Inativo = Inativo;
                enderecoDto.IdCep = Endereco.Id;
                enderecoDto.IdCliente = IdCliente;

                enderecoDto.Endereco.Bairro = string.IsNullOrWhiteSpace(Endereco.Bairro) ? "" : Endereco.Bairro.Trim();
                enderecoDto.Endereco.Cep = string.IsNullOrWhiteSpace(Endereco.Cep) ? "" : Endereco.Cep.Trim().Replace("-", "");
                enderecoDto.Endereco.Cidade = string.IsNullOrWhiteSpace(Endereco.Cidade) ? "" : Endereco.Cidade.Trim();
                enderecoDto.Endereco.Logradouro = string.IsNullOrWhiteSpace(Endereco.Logradouro) ? "" : Endereco.Logradouro.Trim();
                enderecoDto.Endereco.Id = Endereco.Id;

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