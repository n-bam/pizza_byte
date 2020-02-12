using PizzaByteDto.ClassesBase;
using System.Collections.Generic;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma lista de entidades
    /// </summary>
    public class RequisicaoObterMelhoresMotoboysDto : RequisicaoObterRelatorioListagemDto
    {
        /// <summary>
        /// Procurar por o nome do motoboy
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Procurar por um telefone
        /// </summary>
        public string Telefone { get; set; }

    }
}
