using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaByteVo
{
    /// <summary>
    /// Classe que representa os campos do Endereço de um cliente no banco de dados
    /// </summary>

    public class ClienteEnderecoVo : EntidadeBaseVo
    {
        /// <summary>
        /// Numero do endereço do cliente
        /// </summary>
        public string NumeroEndereco;

        /// <summary>
        /// Pontos de referência do endereço do cliente
        /// </summary>
        public string ComplementeEndereco;

        /// <summary>
        /// Identificação do cliente no qual o endereço pertence
        /// </summary>
        public string IdCliente;

        /// <summary>
        /// Identifica o CEP do endereço
        /// </summary>
        public string Cep;

    }
}