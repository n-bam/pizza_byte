using PizzaByteBll;
using PizzaByteDto.ClassesBase;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteSite.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PizzaByteSite.Controllers
{
    public class TaxaEntregaController : Controller
    {
        /// <summary>
        /// Chama a tela com a listagem de taxas de entrega
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
            FiltrosTaxaEntregaModel model = new FiltrosTaxaEntregaModel()
            {
                Pagina = 1
            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para incluir/alterar uma lista de taxas de entrega 
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfigurarTaxas()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Taxas a serem incluídas
            List<TaxaEntregaModel> model = new List<TaxaEntregaModel>();
            TaxaEntregaBll taxaEntregaBll = new TaxaEntregaBll(false);
            BaseRequisicaoDto requisicaoDto = new BaseRequisicaoDto()
            {
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            // Obter a lista com bairros e taxas
            RetornoObterListaDto<TaxaEntregaDto> retornoDto = new RetornoObterListaDto<TaxaEntregaDto>();
            if (!taxaEntregaBll.ObterListaBairrosComTaxa(requisicaoDto, ref retornoDto))
            {
                ViewBag.MensagemErro = retornoDto.Mensagem;
                return View("Erro");
            }

            // Converter e add a lista da model
            string mensagemErro = "";
            foreach (var tax in retornoDto.ListaEntidades)
            {
                TaxaEntregaModel taxaModel = new TaxaEntregaModel();
                if (!taxaModel.ConverterDtoParaModel(tax, ref mensagemErro))
                {
                    ViewBag.MensagemErro = mensagemErro;
                    return View("Erro");
                }

                model.Add(taxaModel);
            }

            TempData["Retorno"] = "INCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para incluir um nova taxa de entrega 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfigurarTaxas(List<TaxaEntregaModel> model)
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

            string mensagemErro = "";
            List<TaxaEntregaDto> listaDtos = new List<TaxaEntregaDto>();

            //Converter para DTO
            foreach (var taxa in model)
            {
                TaxaEntregaDto produtoDto = new TaxaEntregaDto();
                if (!taxa.ConverterModelParaDto(ref produtoDto, ref mensagemErro))
                {
                    ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                    return View(model);
                }

                listaDtos.Add(produtoDto);
            }
            
            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoListaEntidadesDto<TaxaEntregaDto> requisicaoDto = new RequisicaoListaEntidadesDto<TaxaEntregaDto>()
            {
                ListaEntidadesDto = listaDtos,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            TaxaEntregaBll produtoBll = new TaxaEntregaBll(true);
            produtoBll.IncluirEditarLista(requisicaoDto, ref retorno);

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
        /// Chama a tela para editar uma taxa de entrega 
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

            //Model a ser populada
            TaxaEntregaModel model = new TaxaEntregaModel();
            string mensagemRetorno = "";

            //Obtem a taxa de entrega  pelo ID
            if (!this.ObterTaxaEntrega(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "EDITANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para salvar os dados da taxa de entrega 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(TaxaEntregaModel model)
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
            TaxaEntregaDto produtoDto = new TaxaEntregaDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref produtoDto, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<TaxaEntregaDto> requisicaoDto = new RequisicaoEntidadeDto<TaxaEntregaDto>()
            {
                EntidadeDto = produtoDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            TaxaEntregaBll produtoBll = new TaxaEntregaBll(true);
            produtoBll.Editar(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "ALTERADO";

            //Voltar para o visualizar da taxa de entrega 
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Obtem uma taxa de entrega  e converte em Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ObterTaxaEntrega(Guid id, ref TaxaEntregaModel model, ref string mensagemErro)
        {
            //Preparar a requisição e o retorno
            RetornoObterDto<TaxaEntregaDto> retorno = new RetornoObterDto<TaxaEntregaDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            TaxaEntregaBll produtoBll = new TaxaEntregaBll(true);
            produtoBll.Obter(requisicaoDto, ref retorno);

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
        /// Obtem uma listra filtrada de taxas de entrega
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string ObterListaFiltrada(FiltrosTaxaEntregaModel filtros)
        {
            //Requisição para obter a lista
            RequisicaoObterListaDto requisicaoDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = string.IsNullOrWhiteSpace(filtros.CampoOrdenacao) ? "" : filtros.CampoOrdenacao.Trim(),
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                NaoPaginarPesquisa = filtros.NaoPaginaPesquisa,
                Pagina = filtros.Pagina,
                NumeroItensPorPagina = 20
            };

            //Adicionar filtros utilizados
            if (!string.IsNullOrWhiteSpace(filtros.Bairro))
            {
                requisicaoDto.ListaFiltros.Add("BAIRRO", filtros.Bairro.Trim());
            }

            if (filtros.TaxaInicial > 0)
            {
                requisicaoDto.ListaFiltros.Add("VALORTAXAMAIOR", filtros.TaxaInicial.ToString());
            }

            if (filtros.TaxaFinal > 0)
            {
                requisicaoDto.ListaFiltros.Add("VALORTAXAMENOR", filtros.TaxaFinal.ToString());
            }

            if (!string.IsNullOrWhiteSpace(filtros.ObterInativos))
            {
                requisicaoDto.ListaFiltros.Add("INATIVO", filtros.ObterInativos.Trim());
            }

            //Consumir o serviço
            TaxaEntregaBll produtoBll = new TaxaEntregaBll(true);
            RetornoObterListaDto<TaxaEntregaDto> retornoDto = new RetornoObterListaDto<TaxaEntregaDto>();
            produtoBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

    }
}