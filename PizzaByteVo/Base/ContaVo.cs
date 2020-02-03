using System;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteVo.Base
{
    /// <summary>
    /// Classe que representa os campos de logs no banco de dados
    /// </summary>
    public class ContaVo : EntidadeBaseVo
    {
        /// <summary>
        /// Data de vencimento da conta
        /// </summary>
        public DateTime DataVencimento { get; set; }

        /// <summary>
        /// Breve texto que descreve a conta
        /// MAX.: 200 / MIN.: 3
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Valor total a pagar
        /// </summary>
        public float Valor { get; set; }

        /// <summary>
        /// Data em que a conta foi gerada
        /// </summary>
        public DateTime DataCompetencia { get; set; }

        /// <summary>
        /// Estado em que a conta se encontra (quitada, estornada, em aberto, etc.)
        /// </summary>
        public StatusConta Status { get; set; }


    }
}
