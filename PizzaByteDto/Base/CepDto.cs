using System.Text;

namespace PizzaByteDto.Base
{
    public class CepDto : BaseEntidadeDto
    {
        /// <summary>
        /// Rua ou avenida correspondente ao CEP
        /// MIN: 3 / MAX: 150
        /// </summary>
        public string Logradouro { get; set; }

        /// <summary>
        /// Cidade correspondente ao CEP
        /// MIN: 2 / MAX: 50
        /// </summary>
        public string Cidade { get; set; }

        /// <summary>
        /// Bairro correspondente ao CEP
        /// MIN: 2 / MAX:50
        /// </summary>
        public string Bairro { get; set; }

        /// <summary>
        /// Chave que identifica unicamente o CEP
        /// MIN/MAX: 8
        /// </summary>
        public string Cep { get; set; }

        /// <summary>
        /// Para o retorno da pesquisa (viacep), equivalente a cidade
        /// </summary>
        public string Localidade { get; set; }

        #region Métodos

        /// <summary>
        /// Valida se os dados da entidade estão consistentes
        /// </summary>
        /// <returns></returns>
        public override bool ValidarEntidade(ref string mensagemErro)
        {
            bool retorno = true;
            StringBuilder sb = new StringBuilder();

            sb.Append(mensagemErro);
            mensagemErro = "";

            // Validar o CEP
            if (string.IsNullOrWhiteSpace(Cep))
            {
                sb.Append("O CEP do endereço é obrigatório, por favor informe um CEP para continuar. ");
                retorno = false;
            }
            else if (Cep.Length != 8)
            {
                sb.Append("O CEP do fornecedor devem ter 8 caracteres! " +
                $"O CEP inserido tem {Cep.Length} caracteres, por favor informe um CEP válido. ");
                retorno = false;
            }

            // Validar o logradouro
            if (string.IsNullOrWhiteSpace(Logradouro))
            {
                sb.Append("O logradouro (endereço) do endereço é obrigatório, por favor informe um logradouro para continuar. ");
                retorno = false;
            }
            else if (Logradouro.Length > 150)
            {
                sb.Append("O logradouro (endereço) deve ter, no máximo, 150 caracteres! " +
                   $"O logradouro inserido têm {Logradouro.Length} caracteres, por favor remova ao " +
                   $"menos {Logradouro.Length - 150} caracteres para continuar. ");
                retorno = false;
            }
            else if (Logradouro.Length < 3)
            {
                sb.Append("O logradouro (endereço) deve ter, ao menos, 3 caracteres! Por favor, informe um " +
                   "logradouro válido para continuar. ");
                retorno = false;
            }

            // Validar o bairro
            if (string.IsNullOrWhiteSpace(Bairro))
            {
                sb.Append("O bairro (endereço) do endereço é obrigatório, por favor informe um bairro para continuar. ");
                retorno = false;
            }
            else if (Bairro.Length > 50)
            {
                sb.Append("O bairro (endereço) deve ter, no máximo, 50 caracteres! " +
                   $"O bairro inserido têm {Bairro.Length} caracteres, por favor remova ao " +
                   $"menos {Bairro.Length - 50} caracteres para continuar. ");
                retorno = false;
            }
            else if (Bairro.Length < 2)
            {
                sb.Append("O bairro (endereço) deve ter, ao menos, 2 caracteres! Por favor, informe um " +
                   "bairro válido para continuar. ");
                retorno = false;
            }

            // Validar o cidade
            if (string.IsNullOrWhiteSpace(Cidade))
            {
                sb.Append("O cidade (endereço) do endereço é obrigatório, por favor informe uma cidade para continuar. ");
                retorno = false;
            }
            else if (Cidade.Length > 50)
            {
                sb.Append("O cidade (endereço) deve ter, no máximo, 50 caracteres! " +
                   $"O cidade inserido têm {Cidade.Length} caracteres, por favor remova ao " +
                   $"menos {Cidade.Length - 50} caracteres para continuar. ");
                retorno = false;
            }
            else if (Cidade.Length < 2)
            {
                sb.Append("O cidade (endereço) deve ter, ao menos, 2 caracteres! Por favor, informe um " +
                   "cidade válido para continuar. ");
                retorno = false;
            }

            mensagemErro = sb.ToString();
            return retorno;
        }

        #endregion
    }
}
