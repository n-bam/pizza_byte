using PizzaByteDto.ClassesBase;
using System;
using static PizzaByteEnum.Enumeradores;
using System.Collections.Generic;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma lista de entidades
    /// </summary>
    public class RequisicaoObterListagemContaPagarDto : RequisicaoObterListagemContasDto
    {
        /// <summary>
        /// Identificação do fornecedor no qual a conta pertence
        /// </summary>
        public Guid IdFornecedor { get; set; }
    }
}
