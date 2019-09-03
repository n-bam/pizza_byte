namespace PizzaByteVo
{
    /// <summary>
    /// Classe que representa os campos da taxa de entrega no banco de dados
    /// </summary>
    public class TaxaEntregaVo : EntidadeBaseVo
    {
        /// <summary>
        /// Bairro que a taxa de entrega abrange
        /// MIN.: 3 / MAX.: 50
        /// </summary>

        public string BairroCidade { get; set; }

        /// <summary>
        /// Valor de entrega para o bairro correspondente
        /// </summary>
        public float ValorTaxa { get; set; }
    }
}
