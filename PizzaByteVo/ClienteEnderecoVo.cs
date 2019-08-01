using System;

namespace PizzaByteVo
{
    /// <summary>
    /// Classe que representa os campos do Endereço de um cliente no banco de dados
    /// </summary>

    public class ClienteEnderecoVo : EntidadeBaseVo
    {
        /// <summary>
        /// Numero do endereço do cliente
        /// MIN: 1 / MAX: 10
        /// </summary>
        public string NumeroEndereco { get; set; }

        /// <summary>
        /// Pontos de referência do endereço do cliente
        /// MIN: 0 / MAX: 50
        /// </summary>
        public string ComplementeEndereco { get; set; }

        /// <summary>
        /// Id do cliente que possui o endereço
        /// </summary>
        public Guid IdCliente { get; set; }

        /// <summary>
        /// Identificação do cliente no qual o endereço pertence
        /// </summary>
        public ClienteVo Cliente { get; set; }

        /// <summary>
        /// Id do endereço relacionando a tabela CEP
        /// </summary>
        public Guid IdCep { get; set; }

        /// <summary>
        /// Identifica o CEP do endereço
        /// </summary>
        public CepVo Endereco { get; set; }

    }
}