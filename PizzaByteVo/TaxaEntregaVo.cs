namespace PizzaByteVo
{
    /// <summary>
    /// Classe que representa os campos da taxa de entrega no banco de dados
    /// </summary>
    public class TaxaEntregaVo : EntidadeBaseVo
    {
        /// <summary>
        /// Bairro que a taxa de entrega abrange
        /// MIN: 3 
        /// </summary>
        public string Bairro;

        /// <summary>
        /// Valor de entrega para o bairro correspondente
        /// MIN: 0
        /// </summary>
        public float ValorTaxa;
    }
}
