using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaByteBll;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using System;

namespace PizzaByteTestesUnitarios
{
    [TestClass]
    public class FornecedorTestes
    {
        /// <summary>
        /// Retorna um fornecedor para teste
        /// </summary>
        /// <returns></returns>
        public FornecedorDto RetornarNovoFornecedor()
        {
            FornecedorDto entidadeDto = new FornecedorDto()
            {
                NomeFantasia = "Fornecedor teste " + DateTime.Now.ToString(),
                RazaoSocial = "Fornecedor razão " + DateTime.Now.ToString(),
                Id = Guid.NewGuid(),
                Inativo = false,
                Cnpj = "",
                ComplementoEndereco = "Complemento",
                NumeroEndereco = "10",
                Obs = "Observações gerais",
                Telefone = "(19) 98101-5120"
            };

            return entidadeDto;
        }

        /// <summary>
        /// Testa a inclusão, obtenção, alteração e exclusão de um fornecedor
        /// </summary>
        [TestMethod]
        public void CrudFornecedor()
        {
            RequisicaoEntidadeDto<FornecedorDto> requisicaoDto = new RequisicaoEntidadeDto<FornecedorDto>()
            {
                EntidadeDto = RetornarNovoFornecedor()
            };

            Assert.IsTrue(Utilidades.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));
            FornecedorBll fornecedorBll = new FornecedorBll(true);

            // Incluir
            RetornoDto retornoDto = new RetornoDto();
            fornecedorBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Editar
            requisicaoDto.EntidadeDto.NomeFantasia = "Fornecedor atualizado " + DateTime.Now;
            fornecedorBll.Editar(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Obter
            RetornoObterDto<FornecedorDto> retornoObterDto = new RetornoObterDto<FornecedorDto>();
            RequisicaoObterDto requisicaoObterDto = new RequisicaoObterDto()
            {
                Id = requisicaoDto.EntidadeDto.Id,
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao
            };

            fornecedorBll.Obter(requisicaoObterDto, ref retornoObterDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Obter lista
            RequisicaoObterListaDto requisicaoObterListaDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "NOMEFANTASIA",
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao,
                NumeroItensPorPagina = 2,
                Pagina = 1
            };

            RetornoObterListaDto<FornecedorDto> retornoObterListaDto = new RetornoObterListaDto<FornecedorDto>();
            fornecedorBll.ObterListaFiltrada(requisicaoObterListaDto, ref retornoObterListaDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Excluir
            fornecedorBll.Excluir(requisicaoObterDto, ref retornoDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);
        }

        /// <summary>
        /// Testa a inclusão com campos inválidos
        /// </summary>
        [TestMethod]
        public void TestarInclusaoComFalha()
        {
            RequisicaoEntidadeDto<FornecedorDto> requisicaoDto = new RequisicaoEntidadeDto<FornecedorDto>()
            {
                EntidadeDto = RetornarNovoFornecedor()
            };

            Assert.IsTrue(Utilidades.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));
            FornecedorBll fornecedorBll = new FornecedorBll(true);

            requisicaoDto.EntidadeDto.Id = Guid.Empty;

            // Incluir com id inválido
            RetornoDto retornoDto = new RetornoDto();
            fornecedorBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(false, retornoDto.Retorno);
            Assert.AreEqual("O ID da entidade é obrigatório!", retornoDto.Mensagem);

            requisicaoDto.EntidadeDto.Id = Guid.NewGuid();

            // Incluir com código malicioso
            requisicaoDto.EntidadeDto.NomeFantasia = "";

            retornoDto = new RetornoDto();
            fornecedorBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(false, retornoDto.Retorno);
            Assert.AreEqual("O ID da entidade é obrigatório!", retornoDto.Mensagem);

        }
    }
}
