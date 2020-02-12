using System.Web.Mvc;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa as opções de pesquisa de uma conta
    /// </summary>
    public class FiltrosContaReceberModel : FiltrosContaModel
    {
        public FiltrosContaReceberModel() : base()
        {
            ListaOpcaoData.Add(new SelectListItem() { Text = "Vencimento", Value = "VENCIMENTO" });
            ListaOpcaoData.Add(new SelectListItem() { Text = "Competência", Value = "COMPETENCIA", Selected = true });
        }
    }
}