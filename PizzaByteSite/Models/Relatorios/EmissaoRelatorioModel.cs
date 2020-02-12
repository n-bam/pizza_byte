using Microsoft.Reporting.WebForms;

namespace PizzaByteSite.Models
{
    public class EmissaoRelatorioModel
    {
        /// <summary>
        /// Relatório a ser emitido
        /// </summary>
        public ReportViewer Relatorio { get; set; }

        /// <summary>
        /// De qual controller veio o relatório
        /// </summary>
        public string ControllerOrigem { get; set; }

        /// <summary>
        /// A view de origem do relatório
        /// </summary>
        public string ViewOrigem { get; set; }

        /// <summary>
        /// Nome do relatório
        /// </summary>
        public string NomeRelatorio { get; set; }
    }
}