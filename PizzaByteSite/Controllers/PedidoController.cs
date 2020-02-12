using PizzaByteBll;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteSite.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Controllers
{
    public class PedidoController : Controller
    {
        /// <summary>
        /// Chama a tela com a listagem de pedidos
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
            FiltrosPedidoModel model = new FiltrosPedidoModel()
            {
                Pagina = 1,
                DataInicio = DateTime.Now,
                DataFim = DateTime.Now,
                Tipo = TipoPedido.NaoIdentificado,
                PedidoIfood = ""
            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para incluir um pedido
        /// </summary>
        /// <returns></returns>
        public ActionResult Incluir()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Pedido a ser incluído
            PedidoModel model = new PedidoModel()
            {
                Id = Guid.NewGuid(),
                DiaPromocao = (DateTime.Now.DayOfWeek >= DayOfWeek.Monday && DateTime.Now.DayOfWeek <= DayOfWeek.Thursday)
            };

            // Adicionar o item de promoção sem quantidade
            model.ListaItens.Add(new PedidoItemModel()
            {
                DescricaoProduto = "Brinde da promoção (seg-qui)",
                Quantidade = 0,
                PrecoProduto = 0,
                TipoProduto = TipoProduto.Bebida,
                Id = Guid.Empty,
                IdProduto = Utilidades.RetornaIdProdutoPromocao(),
                IdProdutoComposto = Guid.Empty
            });

            TempData["Retorno"] = "INCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para incluir um novo pedido
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Incluir(PedidoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            ModelState.Where(p => p.Key.Contains("Entrega.ClienteEndereco.")).Select(p => p.Key).ToList().ForEach(p => ModelState.Remove(p));
            ModelState.Where(p => p.Key.Contains("Cliente.")).Select(p => p.Key).ToList().ForEach(p => ModelState.Remove(p));
            ModelState.Where(p => p.Key.Contains("ListaItens")).Select(p => p.Key).ToList().ForEach(p => ModelState.Remove(p));

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string mensagemErro = "";
            if (!PrepararCadastros(ref model, ref mensagemErro))
            {
                ModelState.AddModelError("", mensagemErro);
                return View(model);
            }

            //Converter para DTO
            PedidoDto pedidoDto = new PedidoDto();
            if (!model.ConverterModelParaDto(ref pedidoDto, ref mensagemErro))
            {
                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            pedidoDto.Id = Guid.NewGuid();

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<PedidoDto> requisicaoDto = new RequisicaoEntidadeDto<PedidoDto>()
            {
                EntidadeDto = pedidoDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            PedidoBll pedidoBll = new PedidoBll(true);
            pedidoBll.Incluir(requisicaoDto, ref retorno);

            //Verificar o retorno 
            if (retorno.Retorno == false)
            {
                //Se houver erro, exibir na tela de inclusão
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            model.DataInclusao = DateTime.Now.AddHours(4);
            string impressao = "";

            if (MontarImpressaoPedido(model, ref impressao))
            {
                TempData["Impressao"] = impressao;
            }

            TempData["Retorno"] = "INCLUIDO";

            //Retornar para index
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Chama a tela para visualizar um pedido
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
            PedidoModel model = new PedidoModel();
            string mensagemRetorno = "";

            //Obtem o fornecedor pelo ID
            if (!this.ObterPedido(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            string impressao = "";
            if (MontarImpressaoPedido(model, ref impressao))
            {
                TempData["Impressao"] = impressao;
            }

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Chama a tela para editar um pedido
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
            PedidoModel model = new PedidoModel();
            string mensagemRetorno = "";

            //Obtem o pedido pelo ID
            if (!this.ObterPedido(id, ref model, ref mensagemRetorno))
            {
                ViewBag.MensagemErro = mensagemRetorno;
                return View("Erro");
            }

            TempData["Retorno"] = "EDITANDO";
            model.DiaPromocao = (DateTime.Now.DayOfWeek >= DayOfWeek.Monday && DateTime.Now.DayOfWeek >= DayOfWeek.Thursday);

            // Adicionar o item de promoção sem quantidade
            model.ListaItens.Insert(0, new PedidoItemModel()
            {
                DescricaoProduto = "Brinde da promoção (seg-qui)",
                Quantidade = 0,
                PrecoProduto = 0,
                TipoProduto = TipoProduto.Bebida,
                Id = Guid.Empty,
                IdProduto = Utilidades.RetornaIdProdutoPromocao(),
                IdProdutoComposto = Guid.Empty
            });

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para salvar os dados do pedido
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(PedidoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            ModelState.Where(p => p.Key.Contains("Entrega.ClienteEndereco.")).Select(p => p.Key).ToList().ForEach(p => ModelState.Remove(p));
            ModelState.Where(p => p.Key.Contains("Cliente.")).Select(p => p.Key).ToList().ForEach(p => ModelState.Remove(p));
            ModelState.Where(p => p.Key.Contains("ListaItens")).Select(p => p.Key).ToList().ForEach(p => ModelState.Remove(p));

            //Valida a entidade recebida
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string mensagemErro = "";
            if (!PrepararCadastros(ref model, ref mensagemErro))
            {
                ModelState.AddModelError("", mensagemErro);
                return View(model);
            }

            //Converte para DTO
            PedidoDto pedidoDto = new PedidoDto();
            if (!model.ConverterModelParaDto(ref pedidoDto, ref mensagemErro))
            {
                ViewBag.MensagemErro = mensagemErro;
                return View("Erro");
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoEntidadeDto<PedidoDto> requisicaoDto = new RequisicaoEntidadeDto<PedidoDto>()
            {
                EntidadeDto = pedidoDto,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            PedidoBll pedidoBll = new PedidoBll(true);
            pedidoBll.Editar(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View(model);
            }

            TempData["Retorno"] = "ALTERADO";

            //Voltar para o visualizar do pedido
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Chama a tela para estornar um pedido
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Cancelar(ExclusaoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para estornar um pedido é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            TempData["Retorno"] = "CANCELANDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para estornaro pedido
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelarPedido(ExclusaoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para estornar um pedido é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            if (string.IsNullOrWhiteSpace(model.Justificativa))
            {
                ModelState.AddModelError("", "Preencha uma justificativa para o estorno do pedido.");
                return View("Cancelar", model);
            }

            //Preparar requisição e retorno
            RetornoDto retorno = new RetornoDto();
            RequisicaoCancelarPedidoDto requisicaoDto = new RequisicaoCancelarPedidoDto()
            {
                Id = model.Id,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Justificativa = model.Justificativa
            };

            //Consumir o serviço
            PedidoBll pedidoBll = new PedidoBll(true);
            pedidoBll.CancelarPedido(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View("Cancelar", model);
            }

            TempData["Retorno"] = "ESTORNADO";

            //Voltar para a index de pedido
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Chama a tela para excluir um pedido
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
                ViewBag.MensagemErro = "Para excluir um pedido é necessário " +
                    $"logar com um usuário administrador.";
                return View("SemPermissao");
            }

            TempData["Retorno"] = "EXCLUINDO";

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Consome o serviço para excluir o pedido
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirPedido(ExclusaoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (!SessaoUsuario.SessaoLogin.Administrador)
            {
                ViewBag.MensagemErro = "Para excluir um pedido é necessário " +
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
            PedidoBll pedidoBll = new PedidoBll(true);
            pedidoBll.Excluir(requisicaoDto, ref retorno);

            //Tratar o retorno
            if (retorno.Retorno == false)
            {
                ModelState.AddModelError("", retorno.Mensagem);
                return View("Excluir", model);
            }

            TempData["Retorno"] = "EXCLUIDO";

            //Voltar para a index de pedido
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Obtem um pedido e converte em Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ObterPedido(Guid id, ref PedidoModel model, ref string mensagemErro)
        {
            //Preparar a requisição e o retorno
            RetornoObterDto<PedidoDto> retorno = new RetornoObterDto<PedidoDto>();
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                Id = id,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Consumir o serviço
            PedidoBll pedidoBll = new PedidoBll(true);
            pedidoBll.Obter(requisicaoDto, ref retorno);

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
        /// Obtem uma listra filtrada de pedidos
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string ObterListaFiltrada(FiltrosPedidoModel filtros)
        {
            //Requisição para obter a lista
            RequisicaoObterListaDto requisicaoDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "DATAINCLUSAO",
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                NaoPaginarPesquisa = filtros.NaoPaginaPesquisa,
                Pagina = filtros.Pagina,
                NumeroItensPorPagina = 20
            };

            //Adicionar filtros utilizados
            if (filtros.IdCliente != Guid.Empty && filtros.IdCliente != null)
            {
                requisicaoDto.ListaFiltros.Add("IDCLIENTE", filtros.IdCliente.ToString());
            }

            if (filtros.Tipo != TipoPedido.NaoIdentificado)
            {
                requisicaoDto.ListaFiltros.Add("TIPO", ((int)filtros.Tipo).ToString());
            }

            if (filtros.Total > 0)
            {
                requisicaoDto.ListaFiltros.Add("TOTAL", filtros.Total.ToString());
            }

            if (!string.IsNullOrWhiteSpace(filtros.ObterInativos))
            {
                requisicaoDto.ListaFiltros.Add("INATIVO", filtros.ObterInativos.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filtros.PedidoIfood))
            {
                requisicaoDto.ListaFiltros.Add("PEDIDOIFOOD", filtros.PedidoIfood);
            }

            requisicaoDto.ListaFiltros.Add("DATAINCLUSAOINICIO", filtros.DataInicio.Date.ToString());
            requisicaoDto.ListaFiltros.Add("DATAINCLUSAOFIM", filtros.DataFim.Date.ToString());

            //Consumir o serviço
            PedidoBll pedidoBll = new PedidoBll(true);
            RetornoObterListaDto<PedidoDto> retornoDto = new RetornoObterListaDto<PedidoDto>();
            pedidoBll.ObterListaFiltrada(requisicaoDto, ref retornoDto);

            // o java script converte o fuso horário
            retornoDto.ListaEntidades.ForEach(p => p.DataInclusao = p.DataInclusao.AddHours(-4));

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Obtem uma listra filtrada de pedidos
        /// </summary>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public string ObterPedidoResumido(Guid idPedido)
        {
            //Requisição para obter
            RequisicaoObterDto requisicaoDto = new RequisicaoObterDto()
            {
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario,
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                Id = idPedido
            };

            //Consumir o serviço
            PedidoBll pedidoBll = new PedidoBll(true);
            RetornoObterDto<PedidoResumidoDto> retornoDto = new RetornoObterDto<PedidoResumidoDto>();
            pedidoBll.ObterPedidoResumido(requisicaoDto, ref retornoDto);

            string retorno = new JavaScriptSerializer().Serialize(retornoDto);
            return retorno;
        }

        /// <summary>
        /// Valida e prepara os cadastros para inclusão/edição de um pedido
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool PrepararCadastros(ref PedidoModel model, ref string mensagemErro)
        {
            if (model.ListaItens.Where(p => p.Quantidade > 0).ToList().Count <= 0)
            {
                mensagemErro = "Informe ao menos um item para o pedido";
                return false;
            }

            model.NomeCliente = string.IsNullOrWhiteSpace(model.Cliente.Nome) ? "Não identificado" : model.Cliente.Nome;

            try
            {
                // Se os dados do cliente forem preenchidos, criar um novo ID
                if (model.Cliente.Id == Guid.Empty && !string.IsNullOrWhiteSpace(model.Cliente.Nome))
                {
                    model.Cliente.Id = Guid.NewGuid();
                    model.IdCliente = model.Cliente.Id;
                    model.Entrega.ClienteEndereco.IdCliente = model.Cliente.Id;
                }
                else if (model.Cliente.Id != null && model.Cliente.Id != Guid.Empty)
                {
                    model.IdCliente = model.Cliente.Id;
                    model.Entrega.ClienteEndereco.IdCliente = model.Cliente.Id;
                }

                // Se os dados de endereço
                if (model.Entrega.ClienteEndereco.Id == Guid.Empty && !string.IsNullOrWhiteSpace(model.Entrega.ClienteEndereco.NumeroEndereco))
                {
                    model.Entrega.ClienteEndereco.Id = Guid.NewGuid();
                    model.Entrega.ClienteEndereco.IdCep = (model.Entrega.ClienteEndereco.Endereco.Id == Guid.Empty || model.Entrega.ClienteEndereco.Endereco.Id == null) ? Guid.NewGuid() : model.Entrega.ClienteEndereco.Endereco.Id;
                    model.Entrega.ClienteEndereco.Endereco.Id = model.Entrega.ClienteEndereco.IdCep;
                    model.Entrega.IdEndereco = model.Entrega.ClienteEndereco.Id;
                }
                else
                {
                    model.Entrega.ClienteEndereco.IdCep = (model.Entrega.ClienteEndereco.Endereco.Id == Guid.Empty || model.Entrega.ClienteEndereco.Endereco.Id == null) ? Guid.NewGuid() : model.Entrega.ClienteEndereco.Endereco.Id;
                    model.Entrega.IdEndereco = model.Entrega.ClienteEndereco.Id;
                }

                model.Entrega.Id = (model.Entrega.Id == null || model.Entrega.Id == Guid.Empty) ? Guid.NewGuid() : model.Entrega.Id;
            }
            catch (Exception ex)
            {
                mensagemErro = "Falha ao preparar os cadastros: " + ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retorna o texto html a ser impresso
        /// </summary>
        /// <param name="model"></param>
        /// <param name="retorno"></param>
        /// <returns></returns>
        private bool MontarImpressaoPedido(PedidoModel model, ref string retorno)
        {
            retorno = (model.Tipo == TipoPedido.Balcão) ? "Balcão" : ((model.Tipo == TipoPedido.Entrega) ? "Entrega" : "Retirada");

            retorno = $"<center><h4>PizzaByte - Pedido {model.DataInclusao.ToString("dd/MM/yyyy HH:mm:ss")} ({retorno})</h4></center>";
            retorno += (model.PedidoIfood) ? $"<b>Pedido via IFood</b><br/ >" : "";
            retorno += $"<b>Cliente:</b> {(string.IsNullOrWhiteSpace(model.Cliente.Nome) ? "" : model.Cliente.Nome.Replace("'", "")) }";
            retorno += $"<br /> <b>Telefone:</b> {model.Cliente.Telefone}";

            if (model.Tipo == TipoPedido.Entrega)
            {
                retorno += $"<br /> <b>Dados para entrega:</b>" +
                $" {(string.IsNullOrWhiteSpace(model.Entrega.ClienteEndereco.Endereco.Logradouro) ? "" : model.Entrega.ClienteEndereco.Endereco.Logradouro.Replace("'", ""))}, ";
                retorno += $"{model.Entrega.ClienteEndereco.NumeroEndereco} - {model.Entrega.ClienteEndereco.Endereco.Bairro}/";
                retorno += $"{(string.IsNullOrWhiteSpace(model.Entrega.ClienteEndereco.Endereco.Cidade) ? "" : model.Entrega.ClienteEndereco.Endereco.Cidade.Replace("'", ""))}, " +
                    $"{(string.IsNullOrWhiteSpace(model.Entrega.ClienteEndereco.ComplementoEndereco) ? "" : model.Entrega.ClienteEndereco.ComplementoEndereco.Replace("'", ""))}";
                retorno += $"<br /><br /><b>Taxa:</b> R${String.Format("{0:0.00}", model.TaxaEntrega)}";
            }

            retorno += $"<br /><center><h4>Itens pedidos</center></h4>";
            foreach (var item in model.ListaItens)
            {
                if (item.Quantidade > 0)
                {
                    string descricao = string.IsNullOrWhiteSpace(item.DescricaoProduto) ? "": item.DescricaoProduto.Replace("/", "<br/>");

                    retorno += $"<b>Descrição:</b><br/> {descricao}";
                    retorno += $"<br /> <b>Quantidade:</b> {item.Quantidade}";
                    retorno += $"<br /> <b>Valor:</b> R${String.Format("{0:0.00}", item.PrecoProduto)} <br /><br /> ";
                }
            }

            if (!string.IsNullOrWhiteSpace(model.Obs))
            {
                retorno += $"<b>Observações:</b> {model.Obs.Trim()} <br />";
            }

            retorno += $"<br /> <b>Total:</b> R${String.Format("{0:0.00}", model.Total)}";
            retorno += $"<center><h4>Pagamento</center></h4>";
            retorno += (model.RecebidoDinheiro > 0) ? $"<b>Dinheiro:</b> R${String.Format("{0:0.00}", model.RecebidoDinheiro)} <br /> " : "";
            retorno += (model.RecebidoDebito > 0) ? $"<b>Débito:</b> R${String.Format("{0:0.00}", model.RecebidoDebito)} <br /> " : "";
            retorno += (model.RecebidoCredito > 0) ? $"<b>Crédito:</b> R${String.Format("{0:0.00}", model.RecebidoCredito)} <br /> " : "";
            retorno += (model.Troco > 0) ? $"<b>Troco:</b> R${String.Format("{0:0.00}", model.Troco)}" : "";

            return true;
        }
    }
}