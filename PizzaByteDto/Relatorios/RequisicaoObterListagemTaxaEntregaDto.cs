using PizzaByteDto.ClassesBase;
using System.Collections.Generic;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma lista de entidades
    /// </summary>
    public class RequisicaoObterListagemTaxaEntregaDto : RequisicaoObterRelatorioListagemDto
    {
        /// <summary>
        /// Procurar com fragmentos do nome
        /// </summary>
        public string BairroCidade { get; set; }

        /// <summary>
        /// Procurar pelo valor da taxa
        /// </summary>
        public float ValorTaxa { get; set; }

        /// <summary>
        /// Procurar pelo spreço de inicio
        /// </summary>
        public float ValorInicio { get; set; }

        /// <summary>
        /// Procurar pelo prço de fim
        /// </summary>
        public float ValorFim { get; set; }

    }
}
