using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaByteBll;
using PizzaByteBll.Base;
using PizzaByteDto.Base;
using PizzaByteDto.RetornosRequisicoes;
using System;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteTestesUnitarios
{
    [TestClass]
    public class SuporteTestes
    {
        /// <summary>
        /// Retorna um suporte para teste
        /// </summary>
        /// <returns></returns>
        public SuporteDto RetornarNovoSuporte()
        {
            SuporteDto entidadeDto = new SuporteDto()
            {
                Mensagem = "Teste unitário: \n Suporte teste " + DateTime.Now.ToString(),
                Id = Guid.NewGuid(),
                Tipo = TipoMensagemSuporte.Usuario,
                Inativo = false,
            };

            return entidadeDto;
        }

        /// <summary>
        /// Testa a inclusão, obtenção e exclusão de um suporte
        /// </summary>
        [TestMethod]
        public void CrudSuporte()
        {
            RequisicaoEntidadeDto<SuporteDto> requisicaoDto = new RequisicaoEntidadeDto<SuporteDto>()
            {
                EntidadeDto = RetornarNovoSuporte()
            };

            Assert.IsTrue(Utilidades.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));
            SuporteBll suporteBll = new SuporteBll(true);

            // Incluir
            RetornoDto retornoDto = new RetornoDto();
            suporteBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);
            
            // Obter
            RetornoObterDto<SuporteDto> retornoObterDto = new RetornoObterDto<SuporteDto>();
            RequisicaoObterDto requisicaoObterDto = new RequisicaoObterDto()
            {
                Id = requisicaoDto.EntidadeDto.Id,
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao
            };

            suporteBll.Obter(requisicaoObterDto, ref retornoObterDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Obter lista
            RequisicaoObterListaDto requisicaoObterListaDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "DATADECRESCENTE",
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao,
                NumeroItensPorPagina = 2,
                Pagina = 1
            };

            RetornoObterListaDto<SuporteDto> retornoObterListaDto = new RetornoObterListaDto<SuporteDto>();
            suporteBll.ObterListaFiltrada(requisicaoObterListaDto, ref retornoObterListaDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Excluir
            suporteBll.Excluir(requisicaoObterDto, ref retornoDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);
        }
    }
}
