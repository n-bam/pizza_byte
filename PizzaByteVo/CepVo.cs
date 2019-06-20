namespace PizzaByteVo
{
    /// <summary>
    /// Classe base que contém os dados compartilhados entre todas as entidades
    /// </summary>
    public class CepVo : EntidadeBaseVo
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
    }
}
