using PizzaByteDto.ClassesBase;
using System;
using System.Collections.Generic;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter uma lista de entidades
    /// </summary>
    public class RequisicaoObterRelatorioListagemDto : BaseRequisicaoDto
    {
        /// <summary>
        /// Campo de ordem dos registros
        /// </summary>
        public string CampoOrdem { get; set; }

        /// <summary>
        /// Indica se obtem ativos, inativos ou ambos
        /// </summary>
        public string ObterInativos { get; set; }

        /// <summary>
        /// Cadastrados entre, data inicial
        /// </summary>
        public DateTime? DataCadastroInicial { get; set; }

        /// <summary>
        /// Cadastrados entre, data final
        /// </summary>
        public DateTime? DataCadastroFinal { get; set; }
    }
}
