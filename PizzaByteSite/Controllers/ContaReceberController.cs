using PizzaByteBll;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteSite.Models;
using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Controllers
{
    public class ContaReceberController : BaseController
    {
        /// <summary>
        /// Chama a tela com a listagem de contas a receber
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }


            // Filtros da página inicial
            FiltrosContaReceberModel model = new FiltrosContaReceberModel()
            {
                Pagina = 1,
                DataInicio = DateTime.Now,
                DataFim = DateTime.Now,
                PesquisarPor = "DATAVENCIMENTO"
            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para incluir uma conta
        /// </summary>
        /// <returns></returns>
        public ActionResult Incluir()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Conta a ser incluída
            ContaReceberModel model = new ContaReceberModel()
            {
                Id = Guid.NewGuid(),
                DataCompetencia = DateTime.Now,
                DataVencimento = DateTime.Now.AddMonths(1)
            };

            TempData["Retorno"] = "INCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para incluir uma nova conta
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Incluir(ContaReceberModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Converter para DTO
            ContaReceberDto contaReceberDto = new ContaReceberDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref contaReceberDto, ref mensagemErro))
            {
                ModelState.AddModelError("Servico", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            contaReceberDto.Id = Guid.NewGuid();

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<ContaReceberDto> requisicaoDto = new RequisicaoEntidadeDto<ContaReceberDto>()
            {
                EntidadeDto = contaReceberDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            ContaReceberBll contaReceberBll = new ContaReceberBll(true);
            contaReceberBll.Incluir(requisicaoDto, ref retorno);

            //Verificar o retorno 
            if (retorno.Retorno == false)
            {
                //Se houver erro, exibir na tela de inclusão
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "INCLUIDO";

            //Retornar para index
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Chama a tela para visualizar uma conta
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
            ContaReceberModel model = new ContaReceberModel();
            string mensagemRetorno = "";

            //Obtem uma conta pelo ID
            if (!this.ObterContaReceber(id, ref model, ref mensagemRetorno))
            {
                ViewBag.Mensagem = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "VISUALIZANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para editar uma conta
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
                ViewBag.MensagemErro = "Para editar uma conta é necessário " +
                    $"logar com um contaReceber administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            ContaReceberModel model = new ContaReceberModel();
            string mensagemRetorno = "";

            //Obtem o conta pelo ID
            if (!this.ObterContaReceber(id, ref model, ref mensagemRetorno))
            {
                ViewBag.Mensagem = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "EDITANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para salvar os dados da conta
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(ContaReceberModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para editar uma conta é necessário " +
                    $"logar com um contaReceber administrador.";
                return View("SemPermissao");
            }

            //Valida a entidade recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Converte para DTO
            ContaReceberDto contaReceberDto = new ContaReceberDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref contaReceberDto, ref mensagemErro))
            {
                ViewBag.Mensagem = mensagemErro;
                return View("Erro");
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<ContaReceberDto> requisicaoDto = new RequisicaoEntidadeDto<ContaReceberDto>()
            {
                EntidadeDto = contaReceberDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            ContaReceberBll contaReceberBll = new ContaReceberBll(true);
            contaReceberBll.Editar(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "ALTERADO";

            //Voltar para o visualizar do conta a receber
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Chama a tela para excluir uma conta
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Excluir(ExclusaoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para excluir um contaReceber é necessário " +
                    $"logar com um contaReceber administrador.";
                return View("SemPermissao");
            }

            TempData["Retorno"] = "EXCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para excluir uma conta
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirContaReceber(ExclusaoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para excluir uma conta é necessário " +
                    $"logar com um contaReceber administrador.";
                return View("SemPermissao");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.Mensagem = "Este usuario não tem permissão para excluir uma conta.";
                return View("Erro");
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = model.Id,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            ContaReceberBll contaReceberBll = new ContaReceberBll(true);
            contaReceberBll.Excluir(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View("Excluir", model);
            }

            TempData["Retorno"] = "EXCLUIDO";

            //Voltar para a index de contaReceber
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Obtem uma conta e converte em Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ObterContaReceber(Guid id, ref ContaReceberModel model, ref string mensagemErro)
        {
            //Preparar a requisição e o retorno
            RetornoObterDto<ContaReceberDto> retorno = new RetornoObterDto<ContaReceberDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            ContaReceberBll contaReceberBll = new ContaReceberBll(true);
            contaReceberBll.Obter(requisicaoDto, ref retorno);

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
        /// Obtem uma listra filtrada de contas a receber
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string ObterListaFiltrada(FiltrosContaReceberModel filtros)
        {
            //Requisição para obter a lista
            RequisicaoObterListaDto requisicaoDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "VENCIMENTO",
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                NaoPaginarPesquisa = filtros.NaoPaginaPesquisa,
                Pagina = filtros.Pagina,
                NumeroItensPorPagina = 20
            };

            //Adicionar filtros utilizados
            if (!string.IsNullOrWhiteSpace(filtros.Descricao))
            {
                requisicaoDto.ListaFiltros.Add("DESCRICAO", filtros.Descricao.Trim());
            }

            if (filtros.Status != StatusConta.NaoIdentificado)
            {
                requisicaoDto.ListaFiltros.Add("STATUS", ((int)filtros.Status).ToString());
            }

            if (filtros.PrecoInicial > 0)
            {
                requisicaoDto.ListaFiltros.Add("PRECOMAIOR", filtros.PrecoInicial.ToString());
            }

            if (filtros.PrecoFinal > 0)
            {
                requisicaoDto.ListaFiltros.Add("PRECOMENOR", filtros.PrecoFinal.ToString());
            }

            requisicaoDto.ListaFiltros.Add("DATAINICIO" + filtros.PesquisarPor, filtros.DataInicio.Date.ToString());
            requisicaoDto.ListaFiltros.Add("DATAFIM" + filtros.PesquisarPor, filtros.DataFim.Date.ToString());

            //Consumir o serviço
            ContaReceberBll contaReceberBll = new ContaReceberBll(true);
            RetornoObterListaDto<ContaReceberDto> retornoDto = new RetornoObterListaDto<ContaReceberDto>();
            contaReceberBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}