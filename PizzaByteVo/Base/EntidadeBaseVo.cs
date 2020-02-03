﻿using System;

namespace PizzaByteVo
{
    /// <summary>
    /// Classe base que contém os dados compartilhados entre todas as entidades
    /// </summary>
    public abstract class EntidadeBaseVo
    {
        /// <summary>
        /// Id que identifica unicamente a entidade
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Data que a entidade foi incluída no banco de dados
        /// </summary>
        public DateTime DataInclusao { get; set; }

        /// <summary>
        /// Data da última alteração da entidade
        /// </summary>
        public DateTime? DataAlteracao { get; set; }

        /// <summary>
        /// Indica se a entidade está ativa para ser usada no sistema
        /// </summary>
        public bool Inativo { get; set; }

        /// <summary>
        /// Indica se a entidade está excluída 
        /// </summary>
        public bool Excluido { get; set; }
    }
}
