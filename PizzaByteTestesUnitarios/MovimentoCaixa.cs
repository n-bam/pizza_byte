using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaByteBll;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using System;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteTestesUnitarios
{
    [TestClass]
    public class MovimentoCaixaTestes
    {
        /// <summary>
        /// Retorna um movimento de caixa para teste
        /// </summary>
        /// <returns></returns>
        public static MovimentoCaixaDto RetornarNovoMovimentoCaixa()
        {
            MovimentoCaixaDto entidadeDto = new MovimentoCaixaDto()
            {
                Justificativa = "Teste unitário " + DateTime.Now.ToString(),
                Id = Guid.NewGuid(),
                Valor = DateTime.Now.Millisecond
            };

            return entidadeDto;
        }

        /// <summary>
        /// Testa a inclusão, obtenção, alteração e exclusão de um movimento de caixa
        /// </summary>
        [TestMethod]
        public void CrudMovimentoCaixa()
        {
            RequisicaoEntidadeDto<MovimentoCaixaDto> requisicaoDto = new RequisicaoEntidadeDto<MovimentoCaixaDto>()
            {
                EntidadeDto = RetornarNovoMovimentoCaixa()
            };

            Assert.IsTrue(Utilidades.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));
            MovimentoCaixaBll movimentoCaixaBll = new MovimentoCaixaBll(true);

            // Incluir
            RetornoDto retornoDto = new RetornoDto();
            movimentoCaixaBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Editar
            requisicaoDto.EntidadeDto.Justificativa = "Teste atualizado " + DateTime.Now;
            movimentoCaixaBll.Editar(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Obter
            RetornoObterDto<MovimentoCaixaDto> retornoObterDto = new RetornoObterDto<MovimentoCaixaDto>();
            RequisicaoObterDto requisicaoObterDto = new RequisicaoObterDto()
            {
                Id = requisicaoDto.EntidadeDto.Id,
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao
            };

            movimentoCaixaBll.Obter(requisicaoObterDto, ref retornoObterDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Obter lista
            RequisicaoObterListaDto requisicaoObterListaDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "JUSTIFICATIVA",
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao,
                NumeroItensPorPagina = 2,
                Pagina = 1
            };

            RetornoObterListaDto<MovimentoCaixaDto> retornoObterListaDto = new RetornoObterListaDto<MovimentoCaixaDto>();
            movimentoCaixaBll.ObterListaFiltrada(requisicaoObterListaDto, ref retornoObterListaDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Excluir
            movimentoCaixaBll.Excluir(requisicaoObterDto, ref retornoDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);
        }
    }
}
