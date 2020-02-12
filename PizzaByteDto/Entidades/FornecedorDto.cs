using PizzaByteDto.Base;
using System.Text;

namespace PizzaByteDto.Entidades
{
    public class FornecedorDto : BaseEntidadeDto
    {
        public FornecedorDto()
        {
            Endereco = new CepDto();
        }

        /// <summary>
        /// Nome popular do fornecedor
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string NomeFantasia { get; set; }

        /// <summary>
        /// Nome de registro da empresa
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string RazaoSocial { get; set; }

        /// <summary>
        /// Telefone para contato
        /// MIN.: 8 / MAX.: 20
        /// </summary>
        public string Telefone { get; set; }

        /// <summary>
        /// CNPJ do fornecedor
        /// MIN/MAX: 14
        /// </summary>
        public string Cnpj { get; set; }

        /// <summary>
        /// Numero do estabelecimento do fornecedor
        /// MIN: 1 / MAX: 10
        /// </summary>
        public string NumeroEndereco { get; set; }

        /// <summary>
        /// Pontos de referência do endereço do fornecedor
        /// MIN: - / MAX:50
        /// </summary>
        public string ComplementoEndereco { get; set; }

        /// <summary>
        /// Observações gerais
        /// MIN: - / MAX: 2000
        /// </summary>
        public string Obs { get; set; }
        
        /// <summary>
        /// Dados do endereço do fornecedor
        /// </summary>
        public CepDto Endereco { get; set; }

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

            // Validar o nome
            if (string.IsNullOrWhiteSpace(NomeFantasia))
            {
                sb.Append("O nome fantasia do fornecedor é obrigatório! Por favor, informe o nome fantasia do fornecedor " +
                    "no campo indicado para continuar. ");
                retorno = false;
            }
            else if (NomeFantasia.Length > 150)
            {
                sb.Append("O nome fantasia do fornecedor pode ter, no máximo, 150 caracteres! " +
                    $"O nome inserido tem {NomeFantasia.Length} caracteres, por favor remova ao menos {NomeFantasia.Length - 150}" +
                    $" caracteres para continuar. ");
                retorno = false;
            }
            else if (NomeFantasia.Length < 3)
            {
                sb.Append("O nome do fornecedor deve ter, ao menos, 3 caracteres! Por favor, informe um nome " +
                    "válido para continuar. ");
                retorno = false;
            }
            
            // Validar a razão social
            if (!string.IsNullOrWhiteSpace(RazaoSocial))
            {
                if (RazaoSocial.Length > 150)
                {
                    sb.Append("A razão social do fornecedor deve ter, no máximo, 150 caracteres! " +
                    $"A razão social inserida tem {RazaoSocial.Length} caracteres, por favor remova ao " +
                    $"menos {RazaoSocial.Length - 150} caracteres para continuar. ");
                    retorno = false;
                }
                else if (RazaoSocial.Length < 3)
                {
                    sb.Append("A razão social do fornecedor deve ter, ao menos, 3 caracteres! Por favor, informe uma " +
                    "razão social válida para continuar. ");
                    retorno = false;
                }
            }

            // Validar o telefone
            if (!string.IsNullOrWhiteSpace(Telefone))
            {
                if (Telefone.Length > 20)
                {
                    sb.Append("O telefone do fornecedor deve ter, no máximo, 20 caracteres! " +
                    $"O telefone inserido tem {Telefone.Length} caracteres, por favor remova ao " +
                    $"menos {Telefone.Length - 150} caracteres para continuar. ");
                    retorno = false;
                }
                else if (Telefone.Length < 8)
                {
                    sb.Append("O telefone do fornecedor deve ter, ao menos, 8 caracteres! Por favor, informe um " +
                    "telefone válido para continuar. ");
                    retorno = false;
                }
            }

            // Valida o CNPJ
            if (!ValidarCnpj(Cnpj))
            {
                sb.Append("O CNPJ do fornecedor não é válido. " +
                    "Informe um CNPJ válido para continuar. ");
                retorno = false;
            }

            // Validar o número
            if (!string.IsNullOrWhiteSpace(NumeroEndereco))
            {
                if (NumeroEndereco.Length > 10)
                {
                    sb.Append("O número (endereço) do fornecedor deve ter, no máximo, 10 caracteres! " +
                    $"O número inserido tem {NumeroEndereco.Length} caracteres, por favor remova ao " +
                    $"menos {NumeroEndereco.Length - 10} caracteres para continuar. ");
                    retorno = false;
                }
            }

            // Validar o complemento
            if (!string.IsNullOrWhiteSpace(ComplementoEndereco))
            {
                if (ComplementoEndereco.Length > 50)
                {
                    sb.Append("O complemento (endereço) do fornecedor deve ter, no máximo, 50 caracteres! " +
                    $"O complemento inserido tem {ComplementoEndereco.Length} caracteres, por favor remova ao " +
                    $"menos {ComplementoEndereco.Length - 50} caracteres para continuar. ");
                    retorno = false;
                }
            }

            // Validar as observações
            if (!string.IsNullOrWhiteSpace(Obs))
            {
                if (Obs.Length > 2000)
                {
                    sb.Append("As observações do fornecedor devem ter, no máximo, 2000 caracteres! " +
                    $"As observações inseridas têm {Obs.Length} caracteres, por favor remova ao " +
                    $"menos {Obs.Length - 2000} caracteres para continuar. ");
                    retorno = false;
                }
            }

            // Validar o Endereço
            if (!string.IsNullOrWhiteSpace(Endereco.Cep))
            {
                if (!Endereco.ValidarEntidade(ref mensagemErro))
                {
                    sb.Append(mensagemErro);
                    retorno = false;
                }
            }

            mensagemErro = sb.ToString();
            return retorno;
        }

        #endregion
    }
}
