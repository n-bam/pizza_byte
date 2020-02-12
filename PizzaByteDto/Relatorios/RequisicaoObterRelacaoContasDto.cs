using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Requisição utilizada para obter o relatório de contas
    /// </summary>
    public class RequisicaoObterRelacaoContasDto : RequisicaoObterRelatorioListagemDto
    {
        /// <summary>
        /// Indica qual a data de pesquisa
        /// </summary>
        public string PesquisarPor { get; set; }

        /// <summary>
        /// Indica se é para trazer as contas estornadas
        /// </summary>
        public bool IndicadorEstornadas { get; set; }

        /// <summary>
        /// Indica se é para trazer as contas perdidas
        /// </summary>
        public bool IndicadorPerdida { get; set; }

        /// <summary>
        /// Filtrar por tipo de conta (Paga, Aberta, Perdida, etc)
        /// </summary>
        public StatusConta Status { get; set; }
    }
}
