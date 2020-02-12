using PizzaByteBll.Base;
using PizzaByteDal;
using PizzaByteDto.ClassesBase;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteBll
{
    public class RelatoriosBll
    {
        private static LogBll logBll = new LogBll("RelatoriosBll");

        /// <summary>
        /// Obtem os dados para montar o dashboard da tela inicial
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterDadosDashboard(BaseRequisicaoDto requisicaoDto, ref RetornoObterInformacoesDashboardDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterInformacoesDashboard, Guid.Empty, mensagemErro);
                return false;
            }

            string queryClientesNovos = "SELECT" +
                " ISNULL(COUNT(Id), 0) AS QuantidadeNovosClientes" +
                " FROM PizzaByte.Clientes" +
                " WHERE MONTH(DataInclusao) = " + DateTime.Now.Month +
                " AND YEAR(DataInclusao) = " + DateTime.Now.Year +
                " AND Excluido = 0 AND Inativo = 0";

            string queryQuantidadePedidos = "SELECT " +
                " ISNULL(SUM(CASE WHEN Inativo = 1 THEN 1 ELSE 0 END), 0) AS QuantidadePedidosCancelados," +
                " ISNULL(COUNT(Id), 0) AS QuantidadePedidos" +
                " FROM PizzaByte.Pedidos" +
                " WHERE MONTH(DataInclusao) = " + DateTime.Now.Month +
                " AND YEAR(DataInclusao) = " + DateTime.Now.Year +
                " AND Excluido = 0";

            string queryContas = "SELECT" +
                " CASE WHEN COUNT(Id) = 0 THEN CAST(0.0 AS float)" +
                " ELSE CAST(ROUND(((ISNULL(SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END), 0) * 100)/ COUNT(Id)), 2) AS float) END AS PercentualContasQuitadas" +
                " FROM PizzaByte.ContaPagar" +
                " WHERE MONTH(DataVencimento) = " + DateTime.Now.Month +
                " AND YEAR(DataVencimento) = " + DateTime.Now.Year +
                " AND Status != 3 AND Status != 4";

            string queryTipoPedidoDiaSemana = "SELECT" +
                " CASE WHEN COUNT(Id) = 0 THEN 0 ELSE (CAST(SUM(CASE WHEN Tipo = 1 THEN 1 ELSE 0 END) AS float) * 100)/ COUNT(ID) END AS PercentualPedidosBalcaoSemana," +
                " CASE WHEN COUNT(Id) = 0 THEN 0 ELSE (CAST(SUM(CASE WHEN Tipo = 2 THEN 1 ELSE 0 END) AS float) * 100)/ COUNT(ID) END AS PercentualPedidosRetiradaSemana," +
                " CASE WHEN COUNT(Id) = 0 THEN 0 ELSE (CAST(SUM(CASE WHEN Tipo = 3 THEN 1 ELSE 0 END) AS float) * 100)/ COUNT(ID) END AS PercentualPedidosEntregaSemana" +
                " FROM PizzaByte.Pedidos" +
                " WHERE CAST(DataInclusao AS Date) BETWEEN '" + DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd") +
                "' AND '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND Inativo = 0 AND Excluido = 0";

            string queryProdutosDiaSemana = "SELECT CASE WHEN DATEPART(weekday, DataInclusao) = 2 THEN 'Seg'" +
                " WHEN DATEPART(weekday, DataInclusao) = 3 THEN 'Ter'" +
                " WHEN DATEPART(weekday, DataInclusao) = 4 THEN 'Qua'" +
                " WHEN DATEPART(weekday, DataInclusao) = 5 THEN 'Qui'" +
                " WHEN DATEPART(weekday, DataInclusao) = 6 THEN 'Sex'" +
                " WHEN DATEPART(weekday, DataInclusao) = 7 THEN 'Sáb'" +
                " ELSE 'Dom' END AS DiaSemana," +
                " CAST(SUM(CASE WHEN TipoProduto = 1 THEN Quantidade ELSE 0 END) AS int) AS Pizza," +
                " CAST(SUM(CASE WHEN TipoProduto = 2 THEN Quantidade ELSE 0 END) AS int) AS Bebida" +
                " FROM PizzaByte.PedidosItens" +
                " WHERE CAST(DataInclusao AS Date) BETWEEN '" + DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd") +
                "' AND '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND Inativo = 0 AND Excluido = 0" +
                " GROUP BY DATEPART(weekday, DataInclusao)";

            string queryPedidosMes = "SELECT ISNULL(COUNT(Id), 0) AS Pedidos," +
               " CASE WHEN MONTH(DataInclusao) = 1 THEN 'Jan'" +
               " WHEN MONTH(DataInclusao) = 2 THEN 'Fev'" +
               " WHEN MONTH(DataInclusao) = 4 THEN 'Abr'" +
               " WHEN MONTH(DataInclusao) = 5 THEN 'Mai'" +
               " WHEN MONTH(DataInclusao) = 6 THEN 'Jun'" +
               " WHEN MONTH(DataInclusao) = 7 THEN 'Jul'" +
               " WHEN MONTH(DataInclusao) = 8 THEN 'Ago'" +
               " WHEN MONTH(DataInclusao) = 9 THEN 'Set'" +
               " WHEN MONTH(DataInclusao) = 10 THEN 'Out'" +
               " WHEN MONTH(DataInclusao) = 11 THEN 'Nov'" +
               " ELSE 'Dez' END AS Mes" +
               " FROM PizzaByte.Pedidos" +
               " WHERE YEAR(DataInclusao) = " + DateTime.Now.Year +
               " AND Inativo = 0 AND Excluido = 0 GROUP BY MONTH(DataInclusao)";

            try
            {
                using (PizzaByteContexto pizzaByteContexto = new PizzaByteContexto())
                {
                    RetornoObterInformacoesDashboardDto clientesNovos = pizzaByteContexto.Database.SqlQuery<RetornoObterInformacoesDashboardDto>(queryClientesNovos).FirstOrDefault();
                    RetornoObterInformacoesDashboardDto pedidos = pizzaByteContexto.Database.SqlQuery<RetornoObterInformacoesDashboardDto>(queryQuantidadePedidos).FirstOrDefault();
                    RetornoObterInformacoesDashboardDto contas = pizzaByteContexto.Database.SqlQuery<RetornoObterInformacoesDashboardDto>(queryContas).FirstOrDefault();
                    RetornoObterInformacoesDashboardDto pedidosDiaSemana = pizzaByteContexto.Database.SqlQuery<RetornoObterInformacoesDashboardDto>(queryTipoPedidoDiaSemana).FirstOrDefault();

                    // Preencher todos os dias da semana
                    var produtosPorSemana = pizzaByteContexto.Database.SqlQuery<ProdutosVendidosPorDiaSemanaDto>(queryProdutosDiaSemana).ToList();
                    for (int i = (int)DateTime.Now.DayOfWeek; retornoDto.ListaProdutosVendidosPorDiaSemana.Count() < 7; i++)
                    {
                        ProdutosVendidosPorDiaSemanaDto produtos = produtosPorSemana.Where(p => p.DiaSemana.Trim() == UtilitarioBll.RetornaDiaSemana(i)).FirstOrDefault();
                        retornoDto.ListaProdutosVendidosPorDiaSemana.Insert(0, new ProdutosVendidosPorDiaSemanaDto()
                        {
                            DiaSemana = UtilitarioBll.RetornaDiaSemana(i),
                            Bebida = (produtos != null) ? produtos.Bebida : 0,
                            Pizza = (produtos != null) ? produtos.Pizza : 0,
                        });

                        if (i == 6)
                        {
                            i = -1;
                        }
                    }

                    // Preencher todos os meses
                    var pedidosMes = pizzaByteContexto.Database.SqlQuery<PedidosPorMesDto>(queryPedidosMes).ToList();
                    for (int i = 0; i < 12; i++)
                    {
                        PedidosPorMesDto mes = pedidosMes.Where(p => p.Mes.Trim() == UtilitarioBll.RetornaMes(i)).FirstOrDefault();
                        if (mes == null)
                        {
                            retornoDto.ListaPedidosPorMes.Add(new PedidosPorMesDto()
                            {
                                Mes = UtilitarioBll.RetornaMes(i),
                                Pedidos = 0
                            });
                        }
                        else
                        {
                            retornoDto.ListaPedidosPorMes.Add(mes);
                        }
                    }

                    retornoDto.QuantidadeNovosClientes = clientesNovos.QuantidadeNovosClientes;
                    retornoDto.QuantidadePedidos = pedidos.QuantidadePedidos;
                    retornoDto.QuantidadePedidosCancelados = pedidos.QuantidadePedidosCancelados;
                    retornoDto.PercentualContasQuitadas = contas.PercentualContasQuitadas;
                    retornoDto.PercentualPedidosEntregaSemana = Math.Round(pedidosDiaSemana.PercentualPedidosEntregaSemana, 2);
                    retornoDto.PercentualPedidosRetiradaSemana = Math.Round(pedidosDiaSemana.PercentualPedidosRetiradaSemana, 2);
                    retornoDto.PercentualPedidosBalcaoSemana = Math.Round(pedidosDiaSemana.PercentualPedidosBalcaoSemana, 2);
                }

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Erro ao obter as informações: " + ex.Message;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterInformacoesDashboard, Guid.Empty, retornoDto.Mensagem);
                return false;
            }
        }

        /// <summary>
        /// Obtem a listagem de clientes para a emissão do relatório
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterListagemClientes(RequisicaoObterListagemClienteDto requisicaoDto, ref RetornoObterListaDto<ClienteDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemClientes, Guid.Empty, mensagemErro);
                return false;
            }

            string query = "SELECT Nome, Telefone, Cpf, DataInclusao FROM PizzaByte.Clientes WHERE Excluido = 0";
            List<SqlParameter> listaFiltros = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(requisicaoDto.Nome))
            {
                query += " AND Nome LIKE '%' + @nome + '%'";
                listaFiltros.Add(new SqlParameter("nome", requisicaoDto.Nome.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.Telefone))
            {
                query += " AND Telefone LIKE '%' + @tel + '%'";
                listaFiltros.Add(new SqlParameter("tel", requisicaoDto.Telefone.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.Cpf))
            {
                query += " AND Cpf LIKE '%' + @cpf + '%'";
                listaFiltros.Add(new SqlParameter("cpf", requisicaoDto.Cpf.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.ObterInativos))
            {
                query += " AND Inativo = @inativo";
                bool inativo;
                if (!bool.TryParse(requisicaoDto.ObterInativos, out inativo))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao converter o filtro de inativo";

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemClientes, Guid.Empty, mensagemErro);
                    return false;
                }

                listaFiltros.Add(new SqlParameter("inativo", inativo));
            }

            if (requisicaoDto.DataCadastroInicial.HasValue)
            {
                query += " AND CAST(DataInclusao AS Date) >= @dataInicio";
                listaFiltros.Add(new SqlParameter("dataInicio", requisicaoDto.DataCadastroInicial.Value));
            }

            if (requisicaoDto.DataCadastroFinal.HasValue)
            {
                query += " AND CAST(DataInclusao AS Date) <= @dataFim";
                listaFiltros.Add(new SqlParameter("dataFim", requisicaoDto.DataCadastroFinal.Value));
            }

            // Ordenar query
            switch (requisicaoDto.CampoOrdem)
            {
                case "NOME":
                default:
                    query += " ORDER BY Nome";
                    break;

                case "TELEFONE":
                    query += " ORDER BY Telefone";
                    break;

                case "CPF":
                    query += " ORDER BY Cpf";
                    break;

                case "DATACADASTROCRESCENTE":
                    query += " ORDER BY DataInclusao";
                    break;

                case "DATACADASTRODECRESCENTE":
                    query += " ORDER BY DataInclusao DESC";
                    break;
            }

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<ClienteDto>(query, listaFiltros.ToArray()).ToList();

                retornoDto.ListaEntidades.ForEach(p => p.Telefone = UtilitarioBll.RetornarTelefoneFormatado(p.Telefone));
                retornoDto.ListaEntidades.ForEach(p => p.Cpf = UtilitarioBll.RetornarCpfFormatado(p.Cpf));

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemClientes, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtem a listagem de clientes para a emissão do relatório
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterListagemFornecedores(RequisicaoObterListagemFornecedoresDto requisicaoDto, ref RetornoObterListaDto<FornecedorDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemFornecedores, Guid.Empty, mensagemErro);
                return false;
            }

            string query = "SELECT NomeFantasia, RazaoSocial, Telefone, Cnpj, DataInclusao FROM PizzaByte.Fornecedores WHERE Excluido = 0";
            List<SqlParameter> listaFiltros = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(requisicaoDto.NomeFantasia))
            {
                query += " AND NomeFantasia LIKE '%' + @nomeFantasia + '%'";
                listaFiltros.Add(new SqlParameter("nomeFantasia", requisicaoDto.NomeFantasia.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.RazaoSocial))
            {
                query += " AND RazaoSocial LIKE '%' + @razaoSocial + '%'";
                listaFiltros.Add(new SqlParameter("razaoSocial", requisicaoDto.RazaoSocial.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.Telefone))
            {
                query += " AND Telefone LIKE '%' + @tel + '%'";
                listaFiltros.Add(new SqlParameter("tel", requisicaoDto.Telefone.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.Cnpj))
            {
                query += " AND Cnpj LIKE '%' + @cnpj + '%'";
                listaFiltros.Add(new SqlParameter("cnpj", requisicaoDto.Cnpj.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.ObterInativos))
            {
                query += " AND Inativo = @inativo";
                bool inativo;
                if (!bool.TryParse(requisicaoDto.ObterInativos, out inativo))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao converter o filtro de inativo";

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemFornecedores, Guid.Empty, mensagemErro);
                    return false;
                }

                listaFiltros.Add(new SqlParameter("inativo", inativo));
            }

            if (requisicaoDto.DataCadastroInicial.HasValue)
            {
                query += " AND CAST(DataInclusao AS Date) >= @dataInicio";
                listaFiltros.Add(new SqlParameter("dataInicio", requisicaoDto.DataCadastroInicial.Value));
            }

            if (requisicaoDto.DataCadastroFinal.HasValue)
            {
                query += " AND CAST(DataInclusao AS Date) <= @dataFim";
                listaFiltros.Add(new SqlParameter("dataFim", requisicaoDto.DataCadastroFinal.Value));
            }

            // Ordenar query
            switch (requisicaoDto.CampoOrdem)
            {
                case "NOMEFANTASIA":
                default:
                    query += " ORDER BY NomeFantasia";
                    break;

                case "RAZAOSOCIAL":
                    query += " ORDER BY RazaoSocial";
                    break;

                case "TELEFONE":
                    query += " ORDER BY Telefone";
                    break;

                case "CNPJ":
                    query += " ORDER BY Cnpj";
                    break;

                case "DATACADASTROCRESCENTE":
                    query += " ORDER BY DataInclusao";
                    break;

                case "DATACADASTRODECRESCENTE":
                    query += " ORDER BY DataInclusao DESC";
                    break;
            }

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<FornecedorDto>(query, listaFiltros.ToArray()).ToList();

                retornoDto.ListaEntidades.ForEach(p => p.Telefone = UtilitarioBll.RetornarTelefoneFormatado(p.Telefone));
                retornoDto.ListaEntidades.ForEach(p => p.Cnpj = UtilitarioBll.RetornarCnpjFormatado(p.Cnpj));

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemFornecedores, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtem os dados para emissão do relatório de listagem de pedidos
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterListaResumidaPedidos(RequisicaoObterListagemPedidosDto requisicaoDto, ref RetornoObterListaDto<PedidoResumidoDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemResumidaPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            // Validar o período de pesquisa
            if (!requisicaoDto.DataCadastroInicial.HasValue || !requisicaoDto.DataCadastroFinal.HasValue)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Por favor, preencha o período de vendas que deseja pesquisar";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemResumidaPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.DataCadastroInicial.Value > requisicaoDto.DataCadastroFinal.Value)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Período de pesquisa inválido! Por favor, preencha o período corretamente.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemResumidaPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            string query = " WHERE CAST(p.DataInclusao AS Date) BETWEEN @inicio AND @fim AND p.Excluido = 0 ";

            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            listaFiltros.Add(new SqlParameter("inicio", requisicaoDto.DataCadastroInicial.Value.Date));
            listaFiltros.Add(new SqlParameter("fim", requisicaoDto.DataCadastroFinal.Value.Date));

            if (!RetornarWherePedidos(requisicaoDto, ref query, ref listaFiltros))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Erro ao filtrar os pedidos: " + query;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemResumidaPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            query = "SELECT p.Tipo," +
                " p.Total," +
                " p.TaxaEntrega," +
                " p.RecebidoDinheiro," +
                " p.RecebidoCredito," +
                " p.RecebidoDebito," +
                " p.PedidoIfood," +
                " p.Troco," +
                " p.DataInclusao," +
                " p.Inativo," +
                " RTRIM(c.Nome) AS NomeCliente" +
                " FROM PizzaByte.Pedidos AS p " +
                " LEFT JOIN PizzaByte.Clientes AS c ON p.IdCliente = c.Id AND c.Excluido = 0 " + query;

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<PedidoResumidoDto>(query, listaFiltros.ToArray()).ToList();
                retornoDto.ListaEntidades.ForEach(p => p.NomeCliente = string.IsNullOrWhiteSpace(p.NomeCliente) ? "Não identificado" : p.NomeCliente.Trim());

                // Ordenar query
                switch (requisicaoDto.CampoOrdem)
                {
                    case "NOMECLIENTE":
                    default:
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.NomeCliente).ThenBy(p => p.DataInclusao).ToList();
                        break;

                    case "TAXAENTREGA":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.TaxaEntrega).ThenBy(p => p.DataInclusao).ToList();
                        break;

                    case "TOTAL":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Total).ThenBy(p => p.DataInclusao).ToList();
                        break;

                    case "TIPO":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Tipo).ThenBy(p => p.DataInclusao).ToList();
                        break;

                    case "DATACADASTROCRESCENTE":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.DataInclusao).ThenBy(p => p.NomeCliente).ToList();
                        break;

                    case "DATACADASTRODECRESCENTE":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderByDescending(p => p.DataInclusao).ThenBy(p => p.NomeCliente).ToList();
                        break;
                }

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemResumidaPedidos, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtem a listagem de clientes para a emissão do relatório
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterListagemProdutos(RequisicaoObterListagemProdutoDto requisicaoDto, ref RetornoObterListaDto<ProdutoDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemProdutos, Guid.Empty, mensagemErro);
                return false;
            }

            string query = "SELECT Descricao, Preco, Tipo, DataInclusao FROM PizzaByte.Produtos WHERE Excluido = 0 AND Id != @idBrinde";
            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            listaFiltros.Add(new SqlParameter("idBrinde", UtilitarioBll.RetornaIdProdutoPromocao()));

            if (!string.IsNullOrWhiteSpace(requisicaoDto.Descricao))
            {
                query += " AND Descricao LIKE '%' + @descricao + '%'";
                listaFiltros.Add(new SqlParameter("descricao", requisicaoDto.Descricao.Trim()));
            }

            if (requisicaoDto.PrecoInicio > 0)
            {
                query += " AND Preco >= @precoInicio  ";
                listaFiltros.Add(new SqlParameter("precoInicio", requisicaoDto.PrecoInicio));
            }

            if (requisicaoDto.PrecoFim > 0)
            {
                query += " AND Preco <= @precoFim  ";
                listaFiltros.Add(new SqlParameter("precoFim", requisicaoDto.PrecoFim));
            }

            if (requisicaoDto.Tipo != TipoProduto.NaoIdentificado)
            {
                query += " AND Tipo = @tipo ";
                listaFiltros.Add(new SqlParameter("tipo", (int)requisicaoDto.Tipo));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.ObterInativos))
            {
                query += " AND Inativo = @inativo";
                bool inativo;
                if (!bool.TryParse(requisicaoDto.ObterInativos, out inativo))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao converter o filtro de inativo";

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemProdutos, Guid.Empty, mensagemErro);
                    return false;
                }

                listaFiltros.Add(new SqlParameter("inativo", inativo));
            }

            if (requisicaoDto.DataCadastroInicial.HasValue)
            {
                query += " AND CAST(DataInclusao AS Date) >= @dataInicio";
                listaFiltros.Add(new SqlParameter("dataInicio", requisicaoDto.DataCadastroInicial.Value));
            }

            if (requisicaoDto.DataCadastroFinal.HasValue)
            {
                query += " AND CAST(DataInclusao AS Date) <= @dataFim";
                listaFiltros.Add(new SqlParameter("dataFim", requisicaoDto.DataCadastroFinal.Value));
            }

            // Ordenar query
            switch (requisicaoDto.CampoOrdem)
            {
                case "DESCRICAO":
                default:
                    query += " ORDER BY Descricao";
                    break;

                case "PRECO":
                    query += " ORDER BY Preco";
                    break;

                case "TIPO":
                    query += " ORDER BY Tipo";
                    break;

                case "DATACADASTROCRESCENTE":
                    query += " ORDER BY DataInclusao";
                    break;

                case "DATACADASTRODECRESCENTE":
                    query += " ORDER BY DataInclusao DESC";
                    break;
            }

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<ProdutoDto>(query, listaFiltros.ToArray()).ToList();

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemProdutos, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtem os dados para emissão do relatório de listagem de pedidos
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterListagemContaPagar(RequisicaoObterListagemContaPagarDto requisicaoDto, ref RetornoObterListaDto<ContaPagarDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemContaPagar, Guid.Empty, mensagemErro);
                return false;
            }

            // Validar o período de pesquisa
            if (!requisicaoDto.DataCadastroInicial.HasValue || !requisicaoDto.DataCadastroFinal.HasValue)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Por favor, preencha o período das contas que deseja pesquisar.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemContaPagar, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.DataCadastroInicial.Value > requisicaoDto.DataCadastroFinal.Value)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Período de pesquisa inválido! Por favor, preencha o período corretamente.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemContaPagar, Guid.Empty, mensagemErro);
                return false;
            }

            string query = " SELECT c.DataPagamento, c.DataCompetencia, f.NomeFantasia AS NomeFornecedor, c.DataVencimento, c.Descricao, c.Valor, c.Status, c.DataInclusao" +
                " FROM PizzaByte.ContaPagar AS c " +
                " LEFT JOIN PizzaByte.Fornecedores AS f ON c.IdFornecedor = f.NomeFantasia AND f.Excluido = 0" +
                " WHERE c.Excluido = 0";

            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            if (!ObterFiltrosContas(requisicaoDto, ref listaFiltros, ref query, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            if (requisicaoDto.IdFornecedor != Guid.Empty)
            {
                query += " AND c.IdFornecedor = @idFornecedor ";
                listaFiltros.Add(new SqlParameter("idFornecedor", requisicaoDto.IdFornecedor));
            }

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<ContaPagarDto>(query, listaFiltros.ToArray()).ToList();
                retornoDto.ListaEntidades.ForEach(p => p.Descricao = string.IsNullOrWhiteSpace(p.Descricao) ? "Não identificado" : p.Descricao.Trim());

                // Ordenar query
                switch (requisicaoDto.CampoOrdem)
                {
                    case "DESCRICAO":
                    default:
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Descricao).ThenBy(p => p.DataInclusao).ToList();
                        break;

                    case "DATAVENCIMENTO":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.DataVencimento).ThenBy(p => p.DataInclusao).ToList();
                        break;

                    case "DATAPAGAMENTO":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.DataPagamento).ThenBy(p => p.DataInclusao).ToList();
                        break;

                    case "DATACOMPETENCIA":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderByDescending(p => p.DataInclusao).ThenBy(p => p.Descricao).ToList();
                        break;
                }

                retornoDto.ListaEntidades.ForEach(p => p.NomeFornecedor = (p.IdFornecedor == Guid.Empty || p.IdFornecedor == null) ? "Não informado" : p.NomeFornecedor);
                retornoDto.ListaEntidades.ForEach(p => p.NomeFornecedor = string.IsNullOrWhiteSpace(p.NomeFornecedor) ? "Não encontrado" : p.NomeFornecedor);
                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemContaPagar, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtem os dados para emissão do relatório de listagem de pedidos
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterListagemContaReceber(RequisicaoObterListagemContaReceberDto requisicaoDto, ref RetornoObterListaDto<ContaReceberDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemContaReceber, Guid.Empty, mensagemErro);
                return false;
            }

            // Validar o período de pesquisa
            if (!requisicaoDto.DataCadastroInicial.HasValue || !requisicaoDto.DataCadastroFinal.HasValue)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Por favor, preencha o período das contas que deseja pesquisar.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemContaReceber, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.DataCadastroInicial.Value > requisicaoDto.DataCadastroFinal.Value)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Período de pesquisa inválido! Por favor, preencha o período corretamente.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemContaReceber, Guid.Empty, mensagemErro);
                return false;
            }

            string query = " SELECT c.DataVencimento, c.DataCompetencia, c.Descricao, c.Valor, c.Status, c.DataInclusao FROM PizzaByte.ContaReceber AS c" +
                " WHERE c.Excluido = 0 ";
            List<SqlParameter> listaFiltros = new List<SqlParameter>();

            if (!ObterFiltrosContas(requisicaoDto, ref listaFiltros, ref query, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<ContaReceberDto>(query, listaFiltros.ToArray()).ToList();
                retornoDto.ListaEntidades.ForEach(p => p.Descricao = string.IsNullOrWhiteSpace(p.Descricao) ? "Não identificado" : p.Descricao.Trim());

                // Ordenar query
                switch (requisicaoDto.CampoOrdem)
                {
                    case "DESCRICAO":
                    default:
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Descricao).ThenBy(p => p.DataInclusao).ToList();
                        break;

                    case "DATAVENCIMENTO":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.DataVencimento).ThenBy(p => p.DataInclusao).ToList();
                        break;

                    case "DATACOMPETENCIA":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.DataCompetencia).ThenBy(p => p.Descricao).ToList();
                        break;
                }

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemContaReceber, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Filtra a lista de contas
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="listaFiltros"></param>
        /// <param name="query"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ObterFiltrosContas(RequisicaoObterListagemContasDto requisicaoDto, ref List<SqlParameter> listaFiltros, ref string query, ref string mensagemErro)
        {
            listaFiltros.Add(new SqlParameter("dataInicio", requisicaoDto.DataCadastroInicial.Value));
            listaFiltros.Add(new SqlParameter("dataFim", requisicaoDto.DataCadastroFinal.Value));

            switch (requisicaoDto.PesquisarPor.Trim())
            {
                case "DATAVENCIMENTO":
                    query += " AND CAST(c.DataVencimento AS Date) BETWEEN @dataInicio AND @dataFim";
                    break;

                case "DATACOMPETENCIA":
                    query += " AND CAST(c.DataCompetencia AS Date) BETWEEN @dataInicio AND @dataFim";
                    break;

                case "DATAPAGAMENTO":
                    query += " AND CAST(c.DataPagamento AS Date) BETWEEN @dataInicio AND @dataFim";
                    break;

                default:
                    mensagemErro = $"A pesquisa por {requisicaoDto.PesquisarPor} não é válida";
                    return false;
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.Descricao))
            {
                query += " AND f.Descricao LIKE '%' + @descricao + '%'";
                listaFiltros.Add(new SqlParameter("descricao", requisicaoDto.Descricao.Trim()));
            }

            if (requisicaoDto.Status != StatusConta.NaoIdentificado)
            {
                query += " AND Status = @status ";
                listaFiltros.Add(new SqlParameter("status", (int)requisicaoDto.Status));
            }

            return true;
        }

        /// <summary>
        /// Obtem os dados para emissão do relatório de listagem de pedidos (detalhada)
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterListaDetalhadaPedidos(RequisicaoObterListagemPedidosDto requisicaoDto, ref RetornoObterListaDto<PedidoDetalhadoDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemDetalhadaPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            // Validar o período de pesquisa
            if (!requisicaoDto.DataCadastroInicial.HasValue || !requisicaoDto.DataCadastroFinal.HasValue)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Por favor, preencha o período de vendas que deseja pesquisar";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemDetalhadaPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.DataCadastroInicial.Value > requisicaoDto.DataCadastroFinal.Value)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Período de pesquisa inválido! Por favor, preencha o período corretamente.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemDetalhadaPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            string query = " WHERE CAST(p.DataInclusao AS Date) BETWEEN @inicio AND @fim AND p.Excluido = 0 ";

            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            listaFiltros.Add(new SqlParameter("inicio", requisicaoDto.DataCadastroInicial.Value.Date));
            listaFiltros.Add(new SqlParameter("fim", requisicaoDto.DataCadastroFinal.Value.Date));

            if (!RetornarWherePedidos(requisicaoDto, ref query, ref listaFiltros))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Erro ao filtrar os pedidos: " + query;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemDetalhadaPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            query = "SELECT p.Id," +
                " p.Tipo," +
                " p.Total," +
                " p.TaxaEntrega," +
                " p.RecebidoDinheiro," +
                " p.RecebidoCredito," +
                " p.RecebidoDebito," +
                " p.PedidoIfood," +
                " p.Troco," +
                " p.DataInclusao," +
                " p.Inativo," +
                " i.Inativo AS ItemInativo," +
                " RTRIM(p.Obs) AS Obs," +
                " RTRIM(p.JustificativaCancelamento) AS JustificativaCancelamento," +
                " RTRIM(i.DescricaoProduto) AS DescricaoProduto," +
                " i.PrecoProduto AS PrecoProduto," +
                " i.TipoProduto AS TipoProduto," +
                " i.Quantidade AS Quantidade," +
                " RTRIM(c.Nome) AS NomeCliente" +
                " FROM PizzaByte.Pedidos AS p " +
                " LEFT JOIN PizzaByte.Clientes AS c ON p.IdCliente = c.Id AND c.Excluido = 0 " +
                " LEFT JOIN PizzaByte.PedidosItens AS i ON p.Id = i.IdPedido AND i.Excluido = 0 " + query;

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<PedidoDetalhadoDto>(query, listaFiltros.ToArray()).ToList();
                retornoDto.ListaEntidades.ForEach(p => p.NomeCliente = string.IsNullOrWhiteSpace(p.NomeCliente) ? "Não identificado" : p.NomeCliente.Trim());

                // Ordenar query
                switch (requisicaoDto.CampoOrdem)
                {
                    case "NOMECLIENTE":
                    default:
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.NomeCliente).ThenBy(p => p.DataInclusao).ToList();
                        break;

                    case "TAXAENTREGA":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.TaxaEntrega).ThenBy(p => p.DataInclusao).ToList();
                        break;

                    case "TOTAL":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Total).ThenBy(p => p.DataInclusao).ToList();
                        break;

                    case "TIPO":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Tipo).ThenBy(p => p.DataInclusao).ToList();
                        break;

                    case "DATACADASTROCRESCENTE":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.DataInclusao).ThenBy(p => p.NomeCliente).ToList();
                        break;

                    case "DATACADASTRODECRESCENTE":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderByDescending(p => p.DataInclusao).ThenBy(p => p.NomeCliente).ToList();
                        break;
                }

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemDetalhadaPedidos, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Monta o Where do Select para filtrar pedidos
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="where"></param>
        /// <param name="listaFiltros"></param>
        /// <returns></returns>
        private bool RetornarWherePedidos(RequisicaoObterListagemPedidosDto requisicaoDto, ref string where, ref List<SqlParameter> listaFiltros, bool detalhe = false)
        {
            if (requisicaoDto.IdCliente != null)
            {
                where += " AND p.IdCliente = @cliente ";
                listaFiltros.Add(new SqlParameter("cliente", requisicaoDto.IdCliente));
            }

            if (requisicaoDto.IndicadorCredito)
            {
                where += " AND p.RecebidoCredito > 0 ";
            }

            if (requisicaoDto.IndicadorDebito)
            {
                where += " AND p.RecebidoDebito > 0 ";
            }

            if (requisicaoDto.IndicadorDinheiro)
            {
                where += " AND p.RecebidoDinheiro > 0 ";
            }

            if (requisicaoDto.IndicadorPromocao)
            {
                where += " AND p.Id IN (SELECT IdPedido FROM PizzaByte.PedidosItens WHERE IdProduto = @idBrinde" +
                    " OR IdProdutoComposto = @idBrinde AND Inativo = 0 AND Excluido = 0) ";
                listaFiltros.Add(new SqlParameter("idBrinde", UtilitarioBll.RetornaIdProdutoPromocao()));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.JustificativaCancelamento))
            {
                where += " AND p.JustificativaCancelamento LIKE '%' + @justificativa + '%' ";
                listaFiltros.Add(new SqlParameter("justificativa", requisicaoDto.JustificativaCancelamento.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.Obs))
            {
                where += " AND p.Obs LIKE '%' + @obs + '%' ";
                listaFiltros.Add(new SqlParameter("obs", requisicaoDto.Obs.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.ObterInativos))
            {
                where += " AND p.Inativo = @inativo";
                bool inativo;
                if (!bool.TryParse(requisicaoDto.ObterInativos, out inativo))
                {
                    where = "Falha ao converter o filtro de inativo";
                    return false;
                }

                listaFiltros.Add(new SqlParameter("inativo", inativo));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.PedidoIfood))
            {
                where += " AND p.PedidoIfood = @ifood";
                bool ifood;
                if (!bool.TryParse(requisicaoDto.PedidoIfood, out ifood))
                {
                    where = "Falha ao converter o filtro de IFood";
                    return false;
                }

                listaFiltros.Add(new SqlParameter("ifood", ifood));
            }

            if (requisicaoDto.TaxaEntregaInicial > 0)
            {
                where += " AND p.TaxaEntrega >= @taxa";
                listaFiltros.Add(new SqlParameter("taxa", requisicaoDto.TaxaEntregaInicial));
            }

            if (requisicaoDto.TaxaEntregaFinal > 0)
            {
                where += " AND p.TaxaEntrega <= @taxaFinal";
                listaFiltros.Add(new SqlParameter("taxaFinal", requisicaoDto.TaxaEntregaFinal));
            }

            if (requisicaoDto.Tipo != TipoPedido.NaoIdentificado)
            {
                where += " AND p.Tipo = @tipo";
                listaFiltros.Add(new SqlParameter("tipo", (int)requisicaoDto.Tipo));
            }

            if (requisicaoDto.TotalInicial > 0)
            {
                where += " AND p.Total >= @total";
                listaFiltros.Add(new SqlParameter("total", requisicaoDto.TotalInicial));
            }

            if (requisicaoDto.TotalFinal > 0)
            {
                where += " AND p.Total <= @totalFinal";
                listaFiltros.Add(new SqlParameter("totalFinal", requisicaoDto.TotalFinal));
            }

            if (requisicaoDto.TrocoInicial > 0)
            {
                where += " AND p.Troco >= @troco";
                listaFiltros.Add(new SqlParameter("troco", requisicaoDto.TrocoInicial));
            }

            if (requisicaoDto.TrocoFinal > 0)
            {
                where += " AND p.Troco <= @trocoFinal";
                listaFiltros.Add(new SqlParameter("trocoFinal", requisicaoDto.TrocoFinal));
            }

            return true;
        }

        /// <summary>
        /// Obtem a listagem de taxas de entregas para a emissão do relatório
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterListagemTaxaEntrega(RequisicaoObterListagemTaxaEntregaDto requisicaoDto, ref RetornoObterListaDto<TaxaEntregaDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemTaxaEntrega, Guid.Empty, mensagemErro);
                return false;
            }

            string query = "SELECT BairroCidade, ValorTaxa, DataInclusao FROM PizzaByte.TaxasEntrega WHERE Excluido = 0";
            List<SqlParameter> listaFiltros = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(requisicaoDto.BairroCidade))
            {
                query += " AND BairroCidade LIKE '%' + @bairroCidade + '%'";
                listaFiltros.Add(new SqlParameter("bairroCidade", requisicaoDto.BairroCidade.Trim()));
            }

            if (requisicaoDto.ValorInicio > 0)
            {
                query += " AND ValorTaxa >= @valorInicio  ";
                listaFiltros.Add(new SqlParameter("valorInicio", requisicaoDto.ValorInicio));
            }

            if (requisicaoDto.ValorFim > 0)
            {
                query += " AND ValorTaxa <= @valorFim  ";
                listaFiltros.Add(new SqlParameter("valorFim", requisicaoDto.ValorFim));
            }

            if (requisicaoDto.DataCadastroInicial.HasValue)
            {
                query += " AND CAST(DataInclusao AS Date) >= @dataInicio";
                listaFiltros.Add(new SqlParameter("dataInicio", requisicaoDto.DataCadastroInicial.Value));
            }

            if (requisicaoDto.DataCadastroFinal.HasValue)
            {
                query += " AND CAST(DataInclusao AS Date) <= @dataFim";
                listaFiltros.Add(new SqlParameter("dataFim", requisicaoDto.DataCadastroFinal.Value));
            }

            // Ordenar query
            switch (requisicaoDto.CampoOrdem)
            {
                case "BAIRROCIDADE":
                default:
                    query += " ORDER BY BairroCidade";
                    break;

                case "VALORTAXA":
                    query += " ORDER BY ValorTaxa";
                    break;

                case "DATACADASTROCRESCENTE":
                    query += " ORDER BY DataInclusao";
                    break;

                case "DATACADASTRODECRESCENTE":
                    query += " ORDER BY DataInclusao DESC";
                    break;
            }

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<TaxaEntregaDto>(query, listaFiltros.ToArray()).ToList();
                retornoDto.ListaEntidades.ForEach(p => p.Cidade = p.BairroCidade.Split('_')[1]);
                retornoDto.ListaEntidades.ForEach(p => p.BairroCidade = p.BairroCidade.Split('_')[0]);

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemTaxaEntrega, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtem a listagem de clientes para a emissão do relatório
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterListagemEntregas(RequisicaoObterListagemEntregasDto requisicaoDto, ref RetornoObterListaDto<ListagemEntregaDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemEntregas, Guid.Empty, mensagemErro);
                return false;
            }

            string query = "SELECT" +
                            " (RTRIM(c.Logradouro) + ', ' + ce.NumeroEndereco + ' - '" +
                            " + RTRIM(c.Bairro) + '/' + RTRIM(c.Cidade)) AS Logradouro, " +
                            " RTRIM(f.Nome) AS NomeFuncionario, " +
                            " CAST(ISNULL(p.ValorRetorno, 0) AS FLOAT) AS ValorRetorno, " +
                            " CAST(ISNULL(pe.RecebidoDinheiro, 0) AS FLOAT) AS RecebidoDinheiro, " +
                            " p.DataInclusao AS DataInclusao, " +
                            " RTRIM(ISNULL(p.Obs, '')) AS Obs, " +
                            " p.Conferido AS Conferido " +
                            " FROM PizzaByte.PedidosEntregas AS p " +
                            " INNER JOIN PizzaByte.ClientesEnderecos AS ce ON p.IdEndereco = ce.Id" +
                            " INNER JOIN PizzaByte.Ceps AS c ON ce.IdCep = c.Id" +
                            " INNER JOIN PizzaByte.Pedidos AS pe ON p.IdPedido = pe.Id" +
                            " INNER JOIN PizzaByte.Funcionarios AS f ON p.IdFuncionario = f.Id";

            List<SqlParameter> listaFiltros = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(requisicaoDto.Endereco))
            {
                query += " AND c.Logradouro LIKE '%' + @endereco + '%'";
                listaFiltros.Add(new SqlParameter("endereco", requisicaoDto.Endereco.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.Funcionario))
            {
                query += " AND f.Nome LIKE '%' + @funcionario + '%'";
                listaFiltros.Add(new SqlParameter("funcionario", requisicaoDto.Funcionario.Trim()));
            }

            if (requisicaoDto.ValorRetorno > 0)
            {
                query += " AND p.ValorRetorno >= @valorRetorno  ";
                listaFiltros.Add(new SqlParameter("valorRetorno", requisicaoDto.ValorInicio));
            }

            if (requisicaoDto.ValorInicio > 0)
            {
                query += " AND ValorInicio >= @valorInicio  ";
                listaFiltros.Add(new SqlParameter("valorInicio", requisicaoDto.ValorInicio));
            }

            if (requisicaoDto.ValorFim > 0)
            {
                query += " AND ValorFim <= @valorFim  ";
                listaFiltros.Add(new SqlParameter("valorFim", requisicaoDto.ValorFim));
            }

            if (requisicaoDto.DataCadastroInicial.HasValue)
            {
                query += " AND CAST(DataInclusao AS Date) >= @dataInicio";
                listaFiltros.Add(new SqlParameter("dataInicio", requisicaoDto.DataCadastroInicial.Value));
            }

            if (requisicaoDto.DataCadastroFinal.HasValue)
            {
                query += " AND CAST(DataInclusao AS Date) <= @dataFim";
                listaFiltros.Add(new SqlParameter("dataFim", requisicaoDto.DataCadastroFinal.Value));
            }

            // Ordenar query
            switch (requisicaoDto.CampoOrdem)
            {
                case "ENDERECO":
                default:
                    retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Logradouro).ToList();
                    break;

                case "FUNCIONARIO":
                    retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.NomeFuncionario).ToList();
                    break;

                case "VALORRETORNO":
                    retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.ValorRetorno).ToList();
                    break;
            }

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<ListagemEntregaDto>(query, listaFiltros.ToArray()).ToList();

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioListagemEntregas, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtem a lista dos melhores produtos
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterMelhoresProdutos(RequisicaoObterMelhoresProdutosDto requisicaoDto, ref RetornoObterListaDto<MelhoresCadastrosDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioMelhoresProdutos, Guid.Empty, mensagemErro);
                return false;

            }

            // Validar o período de pesquisa
            if (!requisicaoDto.DataCadastroInicial.HasValue || !requisicaoDto.DataCadastroFinal.HasValue)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Por favor, preencha o período de vendas que deseja pesquisar";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioMelhoresProdutos, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.DataCadastroInicial.Value > requisicaoDto.DataCadastroFinal.Value)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Período de pesquisa inválido! Por favor, preencha o período corretamente.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioMelhoresProdutos, Guid.Empty, mensagemErro);
                return false;
            }

            string where = " AND CAST(prod.DataInclusao AS Date) BETWEEN @inicio AND @fim ";

            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            listaFiltros.Add(new SqlParameter("idProdutoBrinde", UtilitarioBll.RetornaIdProdutoPromocao()));
            listaFiltros.Add(new SqlParameter("inicio", requisicaoDto.DataCadastroInicial.Value.Date));
            listaFiltros.Add(new SqlParameter("fim", requisicaoDto.DataCadastroFinal.Value.Date));

            if (requisicaoDto.Tipo != TipoProduto.NaoIdentificado)
            {
                where += " AND prod.TipoProduto = @tipo ";
                listaFiltros.Add(new SqlParameter("tipo", (int)requisicaoDto.Tipo));
            }

            string query = "SELECT SUM(itens.Quantidade) AS Quantidade, " +
                                  "RTRIM(p.Descricao) AS Descricao, " +
                                  "SUM(itens.Valor) AS Valor " +
                                  "FROM(" +
                                        "SELECT CASE WHEN prod.IdProdutoComposto IS NULL THEN prod.Quantidade " +
                                                "ELSE(prod.Quantidade / 2) END AS Quantidade, " +
                                                "CASE WHEN prod.IdProdutoComposto IS NULL THEN prod.PrecoProduto " +
                                                "ELSE(prod.PrecoProduto / 2) END AS Valor, " +
                                                "prod.IdProduto AS Id " +
                                        "FROM PizzaByte.PedidosItens AS prod " +
                                        "WHERE prod.Inativo = 0 AND prod.Excluido = 0 " +
                                        "AND prod.IdProduto != @idProdutoBrinde" +
                                        where +
                                  "UNION ALL " +
                                        "SELECT(comp.Quantidade / 2) AS Quantidade, " +
                                               "(comp.PrecoProduto / 2) AS Valor, " +
                                               "comp.IdProdutoComposto AS Id " +
                                        "FROM PizzaByte.PedidosItens AS comp " +
                                        "WHERE comp.IdProdutoComposto IS NOT NULL " +
                                        "AND comp.Inativo = 0 AND comp.Excluido = 0 " +
                                        where.Replace("prod", "comp") +
                                 ") AS itens " +
                                 "LEFT JOIN PizzaByte.Produtos AS p ON itens.Id = p.Id AND p.Excluido = 0 " +
                                 "GROUP BY itens.Id, p.Descricao";

            // Ordenar query
            switch (requisicaoDto.CampoOrdem)
            {
                case "QUANTIDADE":
                default:
                    query += " ORDER BY Quantidade DESC";
                    break;

                case "VALOR":
                    query += " ORDER BY Valor DESC";
                    break;

                case "DESCRICAO":
                    query += " ORDER BY p.Descricao";
                    break;
            }

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<MelhoresCadastrosDto>(query, listaFiltros.ToArray()).ToList();

                retornoDto.ListaEntidades.ForEach(p => p.Descricao = string.IsNullOrWhiteSpace(p.Descricao) ? "Produto não encontrado" : p.Descricao);
                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioMelhoresProdutos, Guid.Empty, mensagemErro);
                return false;
            }

        }

        /// <summary>
        /// Obtem a lista dos melhores produtos
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterMelhoresClientes(RequisicaoObterMelhoresClientesDto requisicaoDto, ref RetornoObterListaDto<MelhoresCadastrosDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioMelhoresClientes, Guid.Empty, mensagemErro);
                return false;

            }

            // Validar o período de pesquisa
            if (!requisicaoDto.DataCadastroInicial.HasValue || !requisicaoDto.DataCadastroFinal.HasValue)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Por favor, preencha o período que deseja pesquisar";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioMelhoresClientes, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.DataCadastroInicial.Value > requisicaoDto.DataCadastroFinal.Value)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Período de pesquisa inválido! Por favor, preencha o período corretamente.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioMelhoresClientes, Guid.Empty, mensagemErro);
                return false;
            }

            string join = "";
            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            if (!string.IsNullOrWhiteSpace(requisicaoDto.Nome))
            {
                join += " AND c.Nome LIKE '%' + @nomeCliente + '%'";
                listaFiltros.Add(new SqlParameter("nomeCliente", requisicaoDto.Nome.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.Telefone))
            {
                join += " AND c.Telefone LIKE '%' + @telefone + '%'";
                listaFiltros.Add(new SqlParameter("telefone", requisicaoDto.Telefone.Trim()));
            }

            string query = "SELECT c.Nome AS Descricao," +
                " RTRIM(ISNULL(c.Telefone, '')) AS Complemento," +
                " SUM (p.total) AS Valor, " +
                " SUM (i.quantidade) AS Quantidade" +
                " FROM PizzaByte.Pedidos AS p " +
                " LEFT JOIN PizzaByte.Clientes AS c ON p.IdCliente = c.Id AND c.excluido = 0 AND c.inativo = 0" + join +
                " INNER JOIN PizzaByte.PedidosItens AS i ON p.id = i.idPedido AND i.excluido = 0 AND i.inativo = 0" +
                " WHERE p.excluido = 0 AND CAST(p.DataInclusao AS Date) BETWEEN @inicio AND @fim" +
                " GROUP BY p.idCliente, c.Nome, c.Telefone";

            listaFiltros.Add(new SqlParameter("inicio", requisicaoDto.DataCadastroInicial.Value.Date));
            listaFiltros.Add(new SqlParameter("fim", requisicaoDto.DataCadastroFinal.Value.Date));

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<MelhoresCadastrosDto>(query, listaFiltros.ToArray()).ToList();

                retornoDto.ListaEntidades.ForEach(p => p.Descricao = string.IsNullOrWhiteSpace(p.Descricao) ? "Cliente não encontrado" : p.Descricao);
                retornoDto.ListaEntidades.ForEach(p => p.Complemento = UtilitarioBll.RetornarTelefoneFormatado(p.Complemento));

                // Ordenar query
                switch (requisicaoDto.CampoOrdem)
                {
                    case "NOME":
                    default:
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Descricao).ToList();
                        break;

                    case "TELEFONE":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Complemento).ToList();
                        break;

                    case "VALOR":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Valor).ToList();
                        break;

                    case "QUANTIDADE":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Quantidade).ToList();
                        break;
                }

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioMelhoresClientes, Guid.Empty, mensagemErro);
                return false;
            }

        }

        /// <summary>
        /// Obtem a lista dos melhores motoboys
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterMelhoresMotoboys(RequisicaoObterMelhoresMotoboysDto requisicaoDto, ref RetornoObterListaDto<MelhoresCadastrosDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioMelhoresMotoboys, Guid.Empty, mensagemErro);
                return false;

            }

            // Validar o período de pesquisa
            if (!requisicaoDto.DataCadastroInicial.HasValue || !requisicaoDto.DataCadastroFinal.HasValue)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Por favor, preencha o período que deseja pesquisar";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioMelhoresMotoboys, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.DataCadastroInicial.Value > requisicaoDto.DataCadastroFinal.Value)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Período de pesquisa inválido! Por favor, preencha o período corretamente.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioMelhoresMotoboys, Guid.Empty, mensagemErro);
                return false;
            }

            string join = "";
            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            if (!string.IsNullOrWhiteSpace(requisicaoDto.Nome))
            {
                join += " AND c.Nome LIKE '%' + @nomeMotoboy + '%'";
                listaFiltros.Add(new SqlParameter("nomeMotoboy", requisicaoDto.Nome.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(requisicaoDto.Telefone))
            {
                join += " AND c.Telefone LIKE '%' + @telefone + '%'";
                listaFiltros.Add(new SqlParameter("telefone", requisicaoDto.Telefone.Trim()));
            }

            string query = " SELECT f.Nome AS Descricao," +
                           " RTRIM(ISNULL(f.Telefone, '')) AS Complemento," +
                           " SUM (p.TaxaEntrega) AS Valor," +
                           " CAST (COUNT(e.Id) AS FLOAT) AS Quantidade " +
                           " FROM PizzaByte.PedidosEntregas AS e " +
                           " INNER JOIN PizzaByte.Funcionarios AS f ON e.IdFuncionario = f.Id AND f.Excluido = 0 AND f.Inativo = 0 " +
                           " INNER JOIN PizzaByte.Pedidos AS p ON e.IdPedido = p.Id AND p.Excluido = 0 AND p.Inativo = 0 " +
                           " WHERE e.Excluido = 0 AND e.Inativo = 0 " +
                           " GROUP BY e.IdFuncionario, f.Nome, f.Telefone";


            listaFiltros.Add(new SqlParameter("inicio", requisicaoDto.DataCadastroInicial.Value.Date));
            listaFiltros.Add(new SqlParameter("fim", requisicaoDto.DataCadastroFinal.Value.Date));

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<MelhoresCadastrosDto>(query, listaFiltros.ToArray()).ToList();

                retornoDto.ListaEntidades.ForEach(p => p.Descricao = string.IsNullOrWhiteSpace(p.Descricao) ? "Motoboy não encontrado" : p.Descricao);
                retornoDto.ListaEntidades.ForEach(p => p.Complemento = UtilitarioBll.RetornarTelefoneFormatado(p.Complemento));

                // Ordenar query
                switch (requisicaoDto.CampoOrdem)
                {
                    case "NOME":
                    default:
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Descricao).ToList();
                        break;

                    case "VALOR":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Valor).ToList();
                        break;

                    case "TELEFONE":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Complemento).ToList();
                        break;

                    case "QUANTIDADE":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Quantidade).ToList();
                        break;
                }

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioMelhoresMotoboys, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtém a relação de contas
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterRelacaoDiariaContas(RequisicaoObterRelacaoContasDto requisicaoDto, ref RetornoObterListaDto<RelacaoContasDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioRelacaoDiariaContas, Guid.Empty, mensagemErro);
                return false;
            }

            // Validar o período de pesquisa
            if (!requisicaoDto.DataCadastroInicial.HasValue || !requisicaoDto.DataCadastroFinal.HasValue)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Por favor, preencha o período que deseja pesquisar";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioRelacaoDiariaContas, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.DataCadastroInicial.Value > requisicaoDto.DataCadastroFinal.Value)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Período de pesquisa inválido! Por favor, preencha o período corretamente.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioRelacaoDiariaContas, Guid.Empty, mensagemErro);
                return false;
            }

            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            listaFiltros.Add(new SqlParameter("inicio", requisicaoDto.DataCadastroInicial.Value.Date));
            listaFiltros.Add(new SqlParameter("fim", requisicaoDto.DataCadastroFinal.Value.Date));

            string campoPesquisaPagar = "";
            string campoPesquisaReceber = "";
            switch (requisicaoDto.PesquisarPor)
            {

                case "DATAVENCIMENTO":
                    campoPesquisaPagar = "DataVencimento";
                    campoPesquisaReceber = "DataVencimento";
                    break;

                case "DATACOMPETENCIA":
                    campoPesquisaPagar = "DataCompetencia";
                    campoPesquisaReceber = "DataCompetencia";
                    break;

                case "DATAPAGAMENTO":
                    campoPesquisaPagar = "DataPagamento";
                    campoPesquisaReceber = "DataVencimento";
                    break;

                default:
                    campoPesquisaPagar = "DataVencimento";
                    campoPesquisaReceber = "DataVencimento";
                    break;
            }

            string where = " cp.Excluido = 0 ";
            if (requisicaoDto.Status != StatusConta.NaoIdentificado)
            {
                where += " AND cp.Status = @status";
                listaFiltros.Add(new SqlParameter("status", requisicaoDto.Status));
            }
            else
            {
                if (!requisicaoDto.IndicadorEstornadas)
                {
                    where += " AND cp.Status != @estorno";
                    listaFiltros.Add(new SqlParameter("estorno", StatusConta.Estornada));
                }

                if (!requisicaoDto.IndicadorPerdida)
                {
                    where += " AND cp.Status != @perdida";
                    listaFiltros.Add(new SqlParameter("perdida", StatusConta.Perdida));
                }
            }

            string query = "SELECT resultado.DataOriginal," +
                          " SUM(resultado.ValorPagar) AS ValorPagar," +
                          " SUM(resultado.ValorReceber) AS ValorReceber" +
                          " FROM (SELECT" +
                         $" cp.{campoPesquisaPagar} AS DataOriginal," +
                         $" SUM(cp.Valor) AS ValorPagar," +
                         $" 0 AS ValorReceber" +
                         $" FROM PizzaByte.ContaPagar AS cp WHERE " + where +
                         $" AND CAST(cp.{campoPesquisaPagar} AS Date) BETWEEN @inicio AND @fim" +
                         $" GROUP BY cp.{campoPesquisaPagar}" +
                         $" UNION ALL" +
                         $" SELECT " +
                         $" cr.{campoPesquisaReceber} AS DataOriginal," +
                         $" 0 AS ValorPagar," +
                         $" SUM(cr.Valor) AS ValorReceber" +
                         $" FROM PizzaByte.ContaReceber AS cr WHERE {where.Replace("cp.", "cr.")}" +
                         $" AND CAST(cr.{campoPesquisaReceber} AS Date) BETWEEN @inicio AND @fim" +
                         $" GROUP BY cr.{campoPesquisaReceber}) AS resultado" +
                         $" GROUP BY resultado.DataOriginal" +
                         $" ORDER BY resultado.DataOriginal";
            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<RelacaoContasDto>(query, listaFiltros.ToArray()).ToList();

                retornoDto.ListaEntidades.ForEach(p => p.Data = p.DataOriginal.ToString("dd/MM/yyyy"));

                double valorAcumulado = 0;
                for (int i = 0; i < retornoDto.ListaEntidades.Count; i++)
                {
                    valorAcumulado += retornoDto.ListaEntidades[i].ValorReceber - retornoDto.ListaEntidades[i].ValorPagar;
                    retornoDto.ListaEntidades[i].Saldo = valorAcumulado;
                }

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioRelacaoDiariaContas, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtém a relação de contas
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterRelacaoMensalContas(RequisicaoObterRelacaoContasDto requisicaoDto, ref RetornoObterListaDto<RelacaoContasDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioRelacaoDiariaContas, Guid.Empty, mensagemErro);
                return false;
            }

            // Validar o período de pesquisa
            if (!requisicaoDto.DataCadastroInicial.HasValue || !requisicaoDto.DataCadastroFinal.HasValue)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Por favor, preencha o período que deseja pesquisar";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioRelacaoDiariaContas, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.DataCadastroInicial.Value > requisicaoDto.DataCadastroFinal.Value)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Período de pesquisa inválido! Por favor, preencha o período corretamente.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioRelacaoDiariaContas, Guid.Empty, mensagemErro);
                return false;
            }

            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            listaFiltros.Add(new SqlParameter("inicio", requisicaoDto.DataCadastroInicial.Value.Date));
            listaFiltros.Add(new SqlParameter("fim", requisicaoDto.DataCadastroFinal.Value.Date));

            string campoPesquisaPagar = "";
            string campoPesquisaReceber = "";
            switch (requisicaoDto.PesquisarPor)
            {
                case "DATAVENCIMENTO":
                    campoPesquisaPagar = "DataVencimento";
                    campoPesquisaReceber = "DataVencimento";
                    break;

                case "DATACOMPETENCIA":
                    campoPesquisaPagar = "DataCompetencia";
                    campoPesquisaReceber = "DataCompetencia";
                    break;

                case "DATAPAGAMENTO":
                    campoPesquisaPagar = "DataPagamento";
                    campoPesquisaReceber = "DataVencimento";
                    break;

                default:
                    campoPesquisaPagar = "DataVencimento";
                    campoPesquisaReceber = "DataVencimento";
                    break;
            }

            string where = " cp.Excluido = 0 ";
            if (requisicaoDto.Status != StatusConta.NaoIdentificado)
            {
                where += " AND cp.Status = @status";
                listaFiltros.Add(new SqlParameter("status", requisicaoDto.Status));
            }
            else
            {
                if (!requisicaoDto.IndicadorEstornadas)
                {
                    where += " AND cp.Status != @estorno";
                    listaFiltros.Add(new SqlParameter("estorno", StatusConta.Estornada));
                }

                if (!requisicaoDto.IndicadorPerdida)
                {
                    where += " AND cp.Status != @perdida";
                    listaFiltros.Add(new SqlParameter("perdida", StatusConta.Perdida));
                }
            }

            string query = $"SELECT resultado.Data," +
                            $" ROUND(SUM(resultado.ValorPagar), 2) AS ValorPagar," +
                            $" ROUND(SUM(resultado.ValorReceber), 2) AS ValorReceber" +
                            $" FROM (" +
                            $"   SELECT" +
                            $"     MONTH(cp.{campoPesquisaPagar}) AS Mes," +
                            $"     YEAR(cp.{campoPesquisaPagar}) AS Ano," +
                            $"     CASE MONTH(cp.{campoPesquisaPagar})" +
                            $"     WHEN 1 THEN 'Janeiro de ' + CAST(YEAR(cp.{campoPesquisaPagar}) AS varchar(4))" +
                            $"     WHEN 2 THEN 'Fevereiro de ' + CAST(YEAR(cp.{campoPesquisaPagar}) AS varchar(4))" +
                            $"     WHEN 3 THEN 'Março de ' + CAST(YEAR(cp.{campoPesquisaPagar}) AS varchar(4))" +
                            $"     WHEN 4 THEN 'Abril de ' + CAST(YEAR(cp.{campoPesquisaPagar}) AS varchar(4))" +
                            $"     WHEN 5 THEN 'Maio de ' + CAST(YEAR(cp.{campoPesquisaPagar}) AS varchar(4))" +
                            $"     WHEN 6 THEN 'Junho de ' + CAST(YEAR(cp.{campoPesquisaPagar}) AS varchar(4))" +
                            $"     WHEN 7 THEN 'Julho de ' + CAST(YEAR(cp.{campoPesquisaPagar}) AS varchar(4))" +
                            $"     WHEN 8 THEN 'Agosto de ' + CAST(YEAR(cp.{campoPesquisaPagar}) AS varchar(4))" +
                            $"     WHEN 9 THEN 'Setembro de ' + CAST(YEAR(cp.{campoPesquisaPagar}) AS varchar(4))" +
                            $"     WHEN 10 THEN 'Outubro de ' + CAST(YEAR(cp.{campoPesquisaPagar}) AS varchar(4))" +
                            $"     WHEN 11 THEN 'Novembro de ' + CAST(YEAR(cp.{campoPesquisaPagar}) AS varchar(4))" +
                            $"     ELSE 'Dezembro de ' + CAST(YEAR(cp.{campoPesquisaPagar}) AS varchar(4)) END AS Data," +
                            $"   SUM(cp.Valor) AS ValorPagar," +
                            $"   0 AS ValorReceber" +
                            $"   FROM PizzaByte.ContaPagar AS cp WHERE " + where +
                            $"   AND CAST(cp.{campoPesquisaPagar} AS Date) BETWEEN @inicio AND @fim" +
                            $"   GROUP BY YEAR(cp.{campoPesquisaPagar}), MONTH(cp.{campoPesquisaPagar})" +
                            $"   UNION ALL" +
                            $"   SELECT" +
                            $"   MONTH(cr.{campoPesquisaReceber}) AS Mes," +
                            $"   YEAR(cr.{campoPesquisaReceber}) AS Ano," +
                            $"   CASE MONTH(cr.{campoPesquisaReceber})" +
                            $"     WHEN 1 THEN 'Janeiro de ' + CAST(YEAR(cr.{campoPesquisaReceber}) AS varchar(4))" +
                            $"     WHEN 2 THEN 'Fevereiro de '+ CAST(YEAR(cr.{campoPesquisaReceber}) AS varchar(4))" +
                            $"     WHEN 3 THEN 'Março de '+ CAST(YEAR(cr.{campoPesquisaReceber}) AS varchar(4))" +
                            $"     WHEN 4 THEN 'Abril de '+ CAST(YEAR(cr.{campoPesquisaReceber}) AS varchar(4))" +
                            $"     WHEN 5 THEN 'Maio de '+ CAST(YEAR(cr.{campoPesquisaReceber}) AS varchar(4))" +
                            $"     WHEN 6 THEN 'Junho de '+ CAST(YEAR(cr.{campoPesquisaReceber}) AS varchar(4))" +
                            $"     WHEN 7 THEN 'Julho de '+ CAST(YEAR(cr.{campoPesquisaReceber}) AS varchar(4))" +
                            $"     WHEN 8 THEN 'Agosto de '+ CAST(YEAR(cr.{campoPesquisaReceber}) AS varchar(4))" +
                            $"     WHEN 9 THEN 'Setembro de '+ CAST(YEAR(cr.{campoPesquisaReceber}) AS varchar(4))" +
                            $"     WHEN 10 THEN 'Outubro de '+ CAST(YEAR(cr.{campoPesquisaReceber}) AS varchar(4))" +
                            $"     WHEN 11 THEN 'Novembro de '+ CAST(YEAR(cr.{campoPesquisaReceber}) AS varchar(4))" +
                            $"     ELSE 'Dezembro de '+ CAST(YEAR(cr.{campoPesquisaReceber}) AS varchar(4)) END AS Data," +
                            $"   0 AS ValorPagar," +
                            $"   SUM(cr.Valor) AS ValorReceber" +
                            $"   FROM PizzaByte.ContaReceber AS cr WHERE {where.Replace("cp.", "cr.")}" +
                            $"   AND CAST(cr.{campoPesquisaReceber} AS Date) BETWEEN @inicio AND @fim" +
                            $"   GROUP BY YEAR(cr.{campoPesquisaReceber}), MONTH(cr.{campoPesquisaReceber})" +
                            $") AS resultado" +
                            $" GROUP BY resultado.Data, resultado.Ano, resultado.Mes" +
                            $" ORDER BY resultado.Ano, resultado.Mes";
            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<RelacaoContasDto>(query, listaFiltros.ToArray()).ToList();

                double valorAcumulado = 0;
                for (int i = 0; i < retornoDto.ListaEntidades.Count; i++)
                {
                    valorAcumulado += retornoDto.ListaEntidades[i].ValorReceber - retornoDto.ListaEntidades[i].ValorPagar;
                    retornoDto.ListaEntidades[i].Saldo = valorAcumulado;
                }

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioRelacaoDiariaContas, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Busca as contas totalizadas por fornecedor
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterContasPorFornecedor(RequisicaoObterRelacaoContasDto requisicaoDto, ref RetornoObterListaDto<ContasPorFornecedorDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioContasPorFornecedor, Guid.Empty, mensagemErro);
                return false;
            }

            // Validar o período de pesquisa
            if (!requisicaoDto.DataCadastroInicial.HasValue || !requisicaoDto.DataCadastroFinal.HasValue)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Por favor, preencha o período que deseja pesquisar";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioContasPorFornecedor, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.DataCadastroInicial.Value > requisicaoDto.DataCadastroFinal.Value)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Período de pesquisa inválido! Por favor, preencha o período corretamente.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioContasPorFornecedor, Guid.Empty, mensagemErro);
                return false;
            }

            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            listaFiltros.Add(new SqlParameter("inicio", requisicaoDto.DataCadastroInicial.Value.Date));
            listaFiltros.Add(new SqlParameter("fim", requisicaoDto.DataCadastroFinal.Value.Date));

            string campoPesquisaPagar = "";
            switch (requisicaoDto.PesquisarPor)
            {

                case "DATAVENCIMENTO":
                    campoPesquisaPagar = "DataVencimento";
                    break;

                case "DATACOMPETENCIA":
                    campoPesquisaPagar = "DataCompetencia";
                    break;

                case "DATAPAGAMENTO":
                    campoPesquisaPagar = "DataPagamento";
                    break;

                default:
                    campoPesquisaPagar = "DataVencimento";
                    break;
            }

            string where = " c.Excluido = 0 ";
            if (requisicaoDto.Status != StatusConta.NaoIdentificado)
            {
                where += " AND c.Status = @status";
                listaFiltros.Add(new SqlParameter("status", requisicaoDto.Status));
            }
            else
            {
                if (!requisicaoDto.IndicadorEstornadas)
                {
                    where += " AND c.Status != @estorno";
                    listaFiltros.Add(new SqlParameter("estorno", StatusConta.Estornada));
                }

                if (!requisicaoDto.IndicadorPerdida)
                {
                    where += " AND c.Status != @perdida";
                    listaFiltros.Add(new SqlParameter("perdida", StatusConta.Perdida));
                }
            }

            string select = "SELECT" +
                " RTRIM(ISNULL(c.Descricao, '')) AS Descricao," +
                " CAST(ISNULL(c.Valor, 0) AS FLOAT) AS Valor," +
                " c.IdFornecedor AS IdFornecedor," +
                " c.DataVencimento AS DataVencimento," +
                " c.DataPagamento AS DataPagamento," +
                " c.Status AS Status," +
                " RTRIM(ISNULL(f.NomeFantasia, '')) AS NomeFantasia" +
                " FROM PizzaByte.ContaPagar AS c" +
                " LEFT JOIN PizzaByte.Fornecedores AS f ON c.IdFornecedor = f.Id AND f.Excluido = 0" +
                " WHERE " + where + $" AND CAST(c.{campoPesquisaPagar} AS Date) BETWEEN @inicio AND @fim";

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<ContasPorFornecedorDto>(select, listaFiltros.ToArray()).ToList();

                retornoDto.ListaEntidades.ForEach(p => p.NomeFantasia = (p.IdFornecedor == Guid.Empty || p.IdFornecedor == null) ? "Não identificado" : p.NomeFantasia);
                retornoDto.ListaEntidades.ForEach(p => p.NomeFantasia = string.IsNullOrWhiteSpace(p.NomeFantasia) ? "Não encontrado" : p.NomeFantasia.Trim());

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioRelacaoDiariaContas, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtem os dados para emissão do relatório de listagem de pedidos
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterPedidosPorBairro(RequisicaoObterListagemPedidosDto requisicaoDto, ref RetornoObterListaDto<PedidosPorBairroDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioPedidosPorBairro, Guid.Empty, mensagemErro);
                return false;
            }

            // Validar o período de pesquisa
            if (!requisicaoDto.DataCadastroInicial.HasValue || !requisicaoDto.DataCadastroFinal.HasValue)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Por favor, preencha o período de vendas que deseja pesquisar";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioPedidosPorBairro, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.DataCadastroInicial.Value > requisicaoDto.DataCadastroFinal.Value)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Período de pesquisa inválido! Por favor, preencha o período corretamente.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioPedidosPorBairro, Guid.Empty, mensagemErro);
                return false;
            }

            string query = " WHERE CAST(p.DataInclusao AS Date) BETWEEN @inicio AND @fim AND p.Excluido = 0 ";

            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            listaFiltros.Add(new SqlParameter("inicio", requisicaoDto.DataCadastroInicial.Value.Date));
            listaFiltros.Add(new SqlParameter("fim", requisicaoDto.DataCadastroFinal.Value.Date));

            if (!RetornarWherePedidos(requisicaoDto, ref query, ref listaFiltros))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Erro ao filtrar os pedidos: " + query;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioPedidosPorBairro, Guid.Empty, mensagemErro);
                return false;
            }

            query = "SELECT " +
                    " RTRIM(ISNULL(c.Bairro, '')) AS Bairro," +
                    " RTRIM(ISNULL(c.Cidade, '')) AS Cidade," +
                    " SUM(ISNULL(p.TaxaEntrega, 0)) AS TaxaEntrega," +
                    " SUM(ISNULL(p.Total, 0)) AS Valor" +
                    " FROM PizzaByte.Pedidos AS p" +
                    " INNER JOIN PizzaByte.PedidosEntregas AS e ON p.Id = e.IdPedido AND e.Excluido = 0 AND e.Inativo = p.Inativo" +
                    " INNER JOIN PizzaByte.ClientesEnderecos AS ce ON e.IdEndereco = ce.Id AND ce.Excluido = 0 " +
                    " INNER JOIN PizzaByte.Ceps AS c ON  ce.IdCep = c.Id AND c.Excluido = 0"
                     + query +
                    " GROUP BY c.Bairro, c.Cidade";
            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<PedidosPorBairroDto>(query, listaFiltros.ToArray()).ToList();

                // Ordenar query
                switch (requisicaoDto.CampoOrdem)
                {
                    case "BAIRRO":
                    default:
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Bairro).ToList();
                        break;

                    case "TAXAENTREGA":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.TaxaEntrega).ThenBy(p => p.Bairro).ToList();
                        break;

                    case "VALORCRESCENTE":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Valor).ThenBy(p => p.Bairro).ToList();
                        break;

                    case "CIDADE":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Cidade).ThenBy(p => p.Valor).ToList();
                        break;

                    case "VALORDECRESCENTE":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderByDescending(p => p.Valor).ThenBy(p => p.Bairro).ToList();
                        break;

                    case "TAXAENTREGADECRESCENTE":
                        retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderByDescending(p => p.TaxaEntrega).ThenBy(p => p.Bairro).ToList();
                        break;
                }

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioPedidosPorBairro, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtem os dados para emissão do relatório de listagem de pedidos
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterEvolucaoSemanalPedidos(RequisicaoObterListagemPedidosDto requisicaoDto, ref RetornoObterListaDto<ProdutosVendidosPorDiaSemanaDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoSemanalPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            // Validar o período de pesquisa
            if (!requisicaoDto.DataCadastroInicial.HasValue || !requisicaoDto.DataCadastroFinal.HasValue)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Por favor, preencha o período de vendas que deseja pesquisar";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoSemanalPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.DataCadastroInicial.Value > requisicaoDto.DataCadastroFinal.Value)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Período de pesquisa inválido! Por favor, preencha o período corretamente.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoSemanalPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            string query = " WHERE CAST(p.DataInclusao AS Date) BETWEEN @inicio AND @fim AND p.Excluido = 0 ";

            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            listaFiltros.Add(new SqlParameter("inicio", requisicaoDto.DataCadastroInicial.Value.Date));
            listaFiltros.Add(new SqlParameter("fim", requisicaoDto.DataCadastroFinal.Value.Date));

            if (!RetornarWherePedidos(requisicaoDto, ref query, ref listaFiltros))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Erro ao filtrar os pedidos: " + query;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoSemanalPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            query = "SELECT CASE WHEN DATEPART(weekday, p.DataInclusao) = 2 THEN 'Seg'" +
                    " WHEN DATEPART(weekday, p.DataInclusao) = 3 THEN 'Ter'" +
                    " WHEN DATEPART(weekday, p.DataInclusao) = 4 THEN 'Qua'" +
                    " WHEN DATEPART(weekday, p.DataInclusao) = 5 THEN 'Qui'" +
                    " WHEN DATEPART(weekday, p.DataInclusao) = 6 THEN 'Sex'" +
                    " WHEN DATEPART(weekday, p.DataInclusao) = 7 THEN 'Sáb'" +
                    " ELSE 'Dom' END AS DiaSemana," +
                    " CAST(SUM(CASE WHEN i.TipoProduto = 1 THEN i.Quantidade ELSE 0 END) AS int) AS Pizza," +
                    " CAST(SUM(CASE WHEN i.TipoProduto = 2 THEN i.Quantidade ELSE 0 END) AS int) AS Bebida" +
                    " FROM PizzaByte.Pedidos AS p" +
                    " INNER JOIN PizzaByte.PedidosItens AS i ON p.Id = i.IdPedido AND i.Excluido = 0"
                     + query +
                    " GROUP BY DATEPART(weekday, p.DataInclusao)";
            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                var resultado = contexto.Database.SqlQuery<ProdutosVendidosPorDiaSemanaDto>(query, listaFiltros.ToArray()).ToList();

                // Preencher todos os dias da semana
                for (int i = (int)DateTime.Now.DayOfWeek; retornoDto.ListaEntidades.Count() < 7; i++)
                {
                    ProdutosVendidosPorDiaSemanaDto produtos = resultado.Where(p => p.DiaSemana.Trim() == UtilitarioBll.RetornaDiaSemana(i)).FirstOrDefault();
                    retornoDto.ListaEntidades.Insert(0, new ProdutosVendidosPorDiaSemanaDto()
                    {
                        DiaSemana = UtilitarioBll.RetornaDiaSemana(i),
                        Bebida = (produtos != null) ? produtos.Bebida : 0,
                        Pizza = (produtos != null) ? produtos.Pizza : 0,
                    });

                    if (i == 6)
                    {
                        i = -1;
                    }
                }

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoSemanalPedidos, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtem os dados para emissão do relatório de listagem de pedidos
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterEvolucaoMensalPedidos(RequisicaoObterListagemPedidosDto requisicaoDto, ref RetornoObterListaDto<ProdutosVendidosPorMesDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoMensalPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            // Validar o período de pesquisa
            if (!requisicaoDto.DataCadastroInicial.HasValue || !requisicaoDto.DataCadastroFinal.HasValue)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Por favor, preencha o período de vendas que deseja pesquisar";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoMensalPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.DataCadastroInicial.Value > requisicaoDto.DataCadastroFinal.Value)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Período de pesquisa inválido! Por favor, preencha o período corretamente.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoMensalPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            string query = " WHERE CAST(p.DataInclusao AS Date) BETWEEN @inicio AND @fim AND p.Excluido = 0 ";

            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            listaFiltros.Add(new SqlParameter("inicio", requisicaoDto.DataCadastroInicial.Value.Date));
            listaFiltros.Add(new SqlParameter("fim", requisicaoDto.DataCadastroFinal.Value.Date));

            if (!RetornarWherePedidos(requisicaoDto, ref query, ref listaFiltros))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Erro ao filtrar os pedidos: " + query;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoMensalPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            query = "SELECT " +
                    "(CASE MONTH(p.DataInclusao) WHEN 1 THEN 'Jan/' " +
                    "WHEN 2 THEN 'Fev/' " +
                    "WHEN 3 THEN 'Mar/' " +
                    "WHEN 4 THEN 'Abr/' " +
                    "WHEN 5 THEN 'Mai/' " +
                    "WHEN 6 THEN 'Jun/' " +
                    "WHEN 7 THEN 'Jul/' " +
                    "WHEN 8 THEN 'Ago/' " +
                    "WHEN 9 THEN 'Set/' " +
                    "WHEN 10 THEN 'Out/' " +
                    "WHEN 11 THEN 'Nov/' " +
                    "ELSE 'Dez/' END) + CONVERT(VARCHAR(4), YEAR(p.DataInclusao)) AS Mes, " +
                    "CAST(SUM(CASE WHEN i.TipoProduto = 1 THEN i.Quantidade ELSE 0 END) AS int) AS Pizza, " +
                    "CAST(SUM(CASE WHEN i.TipoProduto = 2 THEN i.Quantidade ELSE 0 END) AS int) AS Bebida " +
                    "FROM PizzaByte.Pedidos AS p " +
                    "INNER JOIN PizzaByte.PedidosItens AS i ON p.Id = i.IdPedido AND i.Excluido = 0 "
                     + query +
                    " GROUP BY YEAR(p.DataInclusao), MONTH(p.DataInclusao)" +
                    " ORDER BY YEAR(p.DataInclusao), MONTH(p.DataInclusao)";
            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<ProdutosVendidosPorMesDto>(query, listaFiltros.ToArray()).ToList();

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoMensalPedidos, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtem os dados para emissão do relatório de listagem de pedidos
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterEvolucaoAnualPedidos(RequisicaoObterListagemPedidosDto requisicaoDto, ref RetornoObterListaDto<ProdutosVendidosPorMesDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoAnualPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            // Validar o período de pesquisa
            if (!requisicaoDto.DataCadastroInicial.HasValue || !requisicaoDto.DataCadastroFinal.HasValue)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Por favor, preencha o período de vendas que deseja pesquisar";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoAnualPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.DataCadastroInicial.Value > requisicaoDto.DataCadastroFinal.Value)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Período de pesquisa inválido! Por favor, preencha o período corretamente.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoAnualPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            string query = " WHERE CAST(p.DataInclusao AS Date) BETWEEN @inicio AND @fim AND p.Excluido = 0 ";

            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            listaFiltros.Add(new SqlParameter("inicio", requisicaoDto.DataCadastroInicial.Value.Date));
            listaFiltros.Add(new SqlParameter("fim", requisicaoDto.DataCadastroFinal.Value.Date));

            if (!RetornarWherePedidos(requisicaoDto, ref query, ref listaFiltros))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Erro ao filtrar os pedidos: " + query;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoAnualPedidos, Guid.Empty, mensagemErro);
                return false;
            }

            query = "SELECT " +
                    "CONVERT(VARCHAR(4), YEAR(p.DataInclusao)) AS Mes, " +
                    "CAST(SUM(CASE WHEN i.TipoProduto = 1 THEN i.Quantidade ELSE 0 END) AS int) AS Pizza, " +
                    "CAST(SUM(CASE WHEN i.TipoProduto = 2 THEN i.Quantidade ELSE 0 END) AS int) AS Bebida " +
                    "FROM PizzaByte.Pedidos AS p " +
                    "INNER JOIN PizzaByte.PedidosItens AS i ON p.Id = i.IdPedido AND i.Excluido = 0 "
                     + query +
                    " GROUP BY YEAR(p.DataInclusao)" +
                    " ORDER BY YEAR(p.DataInclusao)";
            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaEntidades = contexto.Database.SqlQuery<ProdutosVendidosPorMesDto>(query, listaFiltros.ToArray()).ToList();

                retornoDto.Retorno = true;
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.RelatorioEvolucaoAnualPedidos, Guid.Empty, mensagemErro);
                return false;
            }
        }

    }
}
