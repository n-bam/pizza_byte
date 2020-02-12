using PizzaByteDto.RetornosRequisicoes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    public class FiltrosListagemContaPagarModel : FiltrosListagemContasModel
    {
        public FiltrosListagemContaPagarModel()
        {
            ListaOpcaoPesquisa.Add(new SelectListItem() { Text = "Pagamento", Value = "DATAPAGAMENTO" });
          
            ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoContaPagar();
            ListaCampoOrdem.Add(new SelectListItem() {
                Text = "Pagamento",
                Value = "DATAPAGAMENTO"
            });
        }
        
        /// <summary>
        /// Procurar com fragmentos do nome
        /// </summary>
        [Display(Name = "Fornecedor")]
        public Guid IdFornecedor { get; set; }

        /// <summary>
        /// Nome do fornecedor
        /// </summary>
        [Display(Name = "Fornecedor")]
        public string NomeFornecedor { get; set; }

        /// <summary>
        /// Converte de model para dto
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool ConverterModelParaDto(ref RequisicaoObterListagemContaPagarDto requisicaoDto, ref string mensagemErro)
        {
            if (!base.ConverterModelParaDto(requisicaoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                requisicaoDto.IdFornecedor = IdFornecedor;

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