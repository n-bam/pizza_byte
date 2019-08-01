using PizzaByteDto.Entidades;
using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um cliente no site
    /// </summary>
    public class ClienteModel : BaseModel
    {
        /// <summary>
        /// Nome completo do cliente (obrigatório)
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        [Required(ErrorMessage = "Por favor, preencha o nome do cliente")]
        [Display(Name = "Nome")]
        [StringLength(150, ErrorMessage = "Informe a descrição para o produto de 3 a 150 letras.", MinimumLength = 3)]
        public string Nome { get; set; }

        /// <summary>
        /// Telefone de contato do cliente
        /// MIN.: 8 / MAX.: 20
        /// </summary>
        [Display(Name = "Telefone")]
        [StringLength(20, ErrorMessage = "Informe um telefone válido.", MinimumLength = 8)]
        public string Telefone { get; set; }

        /// <summary>
        /// CPF do cliente
        /// MIN/MAX.: 11 (15 com pontuação)
        /// </summary>
        [Display(Name = "CPF")]
        [StringLength(15, ErrorMessage = "Informe um CPF válido.", MinimumLength = 11)]
        public string Cpf { get; set; }

        /// <summary>
        /// Converte um produto de DTO para Model
        /// </summary>
        /// <param name="clienteDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(ClienteDto clienteDto, ref string mensagemErro)
        {
            try
            {
                Nome = string.IsNullOrWhiteSpace(clienteDto.Nome) ? "" : clienteDto.Nome.Trim();
                Telefone = string.IsNullOrWhiteSpace(clienteDto.Telefone) ? "" : clienteDto.Telefone.Trim().Replace("-", "");
                Cpf = string.IsNullOrWhiteSpace(clienteDto.Cpf) ? "" : clienteDto.Cpf.Trim().Replace(".", "").Replace("-", "");
                DataAlteracao = clienteDto.DataAlteracao;
                DataInclusao = clienteDto.DataInclusao;
                Id = clienteDto.Id;
                Inativo = clienteDto.Inativo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um produto de Model para Dto
        /// </summary>
        /// <param name="clienteDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref ClienteDto clienteDto, ref string mensagemErro)
        {
            try
            {
                clienteDto.Nome = string.IsNullOrWhiteSpace(Nome) ? "" : Nome.Trim();
                clienteDto.Telefone = string.IsNullOrWhiteSpace(Telefone) ? "" : Telefone.Trim().Replace("-", "");
                clienteDto.Cpf = string.IsNullOrWhiteSpace(Cpf) ? "" : Cpf.Trim().Replace(".", "").Replace("-", "");
                clienteDto.DataAlteracao = this.DataAlteracao;
                clienteDto.DataInclusao = this.DataInclusao;
                clienteDto.Id = this.Id;
                clienteDto.Inativo = this.Inativo;

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