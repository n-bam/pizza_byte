using System;
using System.ComponentModel.DataAnnotations;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa as opções de pesquisa de Logs
    /// </summary>
    public class FiltrosLogModel : BaseFiltrosModel
    {
        /// <summary>
        /// Pesquisar por mensagem
        /// </summary>
        [Display(Name = "Mensagem")]
        public string Mensagem { get; set; }

        /// <summary>
        /// Incluídos a partir da data
        /// </summary>
        [Display(Name = "De")]
        public DateTime DataInicial { get; set; }

        /// <summary>
        /// Incluídos até a data
        /// </summary>
        [Display(Name = "Até")]
        public DateTime DataFinal { get; set; }

        /// <summary>
        /// Executado pelo usuário
        /// </summary>
        [Display(Name = "Usuário")]
        public string IdUsuario { get; set; }

        /// <summary>
        /// Mensagens de um recurso específico
        /// </summary>
        [Display(Name = "Recurso")]
        public LogRecursos Recurso { get; set; }
    }
}