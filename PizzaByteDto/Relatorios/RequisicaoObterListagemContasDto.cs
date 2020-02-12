using System;
using System.Collections.Generic;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma lista de entidades
    /// </summary>
    public class RequisicaoObterListagemContasDto : RequisicaoObterRelatorioListagemDto
    {
        /// <summary>
        /// Pesquisar pela data da conta
        /// </summary>
        public string PesquisarPor { get; set; }
        
        /// <summary>
        /// Pesquisar uma conta por descrição
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Faixa inicial de valor da conta
        /// </summary>
        public float PrecoInicial { get; set; }

        /// <summary>
        /// Faixa final de valor da conta
        /// </summary>
        public float PrecoFinal { get; set; }
        
        /// <summary>
        /// Status da conta
        /// </summary>
        public StatusConta Status { get; set; }
    }
}
