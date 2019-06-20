namespace PizzaByteEnum
{
    /// <summary>
    /// Contém os enumeradores utilizados nas entidades do projeto
    /// </summary>
    public abstract class Enumeradores
    {
        /// <summary>
        /// Opções de tipos de produtos possíveis
        /// </summary>
        public enum TipoProduto
        {
            NaoIdentificado = 0,
            Pizza = 1,
            Bebida = 2
        }

        /// <summary>
        /// Opções de tipos de pedidos possíveis
        /// </summary>
        public enum TipoPedido
        {
            NaoIdentificado = 0,
            Balcao = 1,
            Retirada = 2,
            Entrega = 3
        }

        /// <summary>
        /// Possívei tipos de funcionários
        /// </summary>
        public enum TipoFuncionario
        {
            NaoIdentificado = 0,
            Motoboy = 1,
            Atendente = 2,
            Cozinheiro = 3,
            Gestor = 4
        }

        /// <summary>
        /// Possíveis status para as contas
        /// </summary>
        public enum StatusConta
        {
            NaoIdentificado = 0,
            Aberta = 1,
            Paga = 2,
            Estornada = 3,
            Perdida = 4
        }

        /// <summary>
        /// Opções de recurso de um log
        /// </summary>
        public enum LogRecursos
        {
            NaoIdentificado = 0,
            BaseIncluir = 1,
            BaseEditar = 2,
            BaseObter = 3,
            BaseObterListaFiltrada = 4,
            BaseExcluir = 5,
            ExcluirFornecedor = 6,
            IncluirFornecedor = 7,
            EditarFornecedor = 8,
            ObterFornecedor = 9,
            ObterListaFornecedor = 10
        }

        public enum TipoMensagemSuporte
        {
            NaoIdentificado = 0,
            Usuario = 1,
            Atendente = 2
        }
    }
}
