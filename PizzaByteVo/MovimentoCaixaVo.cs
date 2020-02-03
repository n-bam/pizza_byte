namespace PizzaByteVo
{
    public class MovimentoCaixaVo : EntidadeBaseVo
    {
        /// <summary>
        /// Breve descritivo da movimentação
        /// MIN.: 3 / MAX.: 100
        /// </summary>
        public string Justificativa { get; set; }

        /// <summary>
        /// Valor da movimentação
        /// </summary>
        public float Valor { get; set; }
    }
}
