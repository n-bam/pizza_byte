using PizzaByteDto.ClassesBase;
using System;
using System.Collections.Generic;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma lista de entidades
    /// </summary>
    public class RequisicaoObterListagemEntregasDto : RequisicaoObterRelatorioListagemDto
    {
        /// <summary>
        /// Indica se o retorno da entrega já teve os valores conferidos
        /// </summary>
        public bool Conferido { get; set; }

        /// <summary>
        /// Valor em dinheiro retornado na entrega
        /// </summary>
        public float ValorRetorno { get; set; }

        /// <summary>
        /// Funcionario que fez a entrega
        /// </summary>
        public string Funcionario { get; set; }

        /// <summary>
        /// Identificação do endereço de entrega
        /// </summary>
        public string Endereco { get; set; }

        /// <summary>
        /// Valor em dinheiro retornado na entrega
        /// </summary>
        public float ValorInicio { get; set; }

        /// <summary>
        /// Valor em dinheiro retornado na entrega
        /// </summary>
        public float ValorFim { get; set; }

    }
}
