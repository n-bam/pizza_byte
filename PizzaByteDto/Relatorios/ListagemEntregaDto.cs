using PizzaByteDto.Base;
using System;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Dados básicos para listagem de entregas
    /// </summary>
    public class ListagemEntregaDto : BaseEntidadeDto
    {
        /// <summary>
        /// Endereço da entrega
        /// </summary>
        public string Logradouro { get; set; }

        /// <summary>
        /// Nome do funcionário que fez a entrega
        /// </summary>
        public string NomeFuncionario { get; set; }

        /// <summary>
        /// Observações da entrega
        /// </summary>
        public string Obs { get; set; }

        /// <summary>
        /// Valor total retornado
        /// </summary>
        public double ValorRetorno { get; set; }

        /// <summary>
        /// Valor a ser retornado
        /// </summary>
        public double RecebidoDinheiro { get; set; }

        /// <summary>
        /// Indica se está conferido
        /// </summary>
        public bool Conferido { get; set; }
    }
}
