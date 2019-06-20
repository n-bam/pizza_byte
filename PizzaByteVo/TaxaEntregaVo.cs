namespace PizzaByteVo
{
    /// <summary>
    /// Classe que representa os campos da taxa de entrega no banco de dados
    /// </summary>
    public class TaxaEntregaVo : EntidadeBaseVo
    {
        /// <summary>
        /// Bairro que a taxa de entrega abrange
        /// </summary>
        public string Bairro;

        /// <summary>
        /// Valor de entrega para o bairro correspondente
        /// </summary>
        public float ValorTaxa;
    }
}
