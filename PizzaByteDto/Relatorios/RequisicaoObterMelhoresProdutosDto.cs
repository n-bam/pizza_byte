using PizzaByteDto.ClassesBase;
using System.Collections.Generic;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma lista de entidades
    /// </summary>
    public class RequisicaoObterMelhoresProdutosDto : RequisicaoObterRelatorioListagemDto
    {
        /// <summary>
        /// Procurar por um tipo de produto
        /// </summary>
        public TipoProduto Tipo { get; set; }
    }
}
