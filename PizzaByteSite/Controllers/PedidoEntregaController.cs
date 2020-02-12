using PizzaByteBll;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteSite.Models;
using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PizzaByteSite.Controllers
{
    public class PedidoEntregaController : Controller
    {
        /// <summary>
        /// Chama a tela com a listagem de entregas
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            // Model a ser utilizada na tela
            string mensagemErro = "";
            FiltrosPedidoEntregaModel model = new FiltrosPedidoEntregaModel()
            {
                Pagina = 1,
                DataInicio = DateTime.Now,
                DataFim = DateTime.Now,
                Conferido = "false"
            };

            // Se houver algum erro para popular a model
            if (!string.IsNullOrWhiteSpace(mensagemErro))
            {
                ViewBag.MensagemErro = "Problemas para popular a model: " + mensagemErro;
                return View("Erro");
            }

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para visualizar um fornecedor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Visualizar(Guid id)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Model a ser populada
            PedidoEntregaModel model = new PedidoEntregaModel();
            string mensagemRetorno = "";

            //Obtem o fornecedor pelo ID
            if (!this.ObterPedidoEntrega(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "VISUALIZANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para editar um entrega
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Editar(Guid id)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para alterar os dados de uma entrega é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            PedidoEntregaModel model = new PedidoEntregaModel();
            string mensagemRetorno = "";

            //Obtem o entrega pelo ID
            if (!this.ObterPedidoEntrega(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "EDITANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para salvar os dados do entrega
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(PedidoEntregaModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para alterar os dados de uma entrega é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Valida a entidade recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Converte para DTO
            PedidoEntregaDto entregaDto = new PedidoEntregaDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref entregaDto, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<PedidoEntregaDto> requisicaoDto = new RequisicaoEntidadeDto<PedidoEntregaDto>()
            {
                EntidadeDto = entregaDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            PedidoEntregaBll entregaBll = new PedidoEntregaBll(true);
            entregaBll.Editar(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "ALTERADO";

            //Voltar para o visualizar do entrega
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Obtem um entrega e converte em Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ObterPedidoEntrega(Guid id, ref PedidoEntregaModel model, ref string mensagemErro)
        {
            //Preparar a requisição e o retorno
            RetornoObterDto<PedidoEntregaDto> retorno = new RetornoObterDto<PedidoEntregaDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            PedidoEntregaBll entregaBll = new PedidoEntregaBll(true);
            entregaBll.Obter(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                mensagemErro = retorno.Mensagem;
                return false;
            }
            else
            {
                //Converter para Model
                return model.ConverterDtoParaModel(retorno.Entidade, ref mensagemErro);
            }
        }

        /// <summary>
        /// Obtem uma listra filtrada de entregas
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string ObterListaFiltrada(FiltrosPedidoEntregaModel filtros)
        {
            //Requisição para obter a lista
            RequisicaoObterListaDto requisicaoDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "DATAINCLUSAO",
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                NaoPaginarPesquisa = filtros.NaoPaginaPesquisa,
                Pagina = filtros.Pagina,
                NumeroItensPorPagina = (filtros.NumeroItensPagina == 0) ? 20 : filtros.NumeroItensPagina
            };

            //Adicionar filtros utilizados
            if (!string.IsNullOrWhiteSpace(filtros.Conferido))
            {
                requisicaoDto.ListaFiltros.Add("CONFERIDO", filtros.Conferido.Trim());
            }

            if (filtros.ValorRetorno != 0)
            {
                requisicaoDto.ListaFiltros.Add("VALORRETORNO", filtros.ValorRetorno.ToString());
            }

            if (!string.IsNullOrWhiteSpace(filtros.ObterInativos))
            {
                requisicaoDto.ListaFiltros.Add("INATIVO", filtros.ObterInativos.Trim());
            }

            if (filtros.IdFuncionario != null && filtros.IdFuncionario == Guid.Empty)
            {
                requisicaoDto.ListaFiltros.Add("IDFUNCIONARIO", filtros.IdFuncionario.ToString());
            }

            requisicaoDto.ListaFiltros.Add("DATAINCLUSAOINICIO", filtros.DataInicio.ToString());
            requisicaoDto.ListaFiltros.Add("DATAINCLUSAOFIM", filtros.DataFim.ToString());

            //Consumir o serviço
            PedidoEntregaBll entregaBll = new PedidoEntregaBll(true);
            RetornoObterListaDto<PedidoEntregaDto> retornoDto = new RetornoObterListaDto<PedidoEntregaDto>();
            entregaBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Obtem uma listra filtrada de entregas
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string ConferirEntrega(Guid idEntrega, float valorRetorno)
        {
            RetornoDto retornoDto = new RetornoDto();
            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Para alterar os dados de uma entrega é necessário " +
                    $"logar com um usuário administrador.";
            }
            else
            {
                //Requisição para conferir
                RequisicaoConferirEntregaDto requisicaoDto = new RequisicaoConferirEntregaDto()
                {
                    IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                    Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                    ValorRetornado = valorRetorno,
                    Id = idEntrega
                };

                //Consumir o serviço
                PedidoEntregaBll entregaBll = new PedidoEntregaBll(true);
                entregaBll.ConferirEntrega(requisicaoDto, ref retornoDto);
            }

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Determina o funcionário que fará a entrega
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string AlterarFuncionarioEntrega(Guid idEntrega, Guid idFuncionario)
        {
            //Requisição para conferir
            RequisicaoAlterarFuncionarioEntregaDto requisicaoDto = new RequisicaoAlterarFuncionarioEntregaDto()
            {
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdFuncionario = idFuncionario,
                Id = idEntrega
            };

            //Consumir o serviço
            PedidoEntregaBll entregaBll = new PedidoEntregaBll(true);
            RetornoDto retornoDto = new RetornoDto();
            entregaBll.AlterarFuncionarioEntrega(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}