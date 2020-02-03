namespace PizzaByteVo
{
    /// <summary>
    /// Classe que representa os campos de um cliente no banco de dados
    /// </summary>
    public class ClienteVo : EntidadeBaseVo
    {
        /// <summary>
        /// Nome completo do cliente (obrigatório)
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Telefone de contato do cliente
        /// MIN.: 8 / MAX.: 20
        /// </summary>
        public string Telefone { get; set; }

        /// <summary>
        /// CPF do cliente
        /// MIN/MAX.: 11
        /// </summary>
        public string Cpf { get; set; }        
    }
}
