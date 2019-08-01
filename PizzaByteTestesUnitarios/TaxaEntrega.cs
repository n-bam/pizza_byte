using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaByteBll;
using PizzaByteDto.ClassesBase;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using System;

namespace PizzaByteTestesUnitarios
{
    [TestClass]
    public class TaxaEntregaTestes
    {
        /// <summary>
        /// Retorna uma taxa de entrega para teste
        /// </summary>
        /// <returns></returns>
        public TaxaEntregaDto RetornarNovoTaxaEntrega()
        {
            TaxaEntregaDto taxaEntregaDto = new TaxaEntregaDto()
            {
                Bairro = "Bairro teste",
                Id = Guid.NewGuid(),
                Inativo = false,
                ValorTaxa = 5.2F,
            };

            return taxaEntregaDto;
        }

        /// <summary>
        /// Testa a inclusão, obtenção, alteração e exclusão de uma taxa de entrega
        /// </summary>
        [TestMethod]
        public void CrudTaxaEntrega()
        {
            RequisicaoEntidadeDto<TaxaEntregaDto> requisicaoDto = new RequisicaoEntidadeDto<TaxaEntregaDto>()
            {
                EntidadeDto = RetornarNovoTaxaEntrega()
            };

            Assert.IsTrue(Utilidades.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));
            TaxaEntregaBll taxaEntregaBll = new TaxaEntregaBll(true);

            // Incluir
            RetornoDto retornoDto = new RetornoDto();
            taxaEntregaBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Editar
            requisicaoDto.EntidadeDto.Bairro = "Outro bairro";
            taxaEntregaBll.Editar(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Obter
            RetornoObterDto<TaxaEntregaDto> retornoObterDto = new RetornoObterDto<TaxaEntregaDto>();
            RequisicaoObterDto requisicaoObterDto = new RequisicaoObterDto()
            {
                Id = requisicaoDto.EntidadeDto.Id,
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao
            };

            taxaEntregaBll.Obter(requisicaoObterDto, ref retornoObterDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Obter lista
            RequisicaoObterListaDto requisicaoObterListaDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "BAIRRO",
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao,
                NumeroItensPorPagina = 2,
                Pagina = 1
            };

            RetornoObterListaDto<TaxaEntregaDto> retornoObterListaDto = new RetornoObterListaDto<TaxaEntregaDto>();
            taxaEntregaBll.ObterListaFiltrada(requisicaoObterListaDto, ref retornoObterListaDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Excluir
            taxaEntregaBll.Excluir(requisicaoObterDto, ref retornoDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);
        }

        /// <summary>
        /// Obtem uma lista de bairros e edita o valor das taxas
        /// </summary>
        [TestMethod]
        public void ObterIncluirEditarListaTaxas()
        {
            BaseRequisicaoDto requisicaoDto = new BaseRequisicaoDto();
            Assert.IsTrue(Utilidades.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));
            TaxaEntregaBll taxaEntregaBll = new TaxaEntregaBll(true);

            // Obter a lista
            RetornoObterListaDto<TaxaEntregaDto> retornoDto = new RetornoObterListaDto<TaxaEntregaDto>();
            taxaEntregaBll.ObterListaBairrosComTaxa(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            RequisicaoListaEntidadesDto<TaxaEntregaDto> requisicaoIncluirDto = new RequisicaoListaEntidadesDto<TaxaEntregaDto>()
            {
                Identificacao = requisicaoDto.Identificacao,
                IdUsuario = requisicaoDto.IdUsuario,
                ListaEntidadesDto = retornoDto.ListaEntidades
            };

            RetornoDto retornoIncluirDto = new RetornoDto();
            taxaEntregaBll.IncluirEditarLista(requisicaoIncluirDto, ref retornoIncluirDto);
            Assert.AreEqual(true, retornoDto.Retorno);
        }
    }
}
