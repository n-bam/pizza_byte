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
            Balcão = 1,
            Retirada = 2,
            Entrega = 3
        }

        /// <summary>
        /// Possívei tipos de funcionários
        /// </summary>
        public enum TipoFuncionario
        {
            NaoIdentificado = 0,
            Atendente = 1,
            Gestor = 2,
            Motoboy = 3,
            Pizzaiolo = 4,

        }

        public enum TipoEnderecoCidade
        {
            NaoIdentificado = 0,
            Americana = 1,
            SantaBarbara = 2,

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
            RestaurarBackup = 61,
            IncluirPedido = 62,
            ObterPedido = 63,
            ObterListaPedido = 64,
            EditarPedido = 65,
            ExcluirPedido = 66,
            IncluirPedidoItem = 67,
            ObterPedidoItem = 68,
            ObterListaPedidoItem = 69,
            EditarPedidoItem = 70,
            ExcluirPedidoItem = 71,
            IncluirListaPedidoItem = 72,
            EditarListaPedidoItem = 73,
            ExcluirListaItensPedido = 74,
            IncluirEditarCliente = 75,
            IncluirEditarClienteEndereco = 76,
            IncluirEditarCep = 77,
            ObterTaxaPorBairro = 78,
            IncluirEditarTaxaEntrega = 79,
            ExcluirEnderecosPorIdCliente = 80,
            ExcluirContaPagar = 81,
            IncluirContaPagar = 82,
            EditarContaPagar = 83,
            ObterContaPagar = 84,
            ObterListaContaPagar = 85,
            ExcluirContaReceber = 86,
            IncluirContaReceber = 87,
            EditarContaReceber = 88,
            ObterContaReceber = 89,
            ObterListaContaReceber = 90,
            ExcluirPedidoEntrega = 91,
            IncluirPedidoEntrega = 92,
            EditarPedidoEntrega = 93,
            ObterPedidoEntrega = 94,
            ObterListaPedidoEntrega = 95,
            IncluirEditarPedidoEntrega = 96,
            ObterListaEnderecosClientePorId = 97,
            CancelarPedido = 98,
            ObterPedidoResumido = 99,
            ConferirEntrega = 100,
            AlterarFuncionarioEntrega = 101,
            ObterInformacoesDashboard = 102,
            RelatorioListagemClientes = 103,
            IncluirMovimentoCaixa = 104,
            EditarMovimentoCaixa = 105,
            ObterMovimentoCaixa = 106,
            ObterListaMovimentoCaixa = 107,
            ExcluirMovimentoCaixa = 108,
            ObterTotalEntregaPorProfissional = 109,
            IncluirListaContasReceberPedido = 110,
            EditarListaContasReceberPedido = 111,
            EstornarContasReceberPedido = 112,
            ExcluirContasReceberPedido = 113,
            RelatorioMelhoresProdutos = 114,
            EstornarListaItensPedido = 115,
            RelatorioListagemProdutos = 116,
            RelatorioListagemTaxaEntrega = 117,
            RelatorioListagemResumidaPedidos = 118,
            RelatorioListagemDetalhadaPedidos = 119,
            RelatorioMelhoresClientes = 120,
            RelatorioMelhoresMotoboys = 121,
            RelatorioListagemEntregas = 122,
            RelatorioRelacaoDiariaContas = 123,
            RelatorioContasPorFornecedor = 124,
            RelatorioPedidosPorBairro = 125,
            RelatorioEvolucaoSemanalPedidos = 126,
            RelatorioEvolucaoMensalPedidos = 127,
            RelatorioEvolucaoAnualPedidos = 128,
            RelatorioListagemContaPagar = 129,
            RelatorioListagemFornecedores = 130,
            RelatorioListagemContaReceber = 131,
            
        }

        public enum TipoMensagemSuporte
        {
            NaoIdentificado = 0,
            Usuario = 1,
            Atendente = 2
        }
    }
}
