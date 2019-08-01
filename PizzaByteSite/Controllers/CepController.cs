using PizzaByteBll;
using PizzaByteDto.Base;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteSite.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PizzaByteSite.Controllers
{
    public class CepController : BaseController
    {
        /// <summary>
        /// Chama a tela com a listagem de ceps
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
            FiltrosCepModel model = new FiltrosCepModel()
            {
                Pagina = 1
            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para incluir um cep
        /// </summary>
        /// <returns></returns>
        public ActionResult Incluir()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Cep a ser incluído
            CepModel model = new CepModel()
            {
                Id = Guid.NewGuid()
            };

            TempData["Retorno"] = "INCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para incluir um novo cep
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Incluir(CepModel model)
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
            CepDto cepDto = new CepDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref cepDto, ref mensagemErro))
            {
                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            cepDto.Id = Guid.NewGuid();

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<CepDto> requisicaoDto = new RequisicaoEntidadeDto<CepDto>()
            {
                EntidadeDto = cepDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            CepBll cepBll = new CepBll(true);
            cepBll.Incluir(requisicaoDto, ref retorno);

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
        /// Chama a tela para editar um cep
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
            CepModel model = new CepModel();
            string mensagemRetorno = "";

            //Obtem o cep pelo ID
            if (!this.ObterCep(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "EDITANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para salvar os dados do cep
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(CepModel model)
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
            CepDto cepDto = new CepDto();
            string mensagemErro = "";
            if (!model.ConverterModelParaDto(ref cepDto, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<CepDto> requisicaoDto = new RequisicaoEntidadeDto<CepDto>()
            {
                EntidadeDto = cepDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            CepBll cepBll = new CepBll(true);
            cepBll.Editar(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "ALTERADO";

            //Voltar para o visualizar do cep
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Chama a tela para excluir um cep
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

            TempData["Retorno"] = "EXCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para excluir o cep
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirCep(ExclusaoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
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
            CepBll cepBll = new CepBll(true);
            cepBll.Excluir(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View("Excluir", model);
            }

            TempData["Retorno"] = "EXCLUIDO";

            //Voltar para a index de cep
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Obtem um cep e converte em Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ObterCep(Guid id, ref CepModel model, ref string mensagemErro)
        {
            //Preparar a requisição e o retorno
            RetornoObterDto<CepDto> retorno = new RetornoObterDto<CepDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            CepBll cepBll = new CepBll(true);
            cepBll.Obter(requisicaoDto, ref retorno);

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
        /// Obtem uma listra filtrada de ceps
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string ObterListaFiltrada(FiltrosCepModel filtros)
        {
            //Requisição para obter a lista
            RequisicaoObterListaDto requisicaoDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "CEP",
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                NaoPaginarPesquisa = filtros.NaoPaginaPesquisa,
                Pagina = filtros.Pagina,
                NumeroItensPorPagina = 20
            };

            //Adicionar filtros utilizados
            if (!string.IsNullOrWhiteSpace(filtros.Cep))
            {
                requisicaoDto.ListaFiltros.Add("CEP", filtros.Cep.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtros.Logradouro))
            {
                requisicaoDto.ListaFiltros.Add("LOGRADOURO", filtros.Logradouro.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtros.ObterInativos))
            {
                requisicaoDto.ListaFiltros.Add("INATIVO", filtros.ObterInativos.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtros.Bairro))
            {
                requisicaoDto.ListaFiltros.Add("BAIRRO", filtros.Bairro.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtros.Cidade))
            {
                requisicaoDto.ListaFiltros.Add("CIDADE", filtros.Cidade.Trim());
            }

            //Consumir o serviço
            CepBll cepBll = new CepBll(true);
            RetornoObterListaDto<CepDto> retornoDto = new RetornoObterListaDto<CepDto>();
            cepBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Obtem os dados de um endereço pelo CEP
        /// </summary>
        /// <param name="cep"></param>
        /// <returns></returns>
        [HttpGet]
        public string ObterPorCep(string cep)
        {
            RequisicaoObterCepPorCepDto requisicaoDto = new RequisicaoObterCepPorCepDto()
            {
                Cep = cep,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao
            };

            CepBll cepBll = new CepBll(true);
            RetornoObterDto<CepDto> retornoDto = new RetornoObterDto<CepDto>();
            cepBll.ObterPorCep(requisicaoDto, ref retornoDto);

            // Se não encontrar nada no banco, pesquisar online
            if (retornoDto.Entidade == null)
            {
                HttpClient client = new HttpClient()
                {
                    BaseAddress = new Uri("https://viacep.com.br/ws/")
                };

                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync($"{cep}/json").Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        retornoDto.Entidade = response.Content.ReadAsAsync<CepDto>().Result;
                        if (string.IsNullOrWhiteSpace(retornoDto.Entidade.Logradouro))
                        {
                            retornoDto.Entidade = null;
                        }
                        else
                        {
                            // Compatibilizar os campos
                            retornoDto.Entidade.Cidade = retornoDto.Entidade.Localidade;
                        }
                    }
                    catch (Exception)
                    {
                        retornoDto.Entidade = null;
                    }
                }
            }

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Obtém uma lista de endereços partindo do logradouro
        /// </summary>
        /// <param name="logradouro"></param>
        /// <param name="bairro"></param>
        /// <returns></returns>
        [HttpGet]
        public string ObterListaCepPorLogradouro(string logradouro, string cidade)
        {
            //Requisição para obter a lista
            RequisicaoObterListaDto requisicaoDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "LOGRADOURO",
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                NaoPaginarPesquisa = false,
                Pagina = 1,
                NumeroItensPorPagina = 5
            };

            if (!string.IsNullOrWhiteSpace(cidade))
            {
                requisicaoDto.ListaFiltros.Add("CIDADE", cidade.Trim());
            }

            if (!string.IsNullOrWhiteSpace(logradouro))
            {
                requisicaoDto.ListaFiltros.Add("LOGRADOURO", logradouro.Trim());
            }

            requisicaoDto.ListaFiltros.Add("INATIVO", "false");
            CepBll cepBll = new CepBll(true);
            RetornoObterListaDto<CepDto> retornoDto = new RetornoObterListaDto<CepDto>();
            cepBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            // Se não encontrar nada no banco, pesquisar online
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                HttpClient client = new HttpClient()
                {
                    BaseAddress = new Uri("https://viacep.com.br/ws/")
                };

                cidade = string.IsNullOrWhiteSpace(cidade) ? "Americana" : cidade.Trim();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync($"SP/{cidade}/{logradouro.Replace(" ", "+")}/json").Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        retornoDto.ListaEntidades = response.Content.ReadAsAsync<List<CepDto>>().Result;
                        retornoDto.ListaEntidades.ForEach(o => o.Cidade = o.Localidade);
                    }
                    catch (Exception)
                    {
                        retornoDto.ListaEntidades = new List<CepDto>();
                    }
                }
            }

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }
    }
}
