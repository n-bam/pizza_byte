using PizzaByteBll;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PizzaByteSite.Controllers
{
    public class MovimentoCaixaController : Controller
    {
        /// <summary>
        /// Chama a tela com a listagem de movimento de caixa
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Chamar a view
            return View();
        }

        /// <summary>
        /// Obtem o resumo do caixa
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public string ObterFormasPagamento(DateTime dataCaixa)
        {
            //Preparar a requisição e o retorno
            RetornoObterResumoCaixaDto retornoDto = new RetornoObterResumoCaixaDto();
            RequisicaoDataDto requisicaoDto = new RequisicaoDataDto()
            {
                Data = dataCaixa,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            MovimentoCaixaBll movimentoCaixaBll = new MovimentoCaixaBll(true);
            movimentoCaixaBll.ObterFormasPagamentoDia(requisicaoDto, ref retornoDto);

            //Tratar o retorno
            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Obtem as movimentações de caixa do dia
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public string ObterMovimentosDia(DateTime dataCaixa)
        {
            //Preparar a requisição e o retorno
            RetornoObterListaDto<MovimentoCaixaDto> retornoDto = new RetornoObterListaDto<MovimentoCaixaDto>();
            RequisicaoObterListaDto requisicaoDto = new RequisicaoObterListaDto()
            {
                NaoPaginarPesquisa = true,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            requisicaoDto.ListaFiltros.Add("DATAINCLUSAO", dataCaixa.Date.ToString("dd/MM/yyyy"));

            //Consumir o serviço
            MovimentoCaixaBll movimentoCaixaBll = new MovimentoCaixaBll(true);
            movimentoCaixaBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            //Tratar o retorno
            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Obtem os valores de entrega para cada profissional
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public string ObterTotalProfissionais(DateTime dataCaixa)
        {
            //Preparar a requisição e o retorno
            RetornoObterTotalEntregaPorProfissionalDto retornoDto = new RetornoObterTotalEntregaPorProfissionalDto();
            RequisicaoDataDto requisicaoDto = new RequisicaoDataDto()
            {
                Data = dataCaixa.Date,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            MovimentoCaixaBll movimentoCaixaBll = new MovimentoCaixaBll(true);
            movimentoCaixaBll.ObterTotalEntregaPorProfissional(requisicaoDto, ref retornoDto);

            //Tratar o retorno
            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Obtem uma listra filtrada de movimento de caixa
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string IncluirMovimento(MovimentoCaixaDto movimentoDto, bool indicadorSaida)
        {
            RetornoDto retornoDto = new RetornoDto();
            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                retornoDto.Mensagem = "Para incluir um movimento de caixa é necessário " +
                    $"logar com um usuário administrador.";
            }
            else
            {
                movimentoDto.Id = Guid.NewGuid();

                //Requisição para obter a lista
                RequisicaoEntidadeDto<MovimentoCaixaDto> requisicaoDto = new RequisicaoEntidadeDto<MovimentoCaixaDto>()
                {
                    IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                    Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                    EntidadeDto = movimentoDto
                };

                if (indicadorSaida)
                {
                    requisicaoDto.EntidadeDto.Valor = requisicaoDto.EntidadeDto.Valor * (-1);
                }

                //Consumir o serviço
                MovimentoCaixaBll movimentoCaixaBll = new MovimentoCaixaBll(true);
                movimentoCaixaBll.Incluir(requisicaoDto, ref retornoDto);
            }

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}