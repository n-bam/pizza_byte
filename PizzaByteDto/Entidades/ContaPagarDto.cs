using PizzaByteDto.Base;
using System;
using System.Text;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.Entidades
{
    public class ContaPagarDto : ContaDto
    {

        /// <summary>
        /// Data em que a conta foi paga
        /// </summary>
        public DateTime? DataPagamento { get; set; }

        /// <summary>
        /// Identificação do fornecedor no qual a conta pertence
        /// </summary>
        public Guid? IdFornecedor { get; set; }

        /// <summary>
        /// Nome do fornecedor
        /// </summary>
        public string NomeFornecedor { get; set; }

        #region Métodos

        /// <summary>
        /// Valida se os dados estão corretos
        /// </summary>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public override bool ValidarEntidade(ref string mensagemErro)
        {
            bool retorno = base.ValidarEntidade(ref mensagemErro);
            StringBuilder sb = new StringBuilder();

            sb.Append(mensagemErro);
            mensagemErro = "";
            
            if (DataVencimento < DataCompetencia)
            {
                sb.Append("A data de vencimento não pode ser menor que a data da conta! ");
                retorno = false;
            }

            mensagemErro = sb.ToString();
            return retorno;
        }

        #endregion
    }
}
