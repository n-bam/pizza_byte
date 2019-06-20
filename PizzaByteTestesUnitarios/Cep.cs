using Microsoft.VisualStudio.TestTools.UnitTesting;
using PizzaByteBll;
using PizzaByteDto.Base;
using PizzaByteDto.RetornosRequisicoes;
using System;

namespace PizzaByteTestesUnitarios
{
    [TestClass]
    public class CepTestes
    {
        /// <summary>
        /// Retorna um cep para teste
        /// </summary>
        /// <returns></returns>
        public CepDto RetornarNovoCep()
        {
            CepDto cepDto = new CepDto()
            {
                Bairro = "Pq Franceschini",
                Cidade = "Sumaré",
                Id = Guid.NewGuid(),
                Inativo = false,
                Logradouro = "Rua João Franceschini",
                Cep = "13084200",
                Excluido = false
            };

            return cepDto;
        }

        /// <summary>
        /// Testa a inclusão, obtenção, alteração e exclusão de um cep
        /// </summary>
        [TestMethod]
        public void CrudCep()
        {
            RequisicaoEntidadeDto<CepDto> requisicaoDto = new RequisicaoEntidadeDto<CepDto>()
            {
                EntidadeDto = RetornarNovoCep()
            };

            Assert.IsTrue(Utilidades.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));
            CepBll cepBll = new CepBll(true);

            // Incluir
            RetornoDto retornoDto = new RetornoDto();
            cepBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Editar
            requisicaoDto.EntidadeDto.Logradouro = "Cep atualizado " + DateTime.Now;
            cepBll.Editar(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Obter
            RetornoObterDto<CepDto> retornoObterDto = new RetornoObterDto<CepDto>();
            RequisicaoObterDto requisicaoObterDto = new RequisicaoObterDto()
            {
                Id = requisicaoDto.EntidadeDto.Id,
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao
            };

            cepBll.Obter(requisicaoObterDto, ref retornoObterDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Excluir
            cepBll.Excluir(requisicaoObterDto, ref retornoDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

        }
    }
}
