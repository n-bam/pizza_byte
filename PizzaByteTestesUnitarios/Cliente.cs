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
                Telefone = "19987456523"
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

        /// <summary>
        /// Testa a falha da inclusão, obtenção, alteração e exclusão de um cliente
        /// </summary>
        [TestMethod]
        public void CrudClienteComErro()
        {
            RequisicaoEntidadeDto<ClienteDto> requisicaoDto = new RequisicaoEntidadeDto<ClienteDto>()
            {
                EntidadeDto = RetornarNovoCliente()
            };

            Assert.IsTrue(Utilidades.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));
            ClienteBll clienteBll = new ClienteBll(true);

            // Incluir
            requisicaoDto.EntidadeDto.Nome = "";
            RetornoDto retornoDto = new RetornoDto();
            clienteBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(false, retornoDto.Retorno);
            Assert.AreEqual("O nome do cliente é obrigatório! Por favor, informe o nome do cliente " +
                    "no campo indicado para continuar. ", retornoDto.Mensagem);

            requisicaoDto.EntidadeDto.Nome = "Te";
            retornoDto = new RetornoDto();
            clienteBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(false, retornoDto.Retorno);
            Assert.AreEqual("O nome do cliente deve ter, ao menos, 3 caracteres! Por favor, informe um nome " +
                    "válido para continuar. ", retornoDto.Mensagem);

            requisicaoDto.EntidadeDto.Nome = "Testes";
            retornoDto = new RetornoDto();
            clienteBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);
           
            // Editar
            requisicaoDto.EntidadeDto.Nome = "";
            clienteBll.Editar(requisicaoDto, ref retornoDto);
            Assert.AreEqual(false, retornoDto.Retorno);
            Assert.AreEqual("O nome do cliente é obrigatório! Por favor, informe o nome do cliente " +
                    "no campo indicado para continuar. ", retornoDto.Mensagem);

            // Obter
            RetornoObterDto<ClienteDto> retornoObterDto = new RetornoObterDto<ClienteDto>();
            RequisicaoObterDto requisicaoObterDto = new RequisicaoObterDto()
            {
                Id = Guid.Empty,
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao
            };

            clienteBll.Obter(requisicaoObterDto, ref retornoObterDto);
            Assert.AreEqual(false, retornoObterDto.Retorno);

            // Excluir
            clienteBll.Excluir(requisicaoObterDto, ref retornoDto);
            Assert.AreEqual(false, retornoObterDto.Retorno);

        }

    }
}
