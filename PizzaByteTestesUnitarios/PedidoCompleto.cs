using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaByteBll;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using System;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteTestesUnitarios
{
    [TestClass]
    public class PedidoCompletoTestes
    {
        /// <summary>
        /// Retorna um apenas com itens
        /// </summary>
        /// <returns></returns>
        public PedidoDto RetornarNovoPedidoSimples()
        {
            PedidoDto pedidoDto = new PedidoDto()
            {
                IdCliente = null,
                Id = Guid.NewGuid(),
                Inativo = false,
                JustificativaCancelamento = "",
                Obs = "Sem cebola",
                PedidoIfood = false,
                RecebidoCredito = 0,
                RecebidoDebito = 0,
                RecebidoDinheiro = 50,
                TaxaEntrega = 0,
                Tipo = TipoPedido.Balcão,
                Total = 50,
                Troco = 0
            };

            ProdutoDto produto = ProdutoTestes.IncluirProdutoTeste();
            pedidoDto.ListaItens.Add(new PedidoItemDto()
            {
                Inativo = false,
                DescricaoProduto = produto.Descricao,
                IdProdutoComposto = null,
                IdProduto = produto.Id,
                PrecoProduto = 25,
                Quantidade = 1,
                TipoProduto = TipoProduto.Pizza
            });

            produto = ProdutoTestes.IncluirProdutoTeste();
            pedidoDto.ListaItens.Add(new PedidoItemDto()
            {
                Inativo = false,
                DescricaoProduto = produto.Descricao,
                IdProdutoComposto = null,
                IdProduto = produto.Id,
                PrecoProduto = 25,
                Quantidade = 1,
                TipoProduto = TipoProduto.Pizza
            });

            return pedidoDto;
        }

        /// <summary>
        /// Inclui, obtem, edita e exclui um pedido
        /// </summary>
        [TestMethod]
        public void CrudPedidoSimples()
        {
            RequisicaoEntidadeDto<PedidoDto> requisicaoDto = new RequisicaoEntidadeDto<PedidoDto>()
            {
                EntidadeDto = RetornarNovoPedidoSimples()
            };

            Assert.IsTrue(Utilidades.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));
            PedidoBll pedidoBll = new PedidoBll(true);

            // Incluir pedido
            RetornoDto retornoDto = new RetornoDto();
            pedidoBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            RetornoObterDto<PedidoDto> retornoObterDto = new RetornoObterDto<PedidoDto>();
            RequisicaoObterDto requisicaoObterDto = new RequisicaoObterDto()
            {
                Id = requisicaoDto.EntidadeDto.Id,
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao
            };

            // Obter pedido
            pedidoBll.Obter(requisicaoObterDto, ref retornoObterDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            requisicaoDto.EntidadeDto = retornoObterDto.Entidade;
            requisicaoDto.EntidadeDto.ListaItens[0].PrecoProduto = 28;
            requisicaoDto.EntidadeDto.ListaItens.RemoveAt(1);
            requisicaoDto.EntidadeDto.Total = 28;
            requisicaoDto.EntidadeDto.RecebidoDebito = 28;
            requisicaoDto.EntidadeDto.RecebidoDinheiro = 0;

            // Editar pedido
            retornoDto = new RetornoDto();
            pedidoBll.Editar(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Excluir pedido
            retornoDto = new RetornoDto();
            pedidoBll.Excluir(requisicaoObterDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);
        }
    }
}
