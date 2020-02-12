using Microsoft.Reporting.WebForms;
using PizzaByteBll;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteSite.Models;
using System;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteSite.Controllers
{
    public class RelatoriosListagemController : Controller
    {
        /// <summary>
        /// Chama a tela com os filtros do relatório
        /// </summary>
        /// <returns></returns>
        public ActionResult ListagemClientes()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Cliente a ser incluído
            FiltrosListagemClienteModel model = new FiltrosListagemClienteModel()
            {
                CampoOrdem = "NOME",
                Cpf = "",
                DataCadastroFinal = null,
                DataCadastroInicial = null,
                Nome = "",
                ObterInativos = "false",
                Telefone = ""
            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListagemClientes(FiltrosListagemClienteModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoCliente();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                return View(model);
            }

            string mensagemErro = "";
            RequisicaoObterListagemClienteDto requisicaoDto = new RequisicaoObterListagemClienteDto()
            {
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Converter para DTO
            if (!model.ConverterModelParaDto(ref requisicaoDto, ref mensagemErro))
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoCliente();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<ClienteDto> retornoDto = new RetornoObterListaDto<ClienteDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterListagemClientes(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoCliente();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View(model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoCliente();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View(model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!string.IsNullOrWhiteSpace(model.Nome))
            {
                filtros = $"Cliente que tenham o nome '{model.Nome.Trim()}' \n";
            }

            if (!string.IsNullOrWhiteSpace(model.Telefone))
            {
                filtros += $"Cliente que tenham o telefone '{model.Telefone.Trim()}' \n";
            }

            if (!string.IsNullOrWhiteSpace(model.Cpf))
            {
                filtros += $"Cliente que tenham o CPF '{model.Cpf.Trim()}' \n";
            }

            if (!string.IsNullOrWhiteSpace(model.ObterInativos))
            {
                filtros += $"Apenas clientes { (model.ObterInativos.Trim() == "true" ? "inativos" : "ativos") } \n";
            }

            if (model.DataCadastroInicial.HasValue)
            {
                filtros += $"Cadastrados após '{ model.DataCadastroInicial.Value.ToString("dd/MM/yyyy") }' ";

                if (model.DataCadastroFinal.HasValue)
                {
                    filtros += $"e até '{ model.DataCadastroFinal.Value.ToString("dd/MM/yyyy") }' ";
                }
            }

            if (model.DataCadastroFinal.HasValue && !model.DataCadastroInicial.HasValue)
            {
                filtros += $"Cadastrados até '{ model.DataCadastroFinal.Value.ToString("dd/MM/yyyy") }' ";
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);

                //Relatório
                ReportViewer viewer = new ReportViewer()
                {
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(100),
                    ShowPrintButton = false,
                    ShowRefreshButton = false,
                    ShowBackButton = false
                };

                viewer.LocalReport.ReportPath = "Relatorios/ListagemClienteRv.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.DisplayName = "Listagem de clientes";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosListagem",
                    ViewOrigem = "ListagemClientes",
                    NomeRelatorio = "Listagem de clientes",
                    Relatorio = viewer
                };

                //Retornar para index
                return View("EmissaoRelatorio", modelRelatorio);
            }
            catch (Exception ex)
            {
                ViewBag.MensagemErro = "Problemas ao passar os dados para o relatório: " + ex.Message;
                return View("Erro");
            }
        }

        /// <summary>
        /// Chama a tela com os filtros do relatório
        /// </summary>
        /// <returns></returns>
        public ActionResult ListagemFornecedores()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Cliente a ser incluído
            FiltrosListagemFornecedorModel model = new FiltrosListagemFornecedorModel()
            {
                CampoOrdem = "NOMEFANTASIA",
                Cnpj = "",
                DataCadastroFinal = null,
                DataCadastroInicial = null,
                NomeFantasia = "",
                RazaoSocial = "",
                ObterInativos = "false",
                Telefone = ""
            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListagemFornecedores(FiltrosListagemFornecedorModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoFornecedores();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                return View(model);
            }

            string mensagemErro = "";
            RequisicaoObterListagemFornecedoresDto requisicaoDto = new RequisicaoObterListagemFornecedoresDto()
            {
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Converter para DTO
            if (!model.ConverterModelParaDto(ref requisicaoDto, ref mensagemErro))
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoFornecedores();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<FornecedorDto> retornoDto = new RetornoObterListaDto<FornecedorDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterListagemFornecedores(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoFornecedores();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View(model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoFornecedores();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View(model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";

            if (!string.IsNullOrWhiteSpace(model.NomeFantasia))
            {
                filtros = $"Fornecedor que tenham o nome '{model.NomeFantasia.Trim()}' \n";
            }

            if (!string.IsNullOrWhiteSpace(model.RazaoSocial))
            {
                filtros = $"Fornecedor que tenham a razão '{model.RazaoSocial.Trim()}' \n";
            }

            if (!string.IsNullOrWhiteSpace(model.Telefone))
            {
                filtros += $"Fornecedor que tenham o telefone '{model.Telefone.Trim()}' \n";
            }

            if (!string.IsNullOrWhiteSpace(model.Cnpj))
            {
                filtros += $"Fornecedor que tenham o Cnpj '{model.Cnpj.Trim()}' \n";
            }

            if (!string.IsNullOrWhiteSpace(model.ObterInativos))
            {
                filtros += $"Apenas clientes { (model.ObterInativos.Trim() == "true" ? "inativos" : "ativos") } \n";
            }

            if (model.DataCadastroInicial.HasValue)
            {
                filtros += $"Cadastrados após '{ model.DataCadastroInicial.Value.ToString("dd/MM/yyyy") }' ";

                if (model.DataCadastroFinal.HasValue)
                {
                    filtros += $"e até '{ model.DataCadastroFinal.Value.ToString("dd/MM/yyyy") }' ";
                }
            }

            if (model.DataCadastroFinal.HasValue && !model.DataCadastroInicial.HasValue)
            {
                filtros += $"Cadastrados até '{ model.DataCadastroFinal.Value.ToString("dd/MM/yyyy") }' ";
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);

                //Relatório
                ReportViewer viewer = new ReportViewer()
                {
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(100),
                    ShowPrintButton = false,
                    ShowRefreshButton = false,
                    ShowBackButton = false
                };

                viewer.LocalReport.ReportPath = "Relatorios/ListagemFornecedorRv.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.DisplayName = "Listagem de fornecedores";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosListagem",
                    ViewOrigem = "ListagemFornecedores",
                    NomeRelatorio = "Listagem de fornecedores",
                    Relatorio = viewer
                };

                //Retornar para index
                return View("EmissaoRelatorio", modelRelatorio);
            }
            catch (Exception ex)
            {
                ViewBag.MensagemErro = "Problemas ao passar os dados para o relatório: " + ex.Message;
                return View("Erro");
            }
        }

        /// <summary>
        /// Chama a tela com os filtros do relatório de produtos
        /// </summary>
        /// <returns></returns>
        public ActionResult ListagemContaPagar()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            FiltrosListagemContaPagarModel model = new FiltrosListagemContaPagarModel()
            {
                CampoOrdem = "DATAVENCIMENTO",
                DataCadastroFinal = DateTime.Now,
                DataCadastroInicial = DateTime.Now,
                PrecoInicio = 0,
                PrecoFim = 0,
                PesquisarPor = "DATAVENCIMENTO",
                Descricao = "",
                Status = StatusConta.NaoIdentificado
            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListagemContaPagar(FiltrosListagemContaPagarModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                model.ListaStatus = Utilidades.RetornarListaStatusConta();
                model.ListaStatus.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaCampoOrdem = Utilidades.RetornarListaStatusConta();
                return View(model);
            }

            string mensagemErro = "";
            RequisicaoObterListagemContaPagarDto requisicaoDto = new RequisicaoObterListagemContaPagarDto()
            {
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Converter para DTO
            if (!model.ConverterModelParaDto(ref requisicaoDto, ref mensagemErro))
            {
                model.ListaStatus = Utilidades.RetornarListaStatusConta();
                model.ListaStatus.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaCampoOrdem = Utilidades.RetornarListaStatusConta();

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<ContaPagarDto> retornoDto = new RetornoObterListaDto<ContaPagarDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterListagemContaPagar(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaStatus = Utilidades.RetornarListaStatusConta();
                model.ListaStatus.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaCampoOrdem = Utilidades.RetornarListaStatusConta();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View(model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaStatus = Utilidades.RetornarListaStatusConta();
                model.ListaStatus.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaCampoOrdem = Utilidades.RetornarListaStatusConta();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View(model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!Utilidades.RetornarFiltroPreenchidosContas(model, ref filtros))
            {
                ViewBag.MensagemErro = "Problemas ao preencher os filtro";
                return View("Erro");
            }

            if (model.IdFornecedor != null && model.IdFornecedor != Guid.Empty)
            {
                filtros += $"Contas a pagar do fornecedor: {model.NomeFornecedor}. \n";
            }

            string titulo = "";
            if (model.PesquisarPor.Trim() == "COMPETENCIA")
            {
                titulo = "competência";
            }
            else if (model.PesquisarPor.Trim() == "VENCIMENTO")
            {
                titulo = "vencimento";
            }
            else
            {
                titulo = "pagamento";
            }

            titulo = $"Contas a pagar com {titulo} de {model.DataCadastroInicial.Value.ToString("dd/MM/yyyy")} até {model.DataCadastroFinal.Value.ToString("dd/MM/yyyy")}. ";

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);
                ReportParameter tituloCompleto = new ReportParameter("Titulo", titulo);

                //Relatório
                ReportViewer viewer = new ReportViewer()
                {
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(100),
                    ShowPrintButton = false,
                    ShowRefreshButton = false,
                    ShowBackButton = false
                };

                viewer.LocalReport.ReportPath = "Relatorios/ListagemContaPagarRv.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.SetParameters(tituloCompleto);
                viewer.LocalReport.DisplayName = "Listagem de contas a pagar";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosListagem",
                    ViewOrigem = "ListagemContaPagar",
                    NomeRelatorio = "Listagem de contas a pagar",
                    Relatorio = viewer
                };

                //Retornar para index
                return View("EmissaoRelatorio", modelRelatorio);
            }
            catch (Exception ex)
            {
                ViewBag.MensagemErro = "Problemas ao passar os dados para o relatório: " + ex.Message;
                return View("Erro");
            }
        }


        /// <summary>
        /// Chama a tela com os filtros do relatório de produtos
        /// </summary>
        /// <returns></returns>
        public ActionResult ListagemContaReceber()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            FiltrosListagemContaReceberModel model = new FiltrosListagemContaReceberModel()
            {
                CampoOrdem = "DATAVENCIMENTO",
                DataCadastroFinal = DateTime.Now,
                DataCadastroInicial = DateTime.Now,
                PrecoInicio = 0,
                PrecoFim = 0,
                PesquisarPor = "DATAVENCIMENTO",
                Descricao = "",
                Status = StatusConta.NaoIdentificado
            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListagemContaReceber(FiltrosListagemContaReceberModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                model.ListaStatus = Utilidades.RetornarListaStatusConta();
                model.ListaStatus.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaCampoOrdem = Utilidades.RetornarListaStatusConta();
                return View(model);
            }

            string mensagemErro = "";
            RequisicaoObterListagemContaReceberDto requisicaoDto = new RequisicaoObterListagemContaReceberDto()
            {
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Converter para DTO
            if (!model.ConverterModelParaDto(ref requisicaoDto, ref mensagemErro))
            {
                model.ListaStatus = Utilidades.RetornarListaStatusConta();
                model.ListaStatus.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaCampoOrdem = Utilidades.RetornarListaStatusConta();

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<ContaReceberDto> retornoDto = new RetornoObterListaDto<ContaReceberDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterListagemContaReceber(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaStatus = Utilidades.RetornarListaStatusConta();
                model.ListaStatus.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaCampoOrdem = Utilidades.RetornarListaStatusConta();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View(model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaStatus = Utilidades.RetornarListaStatusConta();
                model.ListaStatus.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaCampoOrdem = Utilidades.RetornarListaStatusConta();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View(model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!Utilidades.RetornarFiltroPreenchidosContas(model, ref filtros))
            {
                ViewBag.MensagemErro = "Problemas ao preencher os filtro";
                return View("Erro");
            }

            string titulo = "";
            if (model.PesquisarPor.Trim() == "COMPETENCIA")
            {
                titulo = "competência";
            }
            else if (model.PesquisarPor.Trim() == "VENCIMENTO")
            {
                titulo = "vencimento";
            }

            titulo = $"Contas a receber com {titulo} de {model.DataCadastroInicial.Value.ToString("dd/MM/yyyy")} até {model.DataCadastroFinal.Value.ToString("dd/MM/yyyy")}. ";

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);
                ReportParameter tituloCompleto = new ReportParameter("Titulo", titulo);

                //Relatório
                ReportViewer viewer = new ReportViewer()
                {
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(100),
                    ShowPrintButton = false,
                    ShowRefreshButton = false,
                    ShowBackButton = false
                };

                viewer.LocalReport.ReportPath = "Relatorios/ListagemContaReceberRv.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.SetParameters(tituloCompleto);
                viewer.LocalReport.DisplayName = "Listagem de contas a receber";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosListagem",
                    ViewOrigem = "ListagemContaReceber",
                    NomeRelatorio = "Listagem de contas a receber",
                    Relatorio = viewer
                };

                //Retornar para index
                return View("EmissaoRelatorio", modelRelatorio);
            }
            catch (Exception ex)
            {
                ViewBag.MensagemErro = "Problemas ao passar os dados para o relatório: " + ex.Message;
                return View("Erro");
            }
        }


        /// <summary>
        /// Chama a tela com os filtros do relatório
        /// </summary>
        /// <returns></returns>
        public ActionResult ListagemProdutos()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Cliente a ser incluído
            FiltrosListagemProdutoModel model = new FiltrosListagemProdutoModel()
            {
                CampoOrdem = "DESCRICAO",
                PrecoInicio = 0,
                PrecoFim = 0,
                DataCadastroFinal = null,
                DataCadastroInicial = null,
                Descricao = "",
                ObterInativos = "false",

            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListagemProdutos(FiltrosListagemProdutoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoProduto();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                return View(model);
            }

            string mensagemErro = "";
            RequisicaoObterListagemProdutoDto requisicaoDto = new RequisicaoObterListagemProdutoDto()
            {
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Converter para DTO
            if (!model.ConverterModelParaDto(ref requisicaoDto, ref mensagemErro))
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoProduto();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<ProdutoDto> retornoDto = new RetornoObterListaDto<ProdutoDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterListagemProdutos(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoProduto();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View(model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoProduto();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View(model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!string.IsNullOrWhiteSpace(model.Descricao))
            {
                filtros = $"Produtos que tenham '{model.Descricao.Trim()}' na descrição \n";
            }

            if (model.PrecoInicio > 0)
            {
                filtros += $"Produto que tenha o preço maior que R${String.Format("{0:0.00}", model.PrecoInicio)}' \n";
            }

            if (model.PrecoFim > 0)
            {
                filtros += $"Produto que tenha o preço menor que R${String.Format("{0:0.00}", model.PrecoFim)}' \n";
            }

            if (model.Tipo != TipoProduto.NaoIdentificado)
            {
                filtros += $"Produto que tenha o tipo '{model.Tipo}' \n";
            }

            if (!string.IsNullOrWhiteSpace(model.ObterInativos))
            {
                filtros += $"Apenas produtos { (model.ObterInativos.Trim() == "true" ? "inativos" : "ativos") } \n";
            }

            if (model.DataCadastroInicial.HasValue)
            {
                filtros += $"Cadastrados após '{ model.DataCadastroInicial.Value.ToString("dd/MM/yyyy") }' ";

                if (model.DataCadastroFinal.HasValue)
                {
                    filtros += $"e até '{ model.DataCadastroFinal.Value.ToString("dd/MM/yyyy") }' ";
                }
            }

            if (model.DataCadastroFinal.HasValue && !model.DataCadastroInicial.HasValue)
            {
                filtros += $"Cadastrados até '{ model.DataCadastroFinal.Value.ToString("dd/MM/yyyy") }' ";
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);

                //Relatório
                ReportViewer viewer = new ReportViewer()
                {
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(100),
                    ShowPrintButton = false,
                    ShowRefreshButton = false,
                    ShowBackButton = false
                };

                viewer.LocalReport.ReportPath = "Relatorios/ListagemProdutoRv.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.DisplayName = "Listagem de produtos";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosListagem",
                    ViewOrigem = "ListagemProdutos",
                    NomeRelatorio = "Listagem de produtos",
                    Relatorio = viewer
                };

                //Retornar para index
                return View("EmissaoRelatorio", modelRelatorio);
            }
            catch (Exception ex)
            {
                ViewBag.MensagemErro = "Problemas ao passar os dados para o relatório: " + ex.Message;
                return View("Erro");
            }
        }


        /// <summary>
        /// Chama a tela com os filtros do relatório
        /// </summary>
        /// <returns></returns>
        public ActionResult ListagemTaxaEntrega()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //taxa a ser incluído
            FiltrosListagemTaxaEntregaModel model = new FiltrosListagemTaxaEntregaModel()
            {
                CampoOrdem = "BairroCidade",
                ValorInicio = 0,
                ValorFim = 0,
                DataCadastroFinal = null,
                DataCadastroInicial = null,

            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListagemTaxaEntrega(FiltrosListagemTaxaEntregaModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoTaxaEntrega();

                return View(model);
            }

            string mensagemErro = "";
            RequisicaoObterListagemTaxaEntregaDto requisicaoDto = new RequisicaoObterListagemTaxaEntregaDto()
            {
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Converter para DTO
            if (!model.ConverterModelParaDto(ref requisicaoDto, ref mensagemErro))
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoTaxaEntrega();

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<TaxaEntregaDto> retornoDto = new RetornoObterListaDto<TaxaEntregaDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterListagemTaxaEntrega(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoTaxaEntrega();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View(model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoTaxaEntrega();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View(model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!string.IsNullOrWhiteSpace(model.BairroCidade))
            {
                filtros = $"Bairro que tenham '{ model.BairroCidade.Trim()}' no nome \n";
            }

            if (model.ValorInicio > 0)
            {
                filtros += $"Taxa de entrega que tenha o valor inicial R${String.Format("{0:0.00}", model.ValorInicio)} \n";
            }

            if (model.ValorFim > 0)
            {
                filtros += $"Taxa de entrega que tenha o valor final R${String.Format("{0:0.00}", model.ValorFim)} \n";
            }

            if (model.DataCadastroInicial.HasValue)
            {
                filtros += $"Cadastrados após '{ model.DataCadastroInicial.Value.ToString("dd/MM/yyyy") }' ";

                if (model.DataCadastroFinal.HasValue)
                {
                    filtros += $"e até '{ model.DataCadastroFinal.Value.ToString("dd/MM/yyyy") }' ";
                }
            }

            if (model.DataCadastroFinal.HasValue && !model.DataCadastroInicial.HasValue)
            {
                filtros += $"Cadastrados até '{ model.DataCadastroFinal.Value.ToString("dd/MM/yyyy") }' ";
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);

                //Relatório
                ReportViewer viewer = new ReportViewer()
                {
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(100),
                    ShowPrintButton = false,
                    ShowRefreshButton = false,
                    ShowBackButton = false
                };

                viewer.LocalReport.ReportPath = "Relatorios/ListagemTaxaEntregaRv.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.DisplayName = "Listagem de taxas de entregas";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosListagem",
                    ViewOrigem = "ListagemTaxaEntrega",
                    NomeRelatorio = "Listagem de taxas de entrega",
                    Relatorio = viewer
                };

                //Retornar para index
                return View("EmissaoRelatorio", modelRelatorio);
            }
            catch (Exception ex)
            {
                ViewBag.MensagemErro = "Problemas ao passar os dados para o relatório: " + ex.Message;
                return View("Erro");
            }
        }


        /// <summary>
        /// Chama a tela com os filtros do relatório
        /// </summary>
        /// <returns></returns>
        public ActionResult ListagemEntregas()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Cliente a ser incluído
            FiltrosListagemEntregaModel model = new FiltrosListagemEntregaModel()
            {
                CampoOrdem = "DataInclusao",
                Endereco = "",
                DataCadastroFinal = null,
                DataCadastroInicial = null,
                ValorInicio = 0,
                ValorFim = 0,
                Funcionario = "",

            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListagemEntregas(FiltrosListagemEntregaModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoEntrega();

                return View(model);
            }

            string mensagemErro = "";
            RequisicaoObterListagemEntregasDto requisicaoDto = new RequisicaoObterListagemEntregasDto()
            {
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Converter para DTO
            if (!model.ConverterModelParaDto(ref requisicaoDto, ref mensagemErro))
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoEntrega();

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<ListagemEntregaDto> retornoDto = new RetornoObterListaDto<ListagemEntregaDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterListagemEntregas(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoEntrega();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View(model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoEntrega();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View(model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!string.IsNullOrWhiteSpace(model.Endereco))
            {
                filtros = $"Entrega que tenha o endereço '{model.Endereco.Trim()}' \n";
            }

            if (!string.IsNullOrWhiteSpace(model.Funcionario))
            {
                filtros += $"Entrega que tenha o funcionario '{model.Funcionario.Trim()}' \n";
            }

            if (model.DataCadastroInicial.HasValue)
            {
                filtros += $"Pedidos feitos após '{ model.DataCadastroInicial.Value.ToString("dd/MM/yyyy") }' ";

                if (model.DataCadastroFinal.HasValue)
                {
                    filtros += $"e até '{ model.DataCadastroFinal.Value.ToString("dd/MM/yyyy") }' ";
                }
            }

            if (model.DataCadastroFinal.HasValue && !model.DataCadastroInicial.HasValue)
            {
                filtros += $"Pedidos feitos até '{ model.DataCadastroFinal.Value.ToString("dd/MM/yyyy") }' ";
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);

                //Relatório
                ReportViewer viewer = new ReportViewer()
                {
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(100),
                    ShowPrintButton = false,
                    ShowRefreshButton = false,
                    ShowBackButton = false
                };

                viewer.LocalReport.ReportPath = "Relatorios/ListagemEntregasRv.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.DisplayName = "Listagem de entregas";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosListagem",
                    ViewOrigem = "ListagemEntregas",
                    NomeRelatorio = "Listagem de entregas",
                    Relatorio = viewer
                };

                //Retornar para index
                return View("EmissaoRelatorio", modelRelatorio);
            }
            catch (Exception ex)
            {
                ViewBag.MensagemErro = "Problemas ao passar os dados para o relatório: " + ex.Message;
                return View("Erro");
            }
        }


        /// <summary>
        /// Chama a tela com os filtros do relatório de produtos
        /// </summary>
        /// <returns></returns>
        public ActionResult ListagemPedidosResumida()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            FiltrosListagemPedidoModel model = new FiltrosListagemPedidoModel()
            {
                CampoOrdem = "DATA",
                TaxaEntregaFinal = 0,
                TaxaEntregaInicial = 0,
                IdCliente = null,
                TotalFinal = 0,
                TrocoInicial = 0,
                TrocoFinal = 0,
                TotalInicial = 0,
                DataCadastroFinal = DateTime.Now,
                DataCadastroInicial = DateTime.Now,
                JustificativaCancelamento = "",
                Obs = "",
                ObterInativos = "false",
                Tipo = TipoPedido.NaoIdentificado
            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListagemPedidosResumida(FiltrosListagemPedidoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                model.ListaTipos = Utilidades.RetornarListaTiposPedido();
                model.ListaTipos.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();
                model.ListaInativo[1].Text = "Apenas não cancelados";
                model.ListaInativo[2].Text = "Apenas cancelados";
                model.ListaOpcoesIFood = Utilidades.RetornarListaOpcaoIFood();
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoPedido();
                return View(model);
            }

            string mensagemErro = "";
            RequisicaoObterListagemPedidosDto requisicaoDto = new RequisicaoObterListagemPedidosDto()
            {
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Converter para DTO
            if (!model.ConverterModelParaDto(ref requisicaoDto, ref mensagemErro))
            {
                model.ListaTipos = Utilidades.RetornarListaTiposPedido();
                model.ListaTipos.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();
                model.ListaInativo[1].Text = "Apenas não cancelados";
                model.ListaInativo[2].Text = "Apenas cancelados";
                model.ListaOpcoesIFood = Utilidades.RetornarListaOpcaoIFood();
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoPedido();

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<PedidoResumidoDto> retornoDto = new RetornoObterListaDto<PedidoResumidoDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterListaResumidaPedidos(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaTipos = Utilidades.RetornarListaTiposPedido();
                model.ListaTipos.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();
                model.ListaInativo[1].Text = "Apenas não cancelados";
                model.ListaInativo[2].Text = "Apenas cancelados";
                model.ListaOpcoesIFood = Utilidades.RetornarListaOpcaoIFood();
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoPedido();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View(model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaTipos = Utilidades.RetornarListaTiposPedido();
                model.ListaTipos.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();
                model.ListaInativo[1].Text = "Apenas não cancelados";
                model.ListaInativo[2].Text = "Apenas cancelados";
                model.ListaOpcoesIFood = Utilidades.RetornarListaOpcaoIFood();
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoPedido();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View(model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!Utilidades.RetornarFiltroPreenchidosPedido(model, ref filtros))
            {
                ViewBag.MensagemErro = "Problemas ao preencher os filtro";
                return View("Erro");
            }

            string titulo = $"{model.DataCadastroInicial.Value.ToString("dd/MM/yyyy")} até {model.DataCadastroFinal.Value.ToString("dd/MM/yyyy")}. ";

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);
                ReportParameter tituloRelatorio = new ReportParameter("Titulo", titulo);

                //Relatório
                ReportViewer viewer = new ReportViewer()
                {
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(100),
                    ShowPrintButton = false,
                    ShowRefreshButton = false,
                    ShowBackButton = false
                };

                viewer.LocalReport.ReportPath = "Relatorios/ListagemPedidoResumoRv.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.SetParameters(tituloRelatorio);
                viewer.LocalReport.DisplayName = "Listagem de pedidos (resumido)";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosListagem",
                    ViewOrigem = "ListagemPedidosResumida",
                    NomeRelatorio = "Listagem de pedidos (resumido)",
                    Relatorio = viewer
                };

                //Retornar para index
                return View("EmissaoRelatorio", modelRelatorio);
            }
            catch (Exception ex)
            {
                ViewBag.MensagemErro = "Problemas ao passar os dados para o relatório: " + ex.Message;
                return View("Erro");
            }
        }


        /// <summary>
        /// Chama a tela com os filtros do relatório de produtos
        /// </summary>
        /// <returns></returns>
        public ActionResult ListagemPedidosDetalhada()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Produto a ser incluído
            FiltrosListagemPedidoModel model = new FiltrosListagemPedidoModel()
            {
                CampoOrdem = "DATA",
                TaxaEntregaFinal = 0,
                TaxaEntregaInicial = 0,
                IdCliente = null,
                TotalFinal = 0,
                TrocoInicial = 0,
                TrocoFinal = 0,
                TotalInicial = 0,
                DataCadastroFinal = DateTime.Now,
                DataCadastroInicial = DateTime.Now,
                JustificativaCancelamento = "",
                Obs = "",
                ObterInativos = "false",
                Tipo = TipoPedido.NaoIdentificado
            };

            //Chamar a view
            return View(model);
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListagemPedidosDetalhada(FiltrosListagemPedidoModel model)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                model.ListaTipos = Utilidades.RetornarListaTiposPedido();
                model.ListaTipos.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();
                model.ListaInativo[1].Text = "Apenas não cancelados";
                model.ListaInativo[2].Text = "Apenas cancelados";
                model.ListaOpcoesIFood = Utilidades.RetornarListaOpcaoIFood();
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoPedido();
                return View(model);
            }

            string mensagemErro = "";
            RequisicaoObterListagemPedidosDto requisicaoDto = new RequisicaoObterListagemPedidosDto()
            {
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Converter para DTO
            if (!model.ConverterModelParaDto(ref requisicaoDto, ref mensagemErro))
            {
                model.ListaTipos = Utilidades.RetornarListaTiposPedido();
                model.ListaTipos.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();
                model.ListaInativo[1].Text = "Apenas não cancelados";
                model.ListaInativo[2].Text = "Apenas cancelados";
                model.ListaOpcoesIFood = Utilidades.RetornarListaOpcaoIFood();
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoPedido();

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<PedidoDetalhadoDto> retornoDto = new RetornoObterListaDto<PedidoDetalhadoDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterListaDetalhadaPedidos(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaTipos = Utilidades.RetornarListaTiposPedido();
                model.ListaTipos.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();
                model.ListaInativo[1].Text = "Apenas não cancelados";
                model.ListaInativo[2].Text = "Apenas cancelados";
                model.ListaOpcoesIFood = Utilidades.RetornarListaOpcaoIFood();
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoPedido();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View(model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaTipos = Utilidades.RetornarListaTiposPedido();
                model.ListaTipos.Insert(0, new SelectListItem()
                {
                    Selected = true,
                    Text = "Todos",
                    Value = "0"
                });

                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();
                model.ListaInativo[1].Text = "Apenas não cancelados";
                model.ListaInativo[2].Text = "Apenas cancelados";
                model.ListaOpcoesIFood = Utilidades.RetornarListaOpcaoIFood();
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoPedido();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View(model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!Utilidades.RetornarFiltroPreenchidosPedido(model, ref filtros))
            {
                ViewBag.MensagemErro = "Problemas ao preencher os filtro";
                return View("Erro");
            }

            string titulo = $"{model.DataCadastroInicial.Value.ToString("dd/MM/yyyy")} até {model.DataCadastroFinal.Value.ToString("dd/MM/yyyy")}. ";

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);
                ReportParameter tituloRelatorio = new ReportParameter("Titulo", titulo);

                //Relatório
                ReportViewer viewer = new ReportViewer()
                {
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(100),
                    ShowPrintButton = false,
                    ShowRefreshButton = false,
                    ShowBackButton = false
                };

                viewer.LocalReport.ReportPath = "Relatorios/ListagemPedidoDetalheRv.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.SetParameters(tituloRelatorio);
                viewer.LocalReport.DisplayName = "Listagem de pedidos (datalhado)";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosListagem",
                    ViewOrigem = "ListagemPedidosDetalhada",
                    NomeRelatorio = "Listagem de pedidos (datalhado)",
                    Relatorio = viewer
                };

                //Retornar para index
                return View("EmissaoRelatorio", modelRelatorio);
            }
            catch (Exception ex)
            {
                ViewBag.MensagemErro = "Problemas ao passar os dados para o relatório: " + ex.Message;
                return View("Erro");
            }
        }

    }
}