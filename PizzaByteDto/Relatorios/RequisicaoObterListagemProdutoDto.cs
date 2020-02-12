using PizzaByteDto.ClassesBase;
using System.Collections.Generic;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma lista de entidades
    /// </summary>
    public class RequisicaoObterListagemProdutoDto : RequisicaoObterRelatorioListagemDto
    {
        /// <summary>
        /// Procurar com fragmentos da descrição
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Procurar pelo preço
        /// </summary>
        public float PrecoInicio { get; set; }

        /// <summary>
        /// Procurar pelo preço
        /// </summary>
        public float PrecoFim { get; set; }

        /// <summary>
        /// Procurar pelo tipo
        /// </summary>
        public TipoProduto Tipo { get; set; }
    }
}
