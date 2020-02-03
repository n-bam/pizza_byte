using System;

namespace PizzaByteVo
{
    public class FornecedorVo : EntidadeBaseVo
    {
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
        /// Id do endereço relacionando a tabela CEP
        /// </summary>
        public Guid? IdCep { get; set; }

        /// <summary>
        /// Entidade que leva os dados do endereço do fornecedor
        /// </summary>
        public virtual CepVo Endereco { get; set; }
    }
}
