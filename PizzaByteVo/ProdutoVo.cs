using static PizzaByteEnum.Enumeradores;

namespace PizzaByteVo
{
    public class ProdutoVo : EntidadeBaseVo
    {
        /// <summary>
        /// Breve descritivo do produto
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Breve descritivo do produto
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public float Preco { get; set; }

        /// <summary>
        /// Indica o tipo do produto (bebida, pizza, etc.)
        /// Maior que 0
        /// </summary>
        public TipoProduto Tipo { get; set; }
    }
}
