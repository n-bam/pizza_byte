using PizzaByteDto.ClassesBase;
using System.Collections.Generic;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma lista de entidades
    /// </summary>
    public class RequisicaoObterListagemClienteDto : RequisicaoObterRelatorioListagemDto
    {
        /// <summary>
        /// Procurar com fragmentos do nome
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Procurar pelo telefone
        /// </summary>
        public string Telefone { get; set; }

        /// <summary>
        /// Procurar pelo CPF
        /// </summary>
        public string Cpf { get; set; }
    }
}
