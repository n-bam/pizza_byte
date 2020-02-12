using PizzaByteDto.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de uma conta a receber
    /// </summary>
    public class ContaModel : BaseModel
    {
        public ContaModel()
        {
            ListaStatus = Utilidades.RetornarListaStatusConta();
        }

        /// <summary>
        /// Data de vencimento da conta
        /// </summary>
        [Required(ErrorMessage = "A data de vencimento da conta é obrigatória.")]
        [Display(Name = "Vencimento")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataVencimento { get; set; }

        /// <summary>
        /// Descrição da conta
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        [Required(ErrorMessage = "A descrição da conta é obrigatória.")]
        [MaxLength(150, ErrorMessage = "A descrição da conta deve ter até 150 caracteres")]
        [MinLength(3, ErrorMessage = "A descrição da conta deve ter pelo menos 3 caracteres")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        /// <summary>
        /// Data da conta
        /// </summary>
        [Display(Name = "Competência")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "A data de competência da conta é obrigatória.")]
        public DateTime DataCompetencia { get; set; }
        
        /// <summary>
        /// Valor da conta
        /// </summary>
        [Required(ErrorMessage = "Por favor, preencha o valor da conta")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(0.01, 999999.99, ErrorMessage = "Informe o valor")]
        [Display(Name = "Valor")]
        public float Valor { get; set; }

        /// <summary>
        /// Indica o tipo do contaReceber (Paga, Quitada, Estornada, etc..)
        /// </summary>
        [Required(ErrorMessage = "Por favor, selecione qual o status da conta")]
        [Display(Name = "Status")]
        public StatusConta Status { get; set; }

        /// <summary>
        /// Lista com as opções de tipos de conta
        /// </summary>
        public List<SelectListItem> ListaStatus { get; set; }

        /// <summary>
        /// Converte uma conta de DTO para Model
        /// </summary>
        /// <param name="contaDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(ContaDto contaDto, ref string mensagemErro)
        {
            try
            {
                Descricao = string.IsNullOrWhiteSpace(contaDto.Descricao) ? "" : contaDto.Descricao.Trim();
                Valor = contaDto.Valor;
                Status = contaDto.Status;
                DataAlteracao = contaDto.DataAlteracao;
                DataInclusao = contaDto.DataInclusao;
                DataCompetencia = contaDto.DataCompetencia;
                DataVencimento = contaDto.DataVencimento;
                Id = contaDto.Id;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte uma conta de Model para Dto
        /// </summary>
        /// <param name="contaDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ContaDto contaDto, ref string mensagemErro)
        {
            try
            {
                contaDto.Descricao = string.IsNullOrWhiteSpace(Descricao) ? "" : Descricao.Trim();
                contaDto.Valor = Valor;
                contaDto.Status = Status;
                contaDto.DataAlteracao = this.DataAlteracao;
                contaDto.DataInclusao = this.DataInclusao;
                contaDto.DataCompetencia = this.DataCompetencia;
                contaDto.DataVencimento = this.DataVencimento;
                contaDto.Id = this.Id;

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