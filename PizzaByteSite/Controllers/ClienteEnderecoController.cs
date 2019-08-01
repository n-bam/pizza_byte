using PizzaByteBll;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteSite.Models;
using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PizzaByteSite.Controllers
{
    public class ClienteEnderecoController : Controller
    {
        /// <summary>
        /// Chama a tela para incluir um endereço de cliente
        /// </summary>
        /// <returns></returns>
        public ActionResult Incluir(Guid Id, string nomeCliente)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Cliente a ser incluído
            ClienteEnderecoModel model = new ClienteEnderecoModel()
            {
                Id = Guid.NewGuid(),
                IdCliente = Id,
                NomeCliente = nomeCliente
            };

            TempData["Retorno"] = "INCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para incluir um novo cliente
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Incluir(ClienteEnderecoModel model)
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
            ClienteEnderecoDto clienteEnderecoDto = new ClienteEnderecoDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref clienteEnderecoDto, ref mensagemErro))
            {
                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            clienteEnderecoDto.Id = Guid.NewGuid();

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<ClienteEnderecoDto> requisicaoDto = new RequisicaoEntidadeDto<ClienteEnderecoDto>()
            {
                EntidadeDto = clienteEnderecoDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            ClienteEnderecoBll clienteEnderecoBll = new ClienteEnderecoBll(true);
            clienteEnderecoBll.Incluir(requisicaoDto, ref retorno);

            //Verificar o retorno 
            if (retorno.Retorno == false)
            {
                //Se houver erro, exibir na tela de inclusão
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "INCLUIDO";

            //Retornar para o cliente
            return RedirectToAction("Visualizar", "Cliente", new { id = model.IdCliente });
        }

        /// <summary>
        /// Chama a tela para visualizar um fornecedor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Visualizar(Guid id, string nomeCliente)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Model a ser populada
            ClienteEnderecoModel model = new ClienteEnderecoModel();
            string mensagemRetorno = "";

            //Obtem o fornecedor pelo ID
            if (!this.ObterClienteEndereco(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            model.NomeCliente = nomeCliente;

            TempData["Retorno"] = "VISUALIZANDOENDERECO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para editar um endereço de cliente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Editar(Guid id, string nomeCliente)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Model a ser populada
            ClienteEnderecoModel model = new ClienteEnderecoModel();
            string mensagemRetorno = "";

            //Obtem o cliente pelo ID
            if (!this.ObterClienteEndereco(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            model.NomeCliente = nomeCliente;

            TempData["Retorno"] = "EDITANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para salvar os dados do cliente
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(ClienteEnderecoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Valida a entidade recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Converte para DTO
            ClienteEnderecoDto clienteDto = new ClienteEnderecoDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref clienteDto, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<ClienteEnderecoDto> requisicaoDto = new RequisicaoEntidadeDto<ClienteEnderecoDto>()
            {
                EntidadeDto = clienteDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            ClienteEnderecoBll clienteBll = new ClienteEnderecoBll(true);
            clienteBll.Editar(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "ALTERADO";

            //Retornar para o cliente
            return RedirectToAction("Visualizar", "Cliente", new { id = model.IdCliente });
        }

        /// <summary>
        /// Chama a tela para excluir um endereço de cliente
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
                ViewBag.MensagemErro = "Para excluir um endereço de cliente é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            TempData["Retorno"] = "EXCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para excluir o cliente
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirClienteEndereco(ExclusaoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para excluir um endereço de cliente é necessário " +
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
            ClienteEnderecoBll clienteBll = new ClienteEnderecoBll(true);
            clienteBll.Excluir(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View("Excluir", model);
            }

            TempData["Retorno"] = "EXCLUIDO";

            //Retornar para o cliente
            return RedirectToAction("Visualizar", "Cliente", new { id = model.IdPai });
        }

        /// <summary>
        /// Obtem um endereço de cliente e converte em Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ObterClienteEndereco(Guid id, ref ClienteEnderecoModel model, ref string mensagemErro)
        {
            //Preparar a requisição e o retorno
            RetornoObterDto<ClienteEnderecoDto> retorno = new RetornoObterDto<ClienteEnderecoDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            ClienteEnderecoBll clienteBll = new ClienteEnderecoBll(true);
            clienteBll.Obter(requisicaoDto, ref retorno);

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
        /// Obtem uma listra filtrada de clientes
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string ObterListaEnderecosCliente(Guid idCliente, int pagina = 1)
        {
            RetornoObterListaDto<ClienteEnderecoDto> retornoDto = new RetornoObterListaDto<ClienteEnderecoDto>();

            //Requisição para obter a lista
            RequisicaoObterListaDto requisicaoDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "",
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                NaoPaginarPesquisa = false,
                Pagina = pagina,
                NumeroItensPorPagina = 20
            };

            //Adicionar filtros utilizados
            if (idCliente == Guid.Empty)
            {
                retornoDto.Mensagem = "O id do cliente é obrigatório para obter a lista de endereços";
                retornoDto.Retorno = false;
            }
            else
            {
                requisicaoDto.ListaFiltros.Add("IDCLIENTE", idCliente.ToString());

                //Consumir o serviço
                ClienteEnderecoBll clienteEnderecoBll = new ClienteEnderecoBll(true);
                clienteEnderecoBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);
            }

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

    }
}