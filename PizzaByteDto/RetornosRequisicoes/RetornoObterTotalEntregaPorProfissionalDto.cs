using System.Collections.Generic;

namespace PizzaByteDto.RetornosRequisicoes
{
    public class RetornoObterTotalEntregaPorProfissionalDto : RetornoDto
    {
        public RetornoObterTotalEntregaPorProfissionalDto()
        {
            ListaTotais = new List<TotalPorProfissional>();
        }

        /// <summary>
        /// Lista com os totais por profissional
        /// </summary>
        public List<TotalPorProfissional> ListaTotais { get; set; }
    }

    /// <summary>
    /// Classe que representa um total de entregas
    /// </summary>
    public class TotalPorProfissional
    {
        /// <summary>
        /// Nome do profissional
        /// </summary>
        public string NomeProfissional { get; set; }

        /// <summary>
        /// Total de entregas feita pelo profissional
        /// </summary>
        public double TotalEntregas { get; set; }
    }
}
