using PizzaByteDto.RetornosRequisicoes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    public class FiltrosMelhoresClientesModel : BaseFiltrosRelatorioModel
    {
        public FiltrosMelhoresClientesModel()
        {
            ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoMelhoresClientes();
        }

        /// <summary>
        /// Procurar por um nome de cliente
        /// </summary>
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Procurar por um telefone de cliente
        /// </summary>
        [Display(Name = "Telefone")]
        public string Telefone { get; set; }

        /// <summary>
        /// Converte de model para DTO
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref RequisicaoObterMelhoresClientesDto requisicaoDto, ref string mensagemErro)
        {
            if (!base.ConverterModelParaDto(requisicaoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                requisicaoDto.Nome = string.IsNullOrWhiteSpace(Nome) ? "" : Nome.Trim();
                requisicaoDto.Telefone = string.IsNullOrWhiteSpace(Telefone) ? "" : Telefone.Trim();


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