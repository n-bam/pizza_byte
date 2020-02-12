using PizzaByteDto.ClassesBase;
using System;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para recursos que sejam controlados por data
    /// </summary>
    public class RequisicaoDataDto : BaseRequisicaoDto
    {
        /// <summary>
        /// Data a ser utilizada
        /// </summary>
        public DateTime Data { get; set; }
    }
}
