using PizzaByteDto.Base;

namespace PizzaByteDto.RetornosRequisicoes
{
    public class RetornoObterDto<T> : RetornoDto where T : BaseEntidadeDto
    {
        public T Entidade { get; set; }
    }
}
