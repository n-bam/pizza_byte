using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaByteBll;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using System;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteTestesUnitarios
{
    [TestClass]
    public class FuncionarioTestes
    {
        /// <summary>
        /// Retorna um funcionario para teste
        /// </summary>
        /// <returns></returns>
        public FuncionarioDto RetornarNovoFuncionario()
        {
            FuncionarioDto entidadeDto = new FuncionarioDto()
            {
                Nome = "Funcionário teste " + DateTime.Now.ToString(),
                Id = Guid.NewGuid(),
                Telefone = "(19) 95455-5142",
                Tipo = TipoFuncionario.Motoboy,
                Inativo = false,
            };

            return entidadeDto;
        }

        /// <summary>
        /// Testa a inclusão, obtenção, alteração e exclusão de um funcionario
        /// </summary>
        [TestMethod]
        public void CrudFuncionario()
        {
            RequisicaoEntidadeDto<FuncionarioDto> requisicaoDto = new RequisicaoEntidadeDto<FuncionarioDto>()
            {
                EntidadeDto = RetornarNovoFuncionario()
            };

            Assert.IsTrue(Utilidades.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));
            FuncionarioBll funcionarioBll = new FuncionarioBll(true);

            // Incluir
            RetornoDto retornoDto = new RetornoDto();
            funcionarioBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Editar
            requisicaoDto.EntidadeDto.Nome = "Funcionario atualizado " + DateTime.Now;
            funcionarioBll.Editar(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Obter
            RetornoObterDto<FuncionarioDto> retornoObterDto = new RetornoObterDto<FuncionarioDto>();
            RequisicaoObterDto requisicaoObterDto = new RequisicaoObterDto()
            {
                Id = requisicaoDto.EntidadeDto.Id,
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao
            };

            funcionarioBll.Obter(requisicaoObterDto, ref retornoObterDto);
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

            RetornoObterListaDto<FuncionarioDto> retornoObterListaDto = new RetornoObterListaDto<FuncionarioDto>();
            funcionarioBll.ObterListaFiltrada(requisicaoObterListaDto, ref retornoObterListaDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Excluir
            funcionarioBll.Excluir(requisicaoObterDto, ref retornoDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);
        }
    }
}
