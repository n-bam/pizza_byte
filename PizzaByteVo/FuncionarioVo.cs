using static PizzaByteEnum.Enumeradores;

namespace PizzaByteVo
{
    public class FuncionarioVo : EntidadeBaseVo
    {
        /// <summary>
        /// Nome do funcionário 
        /// MIN.: 3 / MAX.: 150
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Telefone de contato do funcionário
        /// MIN.: 8 / MAX.: 20
        /// </summary>
        public string Telefone { get; set; }

        /// <summary>
        /// Tipo de atuação do funcionário (Motoboy, atendente, etc)
        /// MIN.: 1
        /// </summary>
        public TipoFuncionario Tipo { get; set; }
        
    }
}
