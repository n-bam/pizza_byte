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
            Pizzaiolo = 3,
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
            ObterListaFornecedor = 10,
            IncluirCep = 11,
            ExcluirCep = 12,
            ObterCep = 13,
            ObterCepPorCep = 14,
            ObterListaCep = 15,
            EditarCep = 16,
            IncluirProduto = 17,
            ExcluirProduto = 18,
            ObterProduto = 19,
            ObterListaProduto = 20,
            EditarProduto = 21,
            IncluirUsuario = 22,
            ExcluirUsuario = 23,
            FazerLogin = 24,
            EnviarEmailRecuperacao = 25,
            ObterUsuario = 26,
            ObterListaUsuario = 27,
            EditarUsuario = 28,
            IncluirTaxaEntrega = 29,
            EditarTaxaEntrega = 30,
            ExcluirTaxaEntrega = 31,
            ObterTaxaEntrega = 32,
            ObterListaTaxaEntrega = 33,
            IncluirAlterarListaTaxaEntrega = 34,
            ObterListaBairrosComTaxas = 35,
            ObterListaBairro = 36,
            ObterCidadePorBairro = 37,
            IncluirCliente = 38,
            ExcluirCliente = 39,
            ObterCliente = 40,
            ObterListaCliente = 41,
            EditarCliente = 42,
            IncluirClienteEndereco = 43,
            EditarClienteEndereco = 44,
            ExcluirClienteEndereco = 45,
            ObterClienteEndereco = 46,
            ObterListaClienteEndereco = 47,
            IncluirSuporte = 48,
            ExcluirSuporte = 49,
            ObterSuporte = 50,
            ObterListaSuporte = 51,
            ExcluirFuncionario = 52,
            IncluirFuncionario = 53,
            EditarFuncionario = 54,
            ObterFuncionario = 55,
            ObterListaFuncionario = 56,
            ObterListaLog = 57,
            ObterListaUsuariosParaSelecao = 58,
            VerificarProdutoExistente = 59,
            Backup = 60,
            RestaurarBackup = 61
        }

        public enum TipoMensagemSuporte
        {
            NaoIdentificado = 0,
            Usuario = 1,
            Atendente = 2
        }
    }
}
