using System;

namespace PizzaByteDto.RetornosRequisicoes
{
    public class RetornoGuidDto : RetornoDto
    {
        /// <summary>
        /// Guid a ser retornado
        /// </summary>
        public Guid Id { get; set; }
    }
}
