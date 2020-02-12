using PizzaByteDto.Base;

namespace PizzaByteDto.RetornosRequisicoes
{
    /// <summary>
    /// Dados básicos para definir os melhores cadastros
    /// </summary>
    public class MelhoresCadastrosDto : BaseEntidadeDto
    {
        /// <summary>
        /// Descrição/nome da entidade
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Descrição/detalhamento da entidade
        /// </summary>
        public string Complemento { get; set; }

        /// <summary>
        /// Quantidade vendida/comprada
        /// </summary>
        public double Quantidade { get; set; }

        /// <summary>
        /// Valor total vendido/comprado
        /// </summary>
        public double Valor { get; set; }
    }
}
