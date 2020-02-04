using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaByteBll;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using System;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteTestesUnitarios
{
    [TestClass]
    public class ProdutoTestes
    {
        /// <summary>
        /// Retorna um produto para teste
        /// </summary>
        /// <returns></returns>
        public static ProdutoDto RetornarNovoProduto()
        {
            ProdutoDto entidadeDto = new ProdutoDto()
            {
                Descricao = "Produto teste " + DateTime.Now.ToString(),
                Id = Guid.NewGuid(),
                Preco = DateTime.Now.Millisecond,
                Tipo = TipoProduto.Pizza,
                Inativo = false,
            };

            return entidadeDto;
        }

        /// <summary>
        /// Testa a inclusão, obtenção, alteração e exclusão de um produto
        /// </summary>
        [TestMethod]
        public void CrudProduto()
        {
            RequisicaoEntidadeDto<ProdutoDto> requisicaoDto = new RequisicaoEntidadeDto<ProdutoDto>()
            {
                EntidadeDto = RetornarNovoProduto()
            };

            Assert.IsTrue(Utilidades.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));
            ProdutoBll produtoBll = new ProdutoBll(true);

            // Incluir
            RetornoDto retornoDto = new RetornoDto();
            produtoBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Editar
            requisicaoDto.EntidadeDto.Descricao = "Produto atualizado " + DateTime.Now;
            produtoBll.Editar(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Obter
            RetornoObterDto<ProdutoDto> retornoObterDto = new RetornoObterDto<ProdutoDto>();
            RequisicaoObterDto requisicaoObterDto = new RequisicaoObterDto()
            {
                Id = requisicaoDto.EntidadeDto.Id,
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao
            };

            produtoBll.Obter(requisicaoObterDto, ref retornoObterDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Obter lista
            RequisicaoObterListaDto requisicaoObterListaDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "DESCRICAO",
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao,
                NumeroItensPorPagina = 2,
                Pagina = 1
            };

            RetornoObterListaDto<ProdutoDto> retornoObterListaDto = new RetornoObterListaDto<ProdutoDto>();
            produtoBll.ObterListaFiltrada(requisicaoObterListaDto, ref retornoObterListaDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Excluir
            produtoBll.Excluir(requisicaoObterDto, ref retornoDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);
        }

        public static ProdutoDto IncluirProdutoTeste()
        {
            RequisicaoEntidadeDto<ProdutoDto> requisicaoDto = new RequisicaoEntidadeDto<ProdutoDto>()
            {
                EntidadeDto = RetornarNovoProduto()
            };

            Assert.IsTrue(Utilidades.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));
            ProdutoBll produtoBll = new ProdutoBll(true);

            // Incluir
            RetornoDto retornoDto = new RetornoDto();
            produtoBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            return requisicaoDto.EntidadeDto;
        }
    }
}
