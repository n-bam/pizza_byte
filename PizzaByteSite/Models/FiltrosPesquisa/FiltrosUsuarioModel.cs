using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa as opções de pesquisa de usuarios
    /// </summary>
    public class FiltrosUsuarioModel : BaseFiltrosModel
    {
        public FiltrosUsuarioModel() : base()
        {
            ListaAdministrador = new List<SelectListItem>();
            ListaAdministrador.Add(new SelectListItem() { Text = "Todos", Value = "" });
            ListaAdministrador.Add(new SelectListItem() { Text = "Administradores", Value = "true" });
            ListaAdministrador.Add(new SelectListItem() { Text = "Não adminstradores", Value = "false" });
        }

        /// <summary>
        /// Pesquisar um usuário por nome
        /// </summary>
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Pesquisar um usuário pelo email
        /// </summary>
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Obter os usuários administradores
        /// </summary>
        [Display(Name = "Obter usuários")]
        public string ObterAdministrador { get; set; }

        /// <summary>
        /// Lista com as opções de pesquisa de usuário administrador
        /// </summary>
        public List<SelectListItem> ListaAdministrador { get; set; }
    }
}