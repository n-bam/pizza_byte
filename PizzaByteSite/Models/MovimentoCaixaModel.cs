using PizzaByteDto.Entidades;
using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um produto no site
    /// </summary>
    public class MovimentoCaixaModel : BaseModel
    {
        /// <summary>
        /// Breve descritivo da movimentação
        /// MIN.: 3 / MAX.: 100
        /// </summary>
        [Required(ErrorMessage = "Por favor, preencha a justificativa da movimentação. Ex.: Caixa inicial")]
        [Display(Name = "Justificativa")]
        [StringLength(100, ErrorMessage = "Informe a descrição para o produto de 3 a 100 letras.", MinimumLength = 3)]
        public string Justificativa { get; set; }

        /// <summary>
        /// Valor da movimentação
        /// </summary>
        [Required(ErrorMessage = "Por favor, preencha o preço de venda do produto")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Preço")]
        public float Valor { get; set; }

        /// <summary>
        /// Converte um produto de DTO para Model
        /// </summary>
        /// <param name="movimentoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(MovimentoCaixaDto movimentoDto, ref string mensagemErro)
        {
            try
            {
                Justificativa = string.IsNullOrWhiteSpace(movimentoDto.Justificativa) ? "" : movimentoDto.Justificativa.Trim();
                Valor = movimentoDto.Valor;
                DataInclusao = movimentoDto.DataInclusao;
                Id = movimentoDto.Id;

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
        /// <param name="movimentoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref MovimentoCaixaDto movimentoDto, ref string mensagemErro)
        {
            try
            {
                movimentoDto.Justificativa = string.IsNullOrWhiteSpace(Justificativa) ? "" : Justificativa.Trim();
                movimentoDto.Valor = Valor;
                movimentoDto.DataInclusao = this.DataInclusao;
                movimentoDto.Id = this.Id;

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