using PizzaByteBll;
using PizzaByteBll.Base;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteSite.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PizzaByteSite.Controllers
{
    public class FornecedorController : BaseController
    {
        private LogBll logBll = new LogBll("FornecedorController");

        /// <summary>
        /// Chama a tela com a listagem de fornecedores
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
            FiltrosFornecedorModel model = new FiltrosFornecedorModel()
            {
                Pagina = 1
            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para incluir um fornecedor
        /// </summary>
        /// <returns></returns>
        public ActionResult Incluir()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Fornecedor a ser incluído
            FornecedorModel model = new FornecedorModel()
            {
                Id = Guid.NewGuid(),
            };

            TempData["Retorno"] = "INCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para incluir um novo fornecedor
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Incluir(FornecedorModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            // Validar inclusão do endereço
            Dictionary<string, string> errosEndereco = new Dictionary<string, string>();
            if (!Utilidades.ValidarEndereco(model.Endereco, ref errosEndereco))
            {
                foreach (var erro in errosEndereco)
                {
                    ModelState.AddModelError(erro.Key, erro.Value);
                }
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Converter para DTO
            FornecedorDto fornecedorDto = new FornecedorDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref fornecedorDto, ref mensagemErro))
            {
                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            fornecedorDto.Id = Guid.NewGuid();

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<FornecedorDto> requisicaoDto = new RequisicaoEntidadeDto<FornecedorDto>()
            {
                EntidadeDto = fornecedorDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            FornecedorBll fornecedorBll = new FornecedorBll(true);
            fornecedorBll.Incluir(requisicaoDto, ref retorno);

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
            FornecedorModel model = new FornecedorModel();
            string mensagemRetorno = "";

            //Obtem o fornecedor pelo ID
            if (!this.ObterFornecedor(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "VISUALIZANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para editar um fornecedor
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
                ViewBag.MensagemErro = "Para editar um fornecedor é necessário " +
                    $"logar com um fornecedor administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            FornecedorModel model = new FornecedorModel();
            string mensagemRetorno = "";

            //Obtem o fornecedor pelo ID
            if (!this.ObterFornecedor(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "EDITANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para salvar os dados do fornecedor
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(FornecedorModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para editar um fornecedor é necessário " +
                    $"logar com um fornecedor administrador.";
                return View("SemPermissao");
            }

            //Valida a entidade recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Converte para DTO
            FornecedorDto fornecedorDto = new FornecedorDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref fornecedorDto, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<FornecedorDto> requisicaoDto = new RequisicaoEntidadeDto<FornecedorDto>()
            {
                EntidadeDto = fornecedorDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            FornecedorBll fornecedorBll = new FornecedorBll(true);
            fornecedorBll.Editar(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "ALTERADO";

            //Voltar para o visualizar do fornecedor
            return RedirectToAction("Visualizar", new { id = fornecedorDto.Id });
        }

        /// <summary>
        /// Chama a tela para excluir um fornecedor
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
                ViewBag.MensagemErro = "Para excluir um fornecedor é necessário " +
                    $"logar com um fornecedor administrador.";
                return View("SemPermissao");
            }

            TempData["Retorno"] = "EXCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para excluir o fornecedor
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirFornecedor(ExclusaoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para excluir um fornecedor é necessário " +
                    $"logar com um fornecedor administrador.";
                return View("SemPermissao");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.Mensagem = "Este fornecedor não tem permissão para excluir outros fornecedores.";
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
            FornecedorBll fornecedorBll = new FornecedorBll(true);
            fornecedorBll.Excluir(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View("Excluir", model);
            }

            TempData["Retorno"] = "EXCLUIDO";

            //Voltar para a index de fornecedor
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Obtem um fornecedor e converte em Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ObterFornecedor(Guid id, ref FornecedorModel model, ref string mensagemErro)
        {
            //Preparar a requisição e o retorno
            RetornoObterDto<FornecedorDto> retorno = new RetornoObterDto<FornecedorDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            FornecedorBll fornecedorBll = new FornecedorBll(true);
            fornecedorBll.Obter(requisicaoDto, ref retorno);

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
        /// Obtem uma listra filtrada de fornecedores
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string ObterListaFiltrada(FiltrosFornecedorModel filtros)
        {
            //Requisição para obter a lista
            RequisicaoObterListaDto requisicaoDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "NOMEFANTASIA",
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                NaoPaginarPesquisa = filtros.NaoPaginaPesquisa,
                Pagina = filtros.Pagina,
                NumeroItensPorPagina = 20
            };

            //Adicionar filtros utilizados
            if (!string.IsNullOrWhiteSpace(filtros.NomeFantasia))
            {
                requisicaoDto.ListaFiltros.Add("NOMEFANTASIA", filtros.NomeFantasia.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtros.RazaoSocial))
            {
                requisicaoDto.ListaFiltros.Add("RAZAOSOCIAL", filtros.RazaoSocial.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtros.ObterInativos))
            {
                requisicaoDto.ListaFiltros.Add("INATIVO", filtros.ObterInativos.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtros.Cnpj))
            {
                requisicaoDto.ListaFiltros.Add("CNPJ", filtros.Cnpj.Trim());
            }

            //Consumir o serviço
            FornecedorBll fornecedorBll = new FornecedorBll(true);
            RetornoObterListaDto<FornecedorDto> retornoDto = new RetornoObterListaDto<FornecedorDto>();
            fornecedorBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}
