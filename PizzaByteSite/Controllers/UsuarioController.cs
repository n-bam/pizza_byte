﻿using PizzaByteBll;
using PizzaByteBll.Base;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteSite.Models;
using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PizzaByteSite.Controllers
{
    public class UsuarioController : BaseController
    {
        /// <summary>
        /// Chama a tela com a listagem de usuarios
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para consultar os usuários é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            // Filtros da página inicial
            FiltrosUsuarioModel model = new FiltrosUsuarioModel()
            {
                Pagina = 1
            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama o painel inicial do site
        /// </summary>
        /// <returns></returns>
        public ActionResult Inicio()
        {
            // Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario", new { retorno = "/Usuario/Inicio" });
            }

            return View();
        }

        /// <summary>
        /// Chama a tela para incluir um usuario
        /// </summary>
        /// <returns></returns>
        public ActionResult Incluir()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para incluir um usuário é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Usuario a ser incluído
            UsuarioModel model = new UsuarioModel()
            {
                Id = Guid.NewGuid()
            };

            TempData["Retorno"] = "INCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para incluir um novo usuario
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Incluir(UsuarioModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para incluir um usuário é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Checar se a senha foi preenchida
            if (string.IsNullOrWhiteSpace(model.Senha))
            {
                ModelState.AddModelError("Senha", "A senha é obrigatória para incluir o usuário");
                return View(model);
            }

            //Converter para DTO
            UsuarioDto usuarioDto = new UsuarioDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref usuarioDto, ref mensagemErro))
            {
                ModelState.AddModelError("Servico", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            string senhaCriptografada = "";
            if (!string.IsNullOrWhiteSpace(model.Senha))
            {
                UtilitarioBll.CriptografarSenha(model.Senha, ref senhaCriptografada);
            }

            usuarioDto.Senha = senhaCriptografada;
            usuarioDto.Id = Guid.NewGuid();

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<UsuarioDto> requisicaoDto = new RequisicaoEntidadeDto<UsuarioDto>()
            {
                EntidadeDto = usuarioDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            UsuarioBll usuarioBll = new UsuarioBll(true);
            usuarioBll.Incluir(requisicaoDto, ref retorno);

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
        /// Chama a tela para visualizar um usuario
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

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para visualizar um usuário é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            UsuarioModel model = new UsuarioModel();
            string mensagemRetorno = "";

            //Obtem o usuario pelo ID
            if (!this.ObterUsuario(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "VISUALIZANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para editar um usuario
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
                ViewBag.MensagemErro = "Para editar um usuário é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Model a ser populada
            UsuarioModel model = new UsuarioModel();
            string mensagemRetorno = "";

            //Obtem o usuario pelo ID
            if (!this.ObterUsuario(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "EDITANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para salvar os dados do usuario
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(UsuarioModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para editar um usuário é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            //Valida a entidade recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string senhaCriptografada = "";
            if (!string.IsNullOrWhiteSpace(model.Senha))
            {
                UtilitarioBll.CriptografarSenha(model.Senha, ref senhaCriptografada);
            }

            if (string.IsNullOrWhiteSpace(model.SenhaAntiga))
            {
                ModelState.AddModelError("", "Para alterar os dados do usuário informa a senha atual.");
                return View(model);
            }

            string senhaAntigaCriptografada = "";
            UtilitarioBll.CriptografarSenha(model.SenhaAntiga, ref senhaAntigaCriptografada);

            //Converte para DTO
            UsuarioDto usuarioDto = new UsuarioDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref usuarioDto, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            usuarioDto.SenhaAntiga = senhaAntigaCriptografada;
            if (string.IsNullOrWhiteSpace(senhaCriptografada))
            {
                usuarioDto.Senha = senhaAntigaCriptografada;
            }
            else
            {
                usuarioDto.Senha = senhaCriptografada;
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<UsuarioDto> requisicaoDto = new RequisicaoEntidadeDto<UsuarioDto>()
            {
                EntidadeDto = usuarioDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            UsuarioBll usuarioBll = new UsuarioBll(true);
            usuarioBll.Editar(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "ALTERADO";

            //Voltar para o visualizar do usuario
            return RedirectToAction("Visualizar", new { id = usuarioDto.Id });
        }

        /// <summary>
        /// Chama a tela para excluir um usuario
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
                ViewBag.MensagemErro = "Para excluir um usuário é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            TempData["Retorno"] = "EXCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para excluir o usuario
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirUsuario(ExclusaoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para excluir um usuário é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
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
            UsuarioBll usuarioBll = new UsuarioBll(true);
            usuarioBll.Excluir(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View("Excluir", model);
            }

            TempData["Retorno"] = "EXCLUIDO";

            //Voltar para a index de usuario
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Chama a tela para preencher email e senha
        /// </summary>
        /// <returns></returns>
        public ActionResult Login(string retorno = "")
        {
            //Email e senha a serem preenchidos
            LoginModel model = new LoginModel()
            {
                EnderecoRetorno = retorno
            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para fazer o login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string senhaCriptografada = "";
            UtilitarioBll.CriptografarSenha(model.SenhaLogin, ref senhaCriptografada);

            model.SenhaLogin = senhaCriptografada;

            //Converter para DTO
            string mensagemErro = "";
            RequisicaoFazerLoginDto requisicaoFazerLoginDto = new RequisicaoFazerLoginDto();
            if (!model.ConverterModelParaDto(ref requisicaoFazerLoginDto, ref mensagemErro))
            {
                ModelState.AddModelError("", mensagemErro);
                return View(model);
            }

            //Preparar requisição e retorno
            RetornoFazerLoginDto retorno = new RetornoFazerLoginDto();

            //Consumir o serviço
            UsuarioBll usuarioBll = new UsuarioBll(true);
            usuarioBll.FazerLogin(requisicaoFazerLoginDto, ref retorno);

            //Verificar o retorno 
            if (retorno.Retorno == false)
            {
                //Se houver erro, exibir na tela de inclusão
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            //Guardar na sessão
            SessaoUsuario.SessaoLogin.Identificacao = retorno.Identificacao;
            SessaoUsuario.SessaoLogin.IdUsuario = retorno.IdUsuario;
            SessaoUsuario.SessaoLogin.NomeUsuario = retorno.NomeUsuario;
            SessaoUsuario.SessaoLogin.Administrador = retorno.UsuarioAdministrador;

            //Retornar para index do site
            if (string.IsNullOrWhiteSpace(model.EnderecoRetorno))
            {
                return Redirect("Inicio");
            }
            else
            {
                return Redirect(model.EnderecoRetorno);
            }
        }

        /// <summary>
        /// Chama a tela para recuperar a senha
        /// </summary>
        /// <returns></returns>
        public ActionResult RecuperarSenha()
        {
            //Email e senha a serem preenchidos
            LoginModel model = new LoginModel();

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Envia um email com uma nova senha
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarSenha(LoginModel model)
        {
            //Converter para DTO
            string mensagemErro = "";
            RequisicaoFazerLoginDto requisicaoFazerLoginDto = new RequisicaoFazerLoginDto();
            if (!model.ConverterModelParaDto(ref requisicaoFazerLoginDto, ref mensagemErro))
            {
                ModelState.AddModelError("", mensagemErro);
                return View(model);
            }

            //Preparar requisição e retorno
            RetornoFazerLoginDto retorno = new RetornoFazerLoginDto();

            //Consumir o serviço
            UsuarioBll usuarioBll = new UsuarioBll(true);
            usuarioBll.EnviarEmailRecuperacao(requisicaoFazerLoginDto, retorno);

            if (retorno.Retorno == false)
            {
                ViewBag.MensagemErro = retorno.Mensagem;
                return View("Erro");
            }

            TempData["Retorno"] = "SENHAALTERADA";
          
            //Chamar a view
            return RedirectToAction("Login", "Usuario");
        }

        /// <summary>
        /// Limpa a sessão e abre a tela de login
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            SessaoUsuario.SessaoLogin.Identificacao = "";
            SessaoUsuario.SessaoLogin.NomeUsuario = "";
            SessaoUsuario.SessaoLogin.IdUsuario = Guid.Empty;
            SessaoUsuario.SessaoLogin.Administrador = false;

            //Email e senha a serem preenchidos
            LoginModel model = new LoginModel();

            //Chamar a view
            return RedirectToAction("Login", model);
        }

        /// <summary>
        /// Obtem um usuario e converte em Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ObterUsuario(Guid id, ref UsuarioModel model, ref string mensagemErro)
        {
            //Preparar a requisição e o retorno
            RetornoObterDto<UsuarioDto> retorno = new RetornoObterDto<UsuarioDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            UsuarioBll usuarioBll = new UsuarioBll(true);
            usuarioBll.Obter(requisicaoDto, ref retorno);

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
        /// Obtem uma listra filtrada de usuários
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        [HttpPost]
        public string ObterListaFiltrada(FiltrosUsuarioModel filtros)
        {
            //Requisição para obter a lista
            RequisicaoObterListaDto requisicaoDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "NOME",
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                NaoPaginarPesquisa = filtros.NaoPaginaPesquisa,
                Pagina = filtros.Pagina,
                NumeroItensPorPagina = 20
            };

            //Adicionar filtros utilizados
            if (!string.IsNullOrWhiteSpace(filtros.Email))
            {
                requisicaoDto.ListaFiltros.Add("EMAIL", filtros.Email.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtros.Nome))
            {
                requisicaoDto.ListaFiltros.Add("NOME", filtros.Nome.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtros.ObterInativos))
            {
                requisicaoDto.ListaFiltros.Add("INATIVO", filtros.ObterInativos.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtros.ObterAdministrador))
            {
                requisicaoDto.ListaFiltros.Add("ADMINISTRADOR", filtros.ObterAdministrador.Trim());
            }

            //Consumir o serviço
            UsuarioBll usuarioBll = new UsuarioBll(true);
            RetornoObterListaDto<UsuarioDto> retornoDto = new RetornoObterListaDto<UsuarioDto>();
            usuarioBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}
