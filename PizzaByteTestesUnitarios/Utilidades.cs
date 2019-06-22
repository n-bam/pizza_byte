using PizzaByteBll;
using PizzaByteBll.Base;
using PizzaByteDto.ClassesBase;
using PizzaByteDto.RetornosRequisicoes;
using System;

namespace PizzaByteTestesUnitarios
{
    public static class Utilidades
    {
        /// <summary>
        /// Preenche uma requição com a identificação e id do usuário
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <returns></returns>
        public static bool RetornarAutenticacaoRequisicaoPreenchida(BaseRequisicaoDto requisicaoDto)
        {
            UsuarioBll usuarioBll = new UsuarioBll(true);
            string senhaCriptografada = "";
            UtilitarioBll.CriptografarSenha(DateTime.Now.AddDays(-2).Date.ToString("dd/MM/yyyy").Replace("/", ""), ref senhaCriptografada);

            RequisicaoFazerLoginDto requisicaoLoginDto = new RequisicaoFazerLoginDto()
            {
                Email = "Suporte",
                Senha = senhaCriptografada
            };

            RetornoFazerLoginDto retornoDto = new RetornoFazerLoginDto();
            if (!usuarioBll.FazerLogin(requisicaoLoginDto, ref retornoDto))
            {
                return false;
            }

            requisicaoDto.Identificacao = retornoDto.Identificacao;
            requisicaoDto.IdUsuario = retornoDto.IdUsuario;

            return true;
        }
    }
}
