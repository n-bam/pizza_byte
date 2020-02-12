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
    public class RelatoriosAnaliticosController : Controller
    {
        /// <summary>
        /// Chama a tela com os filtros do relatório
        /// </summary>
        /// <returns></returns>
        public ActionResult MelhoresProdutos()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Cliente a ser incluído
            FiltrosMelhoresProdutosModel model = new FiltrosMelhoresProdutosModel()
            {
                CampoOrdem = "QUANTIDADE",
                DataCadastroInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                DataCadastroFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)),
                Tipo = TipoProduto.NaoIdentificado
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
        public ActionResult MelhoresProdutos(FiltrosMelhoresProdutosModel model)
        {
            return ObterDadosMelhoresProdutos(model, "MelhoresProdutosRv");
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MelhoresProdutosGrafico(FiltrosMelhoresProdutosModel model)
        {
            return ObterDadosMelhoresProdutos(model, "GraficoMelhoresProdutosRv");
        }

        /// <summary>
        /// Obtem os dados e chama o relatório de melhores produtos
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mascaraRelatorio"></param>
        /// <returns></returns>
        private ActionResult ObterDadosMelhoresProdutos(FiltrosMelhoresProdutosModel model, string mascaraRelatorio)
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

                return View("MelhoresProdutos", model);
            }


            string mensagemErro = "";
            RequisicaoObterMelhoresProdutosDto requisicaoDto = new RequisicaoObterMelhoresProdutosDto()
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
                return View("MelhoresProdutos", model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<MelhoresCadastrosDto> retornoDto = new RetornoObterListaDto<MelhoresCadastrosDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterMelhoresProdutos(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoCliente();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View("MelhoresProdutos", model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoCliente();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View("MelhoresProdutos", model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (model.Tipo != TipoProduto.NaoIdentificado)
            {
                filtros = $"Apenas produtos do tipo ";
                switch (model.Tipo)
                {

                    case TipoProduto.Pizza:
                        filtros += "Pizza \n";
                        break;

                    case TipoProduto.Bebida:
                        filtros += "Bebida \n";
                        break;
                }
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);
                ReportParameter periodo = new ReportParameter("Periodo", $"{model.DataCadastroInicial.Value.Date.ToString("dd/MM/yyyy")} até {model.DataCadastroFinal.Value.Date.ToString("dd/MM/yyyy")}");

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

                viewer.LocalReport.ReportPath = $"Relatorios/{mascaraRelatorio}.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.SetParameters(periodo);
                viewer.LocalReport.DisplayName = "Melhores produtos";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosAnaliticos",
                    ViewOrigem = "MelhoresProdutos",
                    NomeRelatorio = "Melhores produtos",
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
        public ActionResult MelhoresClientes()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Cliente a ser incluído
            FiltrosMelhoresClientesModel model = new FiltrosMelhoresClientesModel()
            {
                CampoOrdem = "NOME",
                DataCadastroInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                DataCadastroFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)),
                Nome = "",
                ObterInativos = "false",
                Telefone = "",

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
        public ActionResult MelhoresClientes(FiltrosMelhoresClientesModel model)
        {
            return ObterDadosMelhoresClientes(model, "MelhoresClientesRv");
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MelhoresClientesGrafico(FiltrosMelhoresClientesModel model)
        {
            return ObterDadosMelhoresClientes(model, "GraficoMelhoresClientesRv");
        }

        /// <summary>
        /// Obtem os dados e chama o relatório de melhores clientes
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mascaraRelatorio"></param>
        /// <returns></returns>
        private ActionResult ObterDadosMelhoresClientes(FiltrosMelhoresClientesModel model, string mascaraRelatorio)
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

                return View("MelhoresClientes", model);
            }


            string mensagemErro = "";
            RequisicaoObterMelhoresClientesDto requisicaoDto = new RequisicaoObterMelhoresClientesDto()
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
                return View("MelhoresClientes", model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<MelhoresCadastrosDto> retornoDto = new RetornoObterListaDto<MelhoresCadastrosDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterMelhoresClientes(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoCliente();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View("MelhoresClientes", model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoCliente();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View("MelhoresClientes", model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";

            if (!string.IsNullOrWhiteSpace(model.Nome))
            {
                filtros = $"Cliente que tenham o nome '{model.Nome.Trim()}' \n";
            }

            if (!string.IsNullOrWhiteSpace(model.Telefone))
            {
                filtros = $"Cliente que tenham o telefone '{model.Telefone.Trim()}' \n";
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);
                ReportParameter periodo = new ReportParameter("Periodo", $"{model.DataCadastroInicial.Value.Date.ToString("dd/MM/yyyy")} até {model.DataCadastroFinal.Value.Date.ToString("dd/MM/yyyy")}");

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

                viewer.LocalReport.ReportPath = $"Relatorios/{mascaraRelatorio}.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.SetParameters(periodo);
                viewer.LocalReport.DisplayName = "Melhores clientes";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosAnaliticos",
                    ViewOrigem = "MelhoresClientes",
                    NomeRelatorio = "Melhores clientes",
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
        public ActionResult RelacaoDiariaContas()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Cliente a ser incluído
            FiltrosRelacaoContasModel model = new FiltrosRelacaoContasModel()
            {
                PesquisarPor = "DATAVENCIMENTO",
                DataCadastroInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                DataCadastroFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)),
                IndicadorEstornadas = false,
                IndicadorPerdida = false
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
        public ActionResult RelacaoDiariaContas(FiltrosRelacaoContasModel model)
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
                    Text = "Todos",
                    Value = "0"
                });
                model.ListaOpcoesPesquisa = Utilidades.RetornarOpcoesPesquisaContas();

                return View(model);
            }

            string mensagemErro = "";
            RequisicaoObterRelacaoContasDto requisicaoDto = new RequisicaoObterRelacaoContasDto()
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
                    Text = "Todos",
                    Value = "0"
                });
                model.ListaOpcoesPesquisa = Utilidades.RetornarOpcoesPesquisaContas();

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View(model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<RelacaoContasDto> retornoDto = new RetornoObterListaDto<RelacaoContasDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterRelacaoDiariaContas(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaStatus = Utilidades.RetornarListaStatusConta();
                model.ListaStatus.Insert(0, new SelectListItem()
                {
                    Text = "Todos",
                    Value = "0"
                });
                model.ListaOpcoesPesquisa = Utilidades.RetornarOpcoesPesquisaContas();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View(model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaStatus = Utilidades.RetornarListaStatusConta();
                model.ListaStatus.Insert(0, new SelectListItem()
                {
                    Text = "Todos",
                    Value = "0"
                });
                model.ListaOpcoesPesquisa = Utilidades.RetornarOpcoesPesquisaContas();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View(model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!string.IsNullOrWhiteSpace(model.PesquisarPor))
            {
                filtros = $"Contas por '{model.PesquisarPor.ToLower().Replace("data", "")}' \n";
            }

            if (!model.IndicadorEstornadas)
            {
                filtros += $"Apenas contas não estornadas \n";
            }

            if (!model.IndicadorPerdida)
            {
                filtros += $"Apenas contas com status diferente de 'perdida' \n";
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);
                ReportParameter periodo = new ReportParameter("Periodo", $"{model.DataCadastroInicial.Value.ToString("dd/MM/yyyy")} até {model.DataCadastroFinal.Value.ToString("dd/MM/yyyy")}.");

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

                viewer.LocalReport.ReportPath = "Relatorios/RelacaoContasPagarReceberRv.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.SetParameters(periodo);
                viewer.LocalReport.DisplayName = "Relação diária de contas";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosAnaliticos",
                    ViewOrigem = "RelacaoDiariaContas",
                    NomeRelatorio = "Relação diária de contas",
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
        public ActionResult RelacaoMensalContas()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Cliente a ser incluído
            FiltrosRelacaoContasModel model = new FiltrosRelacaoContasModel()
            {
                PesquisarPor = "DATAVENCIMENTO",
                DataCadastroInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                DataCadastroFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)),
                IndicadorEstornadas = false,
                IndicadorPerdida = false
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
        public ActionResult RelacaoMensalContas(FiltrosRelacaoContasModel model)
        {
            return ObterDadosRelacaoMensalContas(model, "RelacaoContasPagarReceberRv");
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RelacaoMensalContasGrafico(FiltrosRelacaoContasModel model)
        {
            return ObterDadosRelacaoMensalContas(model, "GraficoRelacaoMensalContasRv");
        }

        /// <summary>
        /// Obtem os dados e chama a máscara do relatório
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mascaraRelatorio"></param>
        /// <returns></returns>
        private ActionResult ObterDadosRelacaoMensalContas(FiltrosRelacaoContasModel model, string mascaraRelatorio)
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
                    Text = "Todos",
                    Value = "0"
                });
                model.ListaOpcoesPesquisa = Utilidades.RetornarOpcoesPesquisaContas();

                return View("RelacaoMensalContas", model);
            }

            string mensagemErro = "";
            RequisicaoObterRelacaoContasDto requisicaoDto = new RequisicaoObterRelacaoContasDto()
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
                    Text = "Todos",
                    Value = "0"
                });
                model.ListaOpcoesPesquisa = Utilidades.RetornarOpcoesPesquisaContas();

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View("RelacaoMensalContas", model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<RelacaoContasDto> retornoDto = new RetornoObterListaDto<RelacaoContasDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterRelacaoMensalContas(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaStatus = Utilidades.RetornarListaStatusConta();
                model.ListaStatus.Insert(0, new SelectListItem()
                {
                    Text = "Todos",
                    Value = "0"
                });
                model.ListaOpcoesPesquisa = Utilidades.RetornarOpcoesPesquisaContas();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View("RelacaoMensalContas", model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaStatus = Utilidades.RetornarListaStatusConta();
                model.ListaStatus.Insert(0, new SelectListItem()
                {
                    Text = "Todos",
                    Value = "0"
                });
                model.ListaOpcoesPesquisa = Utilidades.RetornarOpcoesPesquisaContas();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View("RelacaoMensalContas", model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!string.IsNullOrWhiteSpace(model.PesquisarPor))
            {
                filtros = $"Contas por '{model.PesquisarPor.ToLower().Replace("data", "")}' \n";
            }

            if (!model.IndicadorEstornadas)
            {
                filtros += $"Apenas contas não estornadas \n";
            }

            if (!model.IndicadorPerdida)
            {
                filtros += $"Apenas contas com status diferente de 'perdida' \n";
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);
                ReportParameter periodo = new ReportParameter("Periodo", $"{model.DataCadastroInicial.Value.ToString("dd/MM/yyyy")} até {model.DataCadastroFinal.Value.ToString("dd/MM/yyyy")}.");

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

                viewer.LocalReport.ReportPath = $"Relatorios/{mascaraRelatorio}.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.SetParameters(periodo);
                viewer.LocalReport.DisplayName = "Relação mensal de contas";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosAnaliticos",
                    ViewOrigem = "RelacaoMensalContas",
                    NomeRelatorio = "Relação mensal de contas",
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
        public ActionResult MelhoresMotoboys()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Cliente a ser incluído
            FiltrosMelhoresMotoboysModel model = new FiltrosMelhoresMotoboysModel()
            {
                CampoOrdem = "NOME",
                DataCadastroInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                DataCadastroFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)),
                Nome = "",
                ObterInativos = "false",
                Telefone = "",

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
        public ActionResult MelhoresMotoboys(FiltrosMelhoresMotoboysModel model)
        {
            return ObterDadosMelhoresMotoboys(model, "MelhoresMotoboysRv");
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MelhoresMotoboysGrafico(FiltrosMelhoresMotoboysModel model)
        {
            return ObterDadosMelhoresMotoboys(model, "GraficoMelhoresMotoboysRv");
        }

        /// <summary>
        /// Obtem os dados e chama o relatório de melhores motoboys
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mascaraRelatorio"></param>
        /// <returns></returns>
        private ActionResult ObterDadosMelhoresMotoboys(FiltrosMelhoresMotoboysModel model, string mascaraRelatorio)
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Validar a model recebida
            if (!ModelState.IsValid)
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoMotoboy();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                return View("MelhoresMotoboys ", model);
            }


            string mensagemErro = "";
            RequisicaoObterMelhoresMotoboysDto requisicaoDto = new RequisicaoObterMelhoresMotoboysDto()
            {
                Identificacao = SessaoUsuario.SessaoLogin.Identificacao,
                IdUsuario = SessaoUsuario.SessaoLogin.IdUsuario
            };

            //Converter para DTO
            if (!model.ConverterModelParaDto(ref requisicaoDto, ref mensagemErro))
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoMotoboy();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View("MelhoresMotoboys", model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<MelhoresCadastrosDto> retornoDto = new RetornoObterListaDto<MelhoresCadastrosDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterMelhoresMotoboys(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoMotoboy();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View("MelhoresMotoboys", model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoMotoboy();
                model.ListaInativo = Utilidades.RetornarListaOpcaoInativo();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View("MelhoresMotoboys", model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";

            if (!string.IsNullOrWhiteSpace(model.Nome))
            {
                filtros = $"Motoboy que tenha o nome '{model.Nome.Trim()}' \n";
            }

            if (!string.IsNullOrWhiteSpace(model.Telefone))
            {
                filtros = $"Motoboy que tenha o telefone '{model.Telefone.Trim()}' \n";
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);
                ReportParameter periodo = new ReportParameter("Periodo", $"{model.DataCadastroInicial.Value.Date.ToString("dd/MM/yyyy")} até {model.DataCadastroFinal.Value.Date.ToString("dd/MM/yyyy")}");

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

                viewer.LocalReport.ReportPath = $"Relatorios/{mascaraRelatorio}.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.SetParameters(periodo);
                viewer.LocalReport.DisplayName = "Melhores motoboys";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosAnaliticos",
                    ViewOrigem = "MelhoresMotobys",
                    NomeRelatorio = "Melhores motoboys",
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
        public ActionResult ContasPorFornecedor()
        {
            //Se não tiver login, encaminhar para a tela de login
            if (string.IsNullOrWhiteSpace(SessaoUsuario.SessaoLogin.Identificacao))
            {
                return RedirectToAction("Login", "Usuario");
            }

            //Cliente a ser incluído
            FiltrosRelacaoContasModel model = new FiltrosRelacaoContasModel()
            {
                PesquisarPor = "DATAVENCIMENTO",
                DataCadastroInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                DataCadastroFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)),
                IndicadorEstornadas = false,
                IndicadorPerdida = false
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
        public ActionResult ContasPorFornecedor(FiltrosRelacaoContasModel model)
        {
            return ContasPorFornecedor(model, "ContasPorFornecedorRv");
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContasPorFornecedorGrafico(FiltrosRelacaoContasModel model)
        {
            return ContasPorFornecedor(model, "GraficoContasPorFornecedorRv");
        }

        /// <summary>
        /// Obtem os dados e chama a máscara do relatório
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mascaraRelatorio"></param>
        /// <returns></returns>
        private ActionResult ContasPorFornecedor(FiltrosRelacaoContasModel model, string mascaraRelatorio)
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
                    Text = "Todos",
                    Value = "0"
                });
                model.ListaOpcoesPesquisa = Utilidades.RetornarOpcoesPesquisaContas();

                return View("ContasPorFornecedor", model);
            }

            string mensagemErro = "";
            RequisicaoObterRelacaoContasDto requisicaoDto = new RequisicaoObterRelacaoContasDto()
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
                    Text = "Todos",
                    Value = "0"
                });
                model.ListaOpcoesPesquisa = Utilidades.RetornarOpcoesPesquisaContas();

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View("ContasPorFornecedor", model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<ContasPorFornecedorDto> retornoDto = new RetornoObterListaDto<ContasPorFornecedorDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterContasPorFornecedor(requisicaoDto, ref retornoDto);

            //Verificar o retorno 
            if (!retornoDto.Retorno)
            {
                //Se houver erro, exibir na tela de inclusão
                model.ListaStatus = Utilidades.RetornarListaStatusConta();
                model.ListaStatus.Insert(0, new SelectListItem()
                {
                    Text = "Todos",
                    Value = "0"
                });
                model.ListaOpcoesPesquisa = Utilidades.RetornarOpcoesPesquisaContas();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View("ContasPorFornecedor", model);
            }

            // Se não houver resultados
            if (retornoDto.ListaEntidades.Count <= 0)
            {
                model.ListaStatus = Utilidades.RetornarListaStatusConta();
                model.ListaStatus.Insert(0, new SelectListItem()
                {
                    Text = "Todos",
                    Value = "0"
                });
                model.ListaOpcoesPesquisa = Utilidades.RetornarOpcoesPesquisaContas();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View("ContasPorFornecedor", model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!string.IsNullOrWhiteSpace(model.PesquisarPor))
            {
                filtros = $"Contas por '{model.PesquisarPor.ToLower().Replace("data", "")}' \n";
            }

            if (!model.IndicadorEstornadas)
            {
                filtros += $"Apenas contas não estornadas \n";
            }

            if (!model.IndicadorPerdida)
            {
                filtros += $"Apenas contas com status diferente de 'perdida' \n";
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);
                ReportParameter periodo = new ReportParameter("Periodo", $"{model.DataCadastroInicial.Value.ToString("dd/MM/yyyy")} até {model.DataCadastroFinal.Value.ToString("dd/MM/yyyy")}.");

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

                viewer.LocalReport.ReportPath = $"Relatorios/{mascaraRelatorio}.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.SetParameters(periodo);
                viewer.LocalReport.DisplayName = "Relação mensal de contas";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosAnaliticos",
                    ViewOrigem = "ContasPorFornecedor",
                    NomeRelatorio = "Contas por fornecedor",
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
        public ActionResult PedidosPorBairro()
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
                Tipo = TipoPedido.NaoIdentificado,
                ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoPedidoPorBairro()
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
        public ActionResult PedidosPorBairro(FiltrosListagemPedidoModel model)
        {
            return ObterDadosPedidosPorBairro(model, "PedidosPorBairroRv");
        }

        /// <summary>
        /// Faz o post, obtem os dados e monta o relatório
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PedidosPorBairroGrafico(FiltrosListagemPedidoModel model)
        {
            return ObterDadosPedidosPorBairro(model, "GraficoPedidosPorBairroRv");
        }

        /// <summary>
        /// Obtem os dados e chama a máscara do relatório
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mascaraRelatorio"></param>
        /// <returns></returns>
        private ActionResult ObterDadosPedidosPorBairro(FiltrosListagemPedidoModel model, string mascaraRelatorio)
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
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoPedidoPorBairro();

                return View("PedidosPorBairro", model);
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
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoPedidoPorBairro();

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View("PedidosPorBairro", model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<PedidosPorBairroDto> retornoDto = new RetornoObterListaDto<PedidosPorBairroDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterPedidosPorBairro(requisicaoDto, ref retornoDto);

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
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoPedidoPorBairro();

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View("PedidosPorBairro", model);
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
                model.ListaCampoOrdem = Utilidades.RetornarListaOpcaoOrdenacaoPedidoPorBairro();

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View("PedidosPorBairro", model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!Utilidades.RetornarFiltroPreenchidosPedido(model, ref filtros))
            {
                ViewBag.MensagemErro = "Problemas ao preencher os filtro";
                return View("Erro");
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);
                ReportParameter periodo = new ReportParameter("Periodo", $"{model.DataCadastroInicial.Value.ToString("dd/MM/yyyy")} até {model.DataCadastroFinal.Value.ToString("dd/MM/yyyy")}.");

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

                viewer.LocalReport.ReportPath = $"Relatorios/{mascaraRelatorio}.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.SetParameters(periodo);
                viewer.LocalReport.DisplayName = "Pedidos por bairro";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosAnaliticos",
                    ViewOrigem = "PedidosPorBairro",
                    NomeRelatorio = "Pedidos por bairro",
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
        public ActionResult AnaliseSemanalPedidos()
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
        public ActionResult AnaliseSemanalPedidos(FiltrosListagemPedidoModel model)
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

                return View("AnaliseSemanalPedidos", model);
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

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View("AnaliseSemanalPedidos", model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<ProdutosVendidosPorDiaSemanaDto> retornoDto = new RetornoObterListaDto<ProdutosVendidosPorDiaSemanaDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterEvolucaoSemanalPedidos(requisicaoDto, ref retornoDto);

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

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View("AnaliseSemanalPedidos", model);
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

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View("AnaliseSemanalPedidos", model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!Utilidades.RetornarFiltroPreenchidosPedido(model, ref filtros))
            {
                ViewBag.MensagemErro = "Problemas ao preencher os filtro";
                return View("Erro");
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);
                ReportParameter periodo = new ReportParameter("Periodo", $"{model.DataCadastroInicial.Value.ToString("dd/MM/yyyy")} até {model.DataCadastroFinal.Value.ToString("dd/MM/yyyy")}.");

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

                viewer.LocalReport.ReportPath = $"Relatorios/GraficoEvolucaoSemanalPedidosRv.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.SetParameters(periodo);
                viewer.LocalReport.DisplayName = "Análise semanal de pedidos";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosAnaliticos",
                    ViewOrigem = "AnaliseSemanalPedidos",
                    NomeRelatorio = viewer.LocalReport.DisplayName,
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
        public ActionResult AnaliseMensalPedidos()
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
        public ActionResult AnaliseMensalPedidos(FiltrosListagemPedidoModel model)
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

                return View("AnaliseMensalPedidos", model);
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

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View("AnaliseMensalPedidos", model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<ProdutosVendidosPorMesDto> retornoDto = new RetornoObterListaDto<ProdutosVendidosPorMesDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterEvolucaoMensalPedidos(requisicaoDto, ref retornoDto);

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

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View("AnaliseMensalPedidos", model);
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

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View("AnaliseMensalPedidos", model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!Utilidades.RetornarFiltroPreenchidosPedido(model, ref filtros))
            {
                ViewBag.MensagemErro = "Problemas ao preencher os filtro";
                return View("Erro");
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);
                ReportParameter periodo = new ReportParameter("Periodo", $"{model.DataCadastroInicial.Value.ToString("dd/MM/yyyy")} até {model.DataCadastroFinal.Value.ToString("dd/MM/yyyy")}.");

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

                viewer.LocalReport.ReportPath = $"Relatorios/GraficoEvolucaoMensalPedidosRv.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.SetParameters(periodo);
                viewer.LocalReport.DisplayName = "Análise mensal de pedidos";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosAnaliticos",
                    ViewOrigem = "AnaliseMensalPedidos",
                    NomeRelatorio = viewer.LocalReport.DisplayName,
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
        public ActionResult AnaliseAnualPedidos()
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
        public ActionResult AnaliseAnualPedidos(FiltrosListagemPedidoModel model)
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

                return View("AnaliseAnualPedidos", model);
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

                ModelState.AddModelError("", $"Erro ao converter para Dto: {mensagemErro}");
                return View("AnaliseAnualPedidos", model);
            }

            //Preparar requisição e retorno
            RetornoObterListaDto<ProdutosVendidosPorMesDto> retornoDto = new RetornoObterListaDto<ProdutosVendidosPorMesDto>();

            //Consumir o serviço
            RelatoriosBll relatoriosBll = new RelatoriosBll();
            relatoriosBll.ObterEvolucaoAnualPedidos(requisicaoDto, ref retornoDto);

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

                ModelState.AddModelError("", retornoDto.Mensagem);
                return View("AnaliseAnualPedidos", model);
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

                ModelState.AddModelError("", "Nenhum registro encontrado com filtros preenchidos");
                return View("AnaliseAnualPedidos", model);
            }

            // Se houver resultado, preencher os filtros usados
            string filtros = "";
            if (!Utilidades.RetornarFiltroPreenchidosPedido(model, ref filtros))
            {
                ViewBag.MensagemErro = "Problemas ao preencher os filtro";
                return View("Erro");
            }

            try
            {
                ReportParameter filtro = new ReportParameter("Filtros", filtros);
                ReportParameter periodo = new ReportParameter("Periodo", $"{model.DataCadastroInicial.Value.ToString("dd/MM/yyyy")} até {model.DataCadastroFinal.Value.ToString("dd/MM/yyyy")}.");

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

                viewer.LocalReport.ReportPath = $"Relatorios/GraficoEvolucaoAnualPedidosRv.rdlc";
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet", retornoDto.ListaEntidades));
                viewer.LocalReport.SetParameters(filtro);
                viewer.LocalReport.SetParameters(periodo);
                viewer.LocalReport.DisplayName = "Análise anual de pedidos";

                EmissaoRelatorioModel modelRelatorio = new EmissaoRelatorioModel()
                {
                    ControllerOrigem = "RelatoriosAnaliticos",
                    ViewOrigem = "AnaliseAnualPedidos",
                    NomeRelatorio = viewer.LocalReport.DisplayName,
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
