using PizzaByteDto.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um funcionario no site
    /// </summary>
    public class FuncionarioModel : BaseModel
    {
        public FuncionarioModel()
        {
            ListaTipos = Utilidades.RetornarListaTiposFuncionario();
        }

        /// <summary>
        /// Nome popular do funcionario
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        [Required(ErrorMessage = "O nome do funcionario é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O nome do funcionario deve ter até 150 caracteres")]
        [MinLength(3, ErrorMessage = "O nome do funcionario deve ter pelo menos 3 caracteres")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Telefone para contato
        /// MIN.: 8 / MAX.: 20
        /// </summary>
        [MaxLength(20, ErrorMessage = "O telefone deve ter até 20 caracteres")]
        [MinLength(8, ErrorMessage = "O telefone deve ter pelo menos 8 caracteres")]
        [Display(Name = "Telefone")]
        public string Telefone { set; get; }

        /// <summary>
        /// Indica o tipo do funcionario (Motoboy, Atendente, Cozinheiro, etc..)
        /// </summary>
        [Required(ErrorMessage = "Por favor, selecione qual o tipo de funcionario")]
        [Display(Name = "Tipo")]
        public TipoFuncionario Tipo { get; set; }

        /// <summary>
        /// Lista com as opções de tipos de funcionarios
        /// </summary>
        public List<SelectListItem> ListaTipos { get; set; }

        /// <summary>
        /// Converte um funcionario de DTO para Model
        /// </summary>
        /// <param name="funcionarioDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(FuncionarioDto funcionarioDto, ref string mensagemErro)
        {
            try
            {

                Nome = string.IsNullOrWhiteSpace(funcionarioDto.Nome) ? "" : funcionarioDto.Nome.Trim();
                Telefone = funcionarioDto.Telefone;
                Tipo = funcionarioDto.Tipo;
                DataAlteracao = funcionarioDto.DataAlteracao;
                DataInclusao = funcionarioDto.DataInclusao;
                Id = funcionarioDto.Id;
                Inativo = funcionarioDto.Inativo;


                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um funcionario de Model para Dto
        /// </summary>
        /// <param name="funcionarioDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref FuncionarioDto funcionarioDto, ref string mensagemErro)
        {
            try
            {
                funcionarioDto.Nome = string.IsNullOrWhiteSpace(Nome) ? "" : Nome.Trim();
                funcionarioDto.Telefone = Telefone;
                funcionarioDto.Tipo = Tipo;
                funcionarioDto.DataAlteracao = this.DataAlteracao;
                funcionarioDto.DataInclusao = this.DataInclusao;
                funcionarioDto.Id = this.Id;
                funcionarioDto.Inativo = this.Inativo;

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