using PizzaByteDto.ClassesBase;
using System.Collections.Generic;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma lista de entidades
    /// </summary>
    public class RequisicaoObterListagemFornecedoresDto : RequisicaoObterRelatorioListagemDto
    {
        /// <summary>
        /// Procurar com fragmentos do nome
        /// </summary>
        public string NomeFantasia { get; set; }

        /// <summary>
        /// Nome de registro da empresa
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string RazaoSocial { get; set; }

        /// <summary>
        /// Procurar pelo telefone
        /// </summary>
        public string Telefone { get; set; }

        /// <summary>
        /// Procurar pelo CPF
        /// </summary>
        public string Cnpj { get; set; }
    }
}
