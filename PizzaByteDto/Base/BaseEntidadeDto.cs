using System;

namespace PizzaByteDto.Base
{
    public abstract class BaseEntidadeDto
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
        /// Indica se a entidade foi excluída do sistema
        /// </summary>
        public bool Excluido { get; set; }

        #region Métodos

        /// <summary>
        /// Valida se os dados da entidade estão consistentes
        /// </summary>
        /// <returns></returns>
        public virtual bool ValidarEntidade(ref string mensagemErro)
        {
            bool retorno = true;

            if (Id == null || Id == Guid.Empty)
            {
                mensagemErro = "O ID da entidade é obrigatório!";
                retorno = false;
            }

            return retorno;
        }

        /// <summary>
        /// Valida se o CNPJ informado é válido
        /// </summary>
        /// <param name="cnpj"></param>
        /// <returns></returns>
        public bool ValidarCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
            {
                return true;
            }

            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;

            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            if (cnpj.Length != 14)
                return false;

            tempCnpj = cnpj.Substring(0, 12);

            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();

            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cnpj.EndsWith(digito);
        }

        /// <summary>
        /// Valida se o CPF informado é válido
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        public bool ValidarCpf(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf, digito;
            int soma, resto;

            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
            {
                return false;
            }

            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            }

            resto = soma % 11;
            if (resto < 2)
            {
                resto = 0;
            }
            else
            {
                resto = 11 - resto;
            }

            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            }

            resto = soma % 11;
            if (resto < 2)
            {
                resto = 0;
            }
            else
            {
                resto = 11 - resto;
            }

            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }

        #endregion
    }
}
