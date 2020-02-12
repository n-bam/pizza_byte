using PizzaByteDto.Base;

namespace PizzaByteDto.Entidades
{
    public class ProdutosVendidosPorDiaSemanaDto : BaseEntidadeDto
    {
        /// <summary>
        /// Dia da semana (Dom, Seg, Ter, Qua, Qui, Sex, Sáb)
        /// </summary>
        public string DiaSemana { get; set; }

        /// <summary>
        /// Número de pizzas vendidas
        /// </summary>
        public int Pizza { get; set; }

        /// <summary>
        /// Número de bebidas vendidas
        /// </summary>
        public int Bebida { get; set; }
    }
}
