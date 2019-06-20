using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaByteBll;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using System;

namespace PizzaByteTestesUnitarios
{
    [TestClass]
    public class ClienteTestes
    {
        /// <summary>
        /// Retorna um cliente para teste
        /// </summary>
        /// <returns></returns>
        public ClienteDto RetornarNovoCliente()
        {
            ClienteDto clienteDto = new ClienteDto()
            {
                Nome = "Cliente teste " + DateTime.Now.ToString(),
                Cpf = "",
                Id = Guid.NewGuid(),
                Inativo = false,
                Telefone = "333333333"
            };

            return clienteDto;
        }

        /// <summary>
        /// Testa a inclusão, obtenção, alteração e exclusão de um cliente
        /// </summary>
        [TestMethod]
        public void CrudCliente()
        {
            RequisicaoEntidadeDto<ClienteDto> requisicaoDto = new RequisicaoEntidadeDto<ClienteDto>()
            {
                EntidadeDto = RetornarNovoCliente()
            };

            Assert.IsTrue(Utilidades.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));
            ClienteBll clienteBll = new ClienteBll(true);

            // Incluir
            RetornoDto retornoDto = new RetornoDto();
            clienteBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Editar
            requisicaoDto.EntidadeDto.Nome = "Cliente atualizado " + DateTime.Now;
            clienteBll.Editar(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Obter
            RetornoObterDto<ClienteDto> retornoObterDto = new RetornoObterDto<ClienteDto>();
            RequisicaoObterDto requisicaoObterDto = new RequisicaoObterDto()
            {
                Id = requisicaoDto.EntidadeDto.Id,
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao
            };

            clienteBll.Obter(requisicaoObterDto, ref retornoObterDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Excluir
            clienteBll.Excluir(requisicaoObterDto, ref retornoDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

        }
        
    }
}
