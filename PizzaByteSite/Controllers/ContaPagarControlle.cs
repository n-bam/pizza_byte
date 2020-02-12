using PizzaByteBll;
using PizzaByteBll.Base;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteSite.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Controllers
{
    public class ContaPagarController : BaseController
    {
        private LogBll logBll = new LogBll("ContaPagarController");

        /// <summary>
        /// Chama a tela com a listagem de conta pagar
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
            FiltrosContaPagarModel model = new FiltrosContaPagarModel()
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
        /// Chama a tela para incluir uma conta a pagar
        /// </summary>
        /// <returns></returns>
        public ActionResult Incluir()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }
            
            //conta a ser incluída
            ContaPagarModel model = new ContaPagarModel()
            {
                Id = Guid.NewGuid(),
                DataCompetencia = DateTime.Now,
                DataVencimento = DateTime.Now
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
        public ActionResult Incluir(ContaPagarModel model)
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
            ContaPagarDto contaPagarDto = new ContaPagarDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref contaPagarDto, ref mensagemErro))
            {
                ModelState.AddModelError("Servico", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            contaPagarDto.Id = Guid.NewGuid();

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<ContaPagarDto> requisicaoDto = new RequisicaoEntidadeDto<ContaPagarDto>()
            {
                EntidadeDto = contaPagarDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            ContaPagarBll ContaPagarBll = new ContaPagarBll(true);
            ContaPagarBll.Incluir(requisicaoDto, ref retorno);

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
        /// Chama a tela para visualizar da conta
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
            ContaPagarModel model = new ContaPagarModel();
            string mensagemRetorno = "";

            //Obtem o contaPagar pelo ID
            if (!this.ObterContaPagar(id, ref model, ref mensagemRetorno))
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
                    $"logar com um contaPagar administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            ContaPagarModel model = new ContaPagarModel();
            string mensagemRetorno = "";

            //Obtem o contaPagar pelo ID
            if (!this.ObterContaPagar(id, ref model, ref mensagemRetorno))
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
        public ActionResult Editar(ContaPagarModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para editar uma conta é necessário " +
                    $"logar com um contaPagar administrador.";
                return View("SemPermissao");
            }

            //Valida a entidade recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Converte para DTO
            ContaPagarDto contaPagarDto = new ContaPagarDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref contaPagarDto, ref mensagemErro))
            {
                ViewBag.Mensagem = mensagemErro;
                return View("Erro");
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<ContaPagarDto> requisicaoDto = new RequisicaoEntidadeDto<ContaPagarDto>()
            {
                EntidadeDto = contaPagarDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            ContaPagarBll contaPagarBll = new ContaPagarBll(true);
            contaPagarBll.Editar(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "ALTERADO";

            //Voltar para o visualizar uma conta
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
                ViewBag.MensagemErro = "Para excluir um contaPagar é necessário " +
                    $"logar com um contaPagar administrador.";
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
        public ActionResult ExcluirContaPagar(ExclusaoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para excluir uma conta é necessário " +
                    $"logar com um contaPagar administrador.";
                return View("SemPermissao");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.Mensagem = "Esta usuario não tem permissão para excluir uma conta.";
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
            ContaPagarBll contaPagarBll = new ContaPagarBll(true);
            contaPagarBll.Excluir(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View("Excluir", model);
            }

            TempData["Retorno"] = "EXCLUIDO";

            //Voltar para a index de contas a pagar
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Obtem uma conta e converte em Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ObterContaPagar(Guid id, ref ContaPagarModel model, ref string mensagemErro)
        {
            //Preparar a requisição e o retorno
            RetornoObterDto<ContaPagarDto> retorno = new RetornoObterDto<ContaPagarDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            ContaPagarBll contaPagarBll = new ContaPagarBll(true);
            contaPagarBll.Obter(requisicaoDto, ref retorno);

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
        /// Obtem uma listra filtrada de contas a pagar
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string ObterListaFiltrada(FiltrosContaPagarModel filtros)
        {
            //Requisição para obter a lista
            RequisicaoObterListaDto requisicaoDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "DESCRIÇÂO",
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                NaoPaginarPesquisa = filtros.NaoPaginaPesquisa,
                Pagina = filtros.Pagina,
                NumeroItensPorPagina = 20
            };

            //Adicionar filtros utilizados
            if (!string.IsNullOrWhiteSpace(filtros.Descricao))
            {
                requisicaoDto.ListaFiltros.Add("DESCRIÇÂO", filtros.Descricao.Trim());
            }

            if (filtros.Status!= StatusConta.NaoIdentificado)
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
            ContaPagarBll contaPagarBll = new ContaPagarBll(true);
            RetornoObterListaDto<ContaPagarDto> retornoDto = new RetornoObterListaDto<ContaPagarDto>();
            contaPagarBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}