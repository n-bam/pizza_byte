using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaByteBll;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteTestes;
using System;

namespace NacoesTestesUnitarios
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

        [TestMethod]
        public void IncluirCliente()
        {
            RequisicaoEntidadeDto<ClienteDto> requisicaoDto = new RequisicaoEntidadeDto<ClienteDto>()
            {
                EntidadeDto = RetornarNovoCliente()
            };

            Assert.IsTrue(Utilidades.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));

            ClienteBll clienteBll = new ClienteBll(true);
            RetornoDto retornoDto = new RetornoDto();
            clienteBll.Incluir(requisicaoDto, ref retornoDto);

            Assert.AreEqual(true, retornoDto.Retorno);
        }
    }
}
