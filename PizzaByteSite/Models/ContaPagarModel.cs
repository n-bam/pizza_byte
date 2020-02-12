using PizzaByteDto.Entidades;
using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de uma conta a pagar
    /// </summary>
    public class ContaPagarModel : ContaModel
    {

        /// <summary>
        /// Descrição da conta
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        [Display(Name = "Fornecedor")]
        public string NomeFantasia { get; set; }

        /// <summary>
        /// Data em que a conta foi paga
        /// </summary>
        [Display(Name = "Pagamento")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataPagamento { get; set; }

        /// <summary>
        /// Identificação do fornecedor no qual a conta pertence
        /// </summary>
        public Guid? IdFornecedor { get; set; }

        /// <summary>
        /// Dados do endereço do fornecedor
        /// </summary>
        public FornecedorModel Fornecedor { get; set; }

        /// <summary>
        /// Converte uma conta de DTO para Model
        /// </summary>
        /// <param name="contaPagarDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        /// <summary>
        /// Converte uma conta de Model para Dto
        /// </summary>
        /// <param name="contaPagarDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterDtoParaModel(ContaPagarDto contaPagarDto, ref string mensagemErro)
        {

            if (!base.ConverterDtoParaModel(contaPagarDto, ref mensagemErro))
            {
                return false;
            }

            try
            {

                DataPagamento = contaPagarDto.DataPagamento;
                IdFornecedor = (contaPagarDto.IdFornecedor == Guid.Empty) ? null : contaPagarDto.IdFornecedor;
                NomeFantasia = contaPagarDto.NomeFornecedor;

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
        /// <param name="contaPagarDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref ContaPagarDto contaPagarDto, ref string mensagemErro)
        {
            if (!base.ConverterModelParaDto(contaPagarDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                contaPagarDto.DataPagamento = this.DataPagamento;
                contaPagarDto.IdFornecedor = (this.IdFornecedor == Guid.Empty) ? null : this.IdFornecedor;
                contaPagarDto.NomeFornecedor = this.NomeFantasia;

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