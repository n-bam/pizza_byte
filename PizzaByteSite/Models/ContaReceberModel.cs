using PizzaByteDto.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de uma conta a pagar
    /// </summary>
    public class ContaReceberModel : ContaModel
    {

        /// <summary>
        /// Identificação do pedido no qual a conta pertence
        /// </summary>
        public Guid? IdPedido { get; set; }

       /// <summary>
       /// Converte uma DTO para Model
       /// </summary>
       /// <param name="contaReceberDto"></param>
       /// <param name="mensagemErro"></param>
       /// <returns></returns>
        public bool ConverterDtoParaModel(ContaReceberDto contaReceberDto, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaModel(contaReceberDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                IdPedido = contaReceberDto.IdPedido;
        
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
        /// <param name="contaReceberDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref ContaReceberDto contaReceberDto, ref string mensagemErro)
        {
            if (!base.ConverterModelParaDto(contaReceberDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                contaReceberDto.IdPedido = this.IdPedido;
               
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