using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaByteSite.Models
{
    /// <summary>
    /// Classe que representa os dados de um endereço
    /// </summary>
    public class ClienteEnderecoModel : BaseModel
    {
        public ClienteEnderecoModel()
        {
            EnderecoCep = new CepModel();
        }

        /// <summary>
        /// Numero do endereço do cliente
        /// </summary>
        [Display(Name = "Nº")]
        [StringLength(10, ErrorMessage = "Informe um número para o endereço com até 10 letras.")]
        public string NumeroEndereco { get; set; }

        /// <summary>
        /// Pontos de referência do endereço do cliente
        /// </summary>
        [Display(Name = "Complemento")]
        [StringLength(50, ErrorMessage = "Informe uma cidade com até 50 letras.")]
        public string Complemento { get; set; }

        /// <summary>
        /// Bairro correspondente ao CEP
        /// </summary>
        [Display(Name = "Bairro")]
        public Guid IdCliente { get; set; }

        /// <summary>
        /// Chave que identifica unicamente o CEP
        /// </summary>
        [Display(Name = "CEP")]
        [StringLength(8, ErrorMessage = "Informe uma cidade com até 50 letras.")]
        public string Cep { get; set; }

        /// <summary>
        /// Endereço completo do cep
        /// </summary>
        public CepModel EnderecoCep { get; set; }
    }
}