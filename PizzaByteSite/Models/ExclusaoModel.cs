using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Model com as informações da entidade a ser excluída
    /// </summary>
    public class ExclusaoModel
    {
        /// <summary>
        /// Id que identifica a entidade a ser excluída
        /// </summary>
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Descrição de entidade a ser excluída
        /// </summary>
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
    }
}