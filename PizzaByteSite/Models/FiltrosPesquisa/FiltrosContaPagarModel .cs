using System.Web.Mvc;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa as opções de pesquisa de uma conta
    /// </summary>
    public class FiltrosContaPagarModel : FiltrosContaModel
    {
        public FiltrosContaPagarModel() : base()
        {
            ListaOpcaoData.Add(new SelectListItem() { Text = "Vencimento", Value = "VENCIMENTO", Selected = true });
            ListaOpcaoData.Add(new SelectListItem() { Text = "Competência", Value = "COMPETENCIA" });
            ListaOpcaoData.Add(new SelectListItem() { Text = "Pagamento", Value = "PAGAMENTO" });
        }
    }
}