using PizzaByteDto.Base;
using System;
using System.Text;

namespace PizzaByteDto.Entidades
{
    public class ClienteEnderecoDto : BaseEntidadeDto
    {
        public ClienteEnderecoDto()
        {
            Endereco = new CepDto();
        }

        /// <summary>
        /// Numero do endereço do cliente
        /// MIN: 1 / MAX: 10
        /// </summary>
        public string NumeroEndereco { get; set; }

        /// <summary>
        /// Pontos de referência do endereço do cliente
        /// MIN: 0 / MAX: 50
        /// </summary>
        public string ComplementoEndereco { get; set; }

        /// <summary>
        /// Id do cliente que possui o endereço
        /// </summary>
        public Guid IdCliente { get; set; }

        /// <summary>
        /// Id do endereço relacionando a tabela CEP
        /// </summary>
        public Guid IdCep { get; set; }

        /// <summary>
        /// Identifica o CEP do endereço
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

            // Validar o nome do cliente
            if (string.IsNullOrWhiteSpace(NumeroEndereco))
            {
                sb.Append("O número do endereço é obrigatório! Por favor, informe o número " +
                    "no campo indicado para continuar. ");
                retorno = false;
            }
            else if (NumeroEndereco.Length > 10)
            {
                sb.Append("O número do endereço pode ter, no máximo, 10 caracteres! " +
                    $"O número inserido tem {NumeroEndereco.Length} caracteres, por favor remova ao menos {NumeroEndereco.Length - 10}" +
                    $" caracteres para continuar. ");
                retorno = false;
            }

            // Validar o telefone
            if (!string.IsNullOrWhiteSpace(ComplementoEndereco))
            {
                if (ComplementoEndereco.Length > 50)
                {
                    sb.Append("O complemento do endereço deve ter, no máximo, 20 caracteres! " +
                    $"O complemento inserido tem {ComplementoEndereco.Length} caracteres, por favor remova ao " +
                    $"menos {ComplementoEndereco.Length - 20} caracteres para continuar. ");
                    retorno = false;
                }
            }

            if (IdCliente == Guid.Empty)
            {
                sb.Append("O cliente não foi indicado! Por favor, selecione a qual cliente o endereço pertence.");
                retorno = false;
            }
            
            mensagemErro = sb.ToString();
            return retorno;
        }

        #endregion
    }
}
