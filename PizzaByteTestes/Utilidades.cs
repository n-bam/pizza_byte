using PizzaByteBll;
using PizzaByteDto.ClassesBase;
using System;

namespace PizzaByteTestes
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
            string identificacao = "";
            Guid idUsuario = Guid.NewGuid();

            if (!usuarioBll.FazerLoginTestes(ref identificacao, idUsuario))
            {
                return false;
            }

            requisicaoDto.Indetificacao = identificacao;
            requisicaoDto.IdUsuario = idUsuario;

            return true;
        }
    }
}
