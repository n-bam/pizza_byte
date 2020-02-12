using PizzaByteDto.Base;
using System;
using System.Text;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteDto.Entidades
{
    public class ContaDto : BaseEntidadeDto
    {
        /// <summary>
        /// Descrição da conta 
        /// MIN.: 3 / MAX.: 200
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Valor da conta
        ///  MAX.: 10
        /// </summary>
        public float Valor { get; set; }

        /// <summary>
        /// Data da conta 
        /// MIN.: 3 / MAX.: 200
        /// </summary>
        public DateTime DataCompetencia { get; set; }

        /// <summary>
        /// Data da conta 
        /// MIN.: 3 / MAX.: 200
        /// </summary>
        public DateTime DataVencimento { get; set; }

        /// <summary>
        /// Status que mostra se a conta esta paga ou em aberto
        /// MIN.: 1
        /// </summary>
        public StatusConta Status { get; set; }

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

            // Validar descrição
            if (string.IsNullOrWhiteSpace(Descricao))
            {
                sb.Append("A descrição da conta é obrigatório! Por favor, informe uma descrição " +
                    "no campo indicado para continuar. ");
                retorno = false;
            }
            else if (Descricao.Length > 200)
            {
                sb.Append("A descrição da conta pode ter no máximo, 200 caracteres! " +
                    $"a descrição inserida tem {Descricao.Length} caracteres, por favor remova ao menos {Descricao.Length - 200}" +
                    $" caracteres para continuar. ");
                retorno = false;
            }
            else if (Descricao.Length < 3)
            {
                sb.Append("A descrição da conta deve ter ao menos, 3 caracteres! Por favor, informe uma descrição " +
                    "válido para continuar. ");
                retorno = false;
            }

            // Validar o valor
            if (Valor < 0)
            {
                sb.Append("O valor da conta não pode ser negativo! Por favor, informe um " +
                "preço válido para continuar. ");
                retorno = false;
            }


            // Validar Data competencia
            if (DataCompetencia > DataVencimento)
            {
                sb.Append("A data da competência não pode ser maior que a data de vencimento! Por favor, informe uma " +
                "competência valida para continuar. ");
                retorno = false;
            }

            //// Validar Data de vencimento
            //if (DataVencimento <  DateTime.Now)
            //{
            //    sb.Append("A data de vencimento não pode ser ser anterior a data atual! Por favor, informe uma " +
            //    "data de vencimento valida para continuar. ");
            //    retorno = false;
            //}

            // Validar o status
            if (Status == StatusConta.NaoIdentificado)
            {
                sb.Append("O status da conta não foi informado! Por favor, informe um " +
                   "status válido para continuar. ");
                retorno = false;
            }

            mensagemErro = sb.ToString();
            return retorno;
        }

        #endregion

    }
}
