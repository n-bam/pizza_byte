using PizzaByteBll.Base;
using PizzaByteDal;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteVo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteBll
{
    public class MovimentoCaixaBll : BaseBll<MovimentoCaixaVo, MovimentoCaixaDto>
    {
        private bool salvar = true;
        private static LogBll logBll = new LogBll("MovimentoCaixaBll");

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public MovimentoCaixaBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public MovimentoCaixaBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui uma movimentação de caixa no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<MovimentoCaixaDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Converte para VO a ser incluída no banco de dados
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para inclur movimentos de caixa é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirMovimentoCaixa, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            MovimentoCaixaVo movimentoCaixaVo = new MovimentoCaixaVo();
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref movimentoCaixaVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter a movimentação de caixa para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirMovimentoCaixa, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(movimentoCaixaVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter a movimentação de caixa para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirMovimentoCaixa, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirMovimentoCaixa, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirMovimentoCaixa, movimentoCaixaVo.Id, retornoDto.Mensagem);

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui uma movimentação de caixa do banco de dados a partir do ID
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Excluir(RequisicaoObterDto requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Excluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirMovimentoCaixa, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirMovimentoCaixa, requisicaoDto.Id, "Movimento de caixa excluído.");
            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém uma movimentação de caixa pelo ID 
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<MovimentoCaixaDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            MovimentoCaixaVo movimentoCaixaVo;

            if (!ObterPorIdBd(requisicaoDto.Id, out movimentoCaixaVo, ref mensagemErro, true))
            {
                retornoDto.Mensagem = "Erro ao obter o movimentoCaixa: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterMovimentoCaixa, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            MovimentoCaixaDto movimentoCaixaDto = new MovimentoCaixaDto();
            if (!ConverterVoParaDto(movimentoCaixaVo, ref movimentoCaixaDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter a movimentação de caixa: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterMovimentoCaixa, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Entidade = movimentoCaixaDto;
            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte uma movimentação de caixa Dto para uma movimentação de caixa Vo
        /// </summary>
        /// <param name="movimentoCaixaDto"></param>
        /// <param name="movimentoCaixaVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(MovimentoCaixaDto movimentoCaixaDto, ref MovimentoCaixaVo movimentoCaixaVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(movimentoCaixaDto, ref movimentoCaixaVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                movimentoCaixaVo.Justificativa = string.IsNullOrWhiteSpace(movimentoCaixaDto.Justificativa) ? "" : movimentoCaixaDto.Justificativa.Trim();
                movimentoCaixaVo.Valor = movimentoCaixaDto.Valor;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a movimentação de caixa para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte uma movimentação de caixa Dto para uma movimentação de caixa Vo
        /// </summary>
        /// <param name="movimentoCaixaVo"></param>
        /// <param name="movimentoCaixaDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(MovimentoCaixaVo movimentoCaixaVo, ref MovimentoCaixaDto movimentoCaixaDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(movimentoCaixaVo, ref movimentoCaixaDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                movimentoCaixaDto.Justificativa = string.IsNullOrWhiteSpace(movimentoCaixaVo.Justificativa) ? "" : movimentoCaixaVo.Justificativa.Trim();
                movimentoCaixaDto.Valor = movimentoCaixaVo.Valor;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a movimentação de caixa para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém uma lista de movimento de caixas com filtros aplicados, podendo ser paginada
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<MovimentoCaixaDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            IQueryable<MovimentoCaixaVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro, true))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os movimento de caixas: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaMovimentoCaixa, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "JUSTIFICATIVA":
                        query = query.Where(p => p.Justificativa.Contains(filtro.Value));
                        break;

                    case "VALOR":
                        float valor;
                        if (!float.TryParse(filtro.Value, out valor))
                        {
                            retornoDto.Mensagem = $"Houve um problema ao converter o filtro de valor";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaMovimentoCaixa, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Valor == valor);
                        break;

                    case "DATAINCLUSAO":
                        DateTime data;
                        if (!DateTime.TryParse(filtro.Value, out data))
                        {
                            retornoDto.Mensagem = $"Houve um problema ao converter o filtro de data";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaMovimentoCaixa, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataInclusao) == data);
                        break;

                    case "DATAINCLUSAOINICIAL":
                        DateTime dataInicio;
                        if (!DateTime.TryParse(filtro.Value, out dataInicio))
                        {
                            retornoDto.Mensagem = $"Houve um problema ao converter o filtro de data inicial";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaMovimentoCaixa, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataInclusao) >= dataInicio);
                        break;

                    case "DATAINCLUSAOFINAL":
                        DateTime dataFinal;
                        if (!DateTime.TryParse(filtro.Value, out dataFinal))
                        {
                            retornoDto.Mensagem = $"Houve um problema ao converter o filtro de data final";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaMovimentoCaixa, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataInclusao) <= dataFinal);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaMovimentoCaixa, Guid.Empty, retornoDto.Mensagem);
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "JUSTIFICATIVA":
                    query = query.OrderBy(p => p.Justificativa).ThenBy(p => p.DataInclusao).ThenBy(p => p.Valor);
                    break;

                case "VALORCRESCENTE":
                    query = query.OrderBy(p => p.Valor).ThenBy(p => p.DataInclusao).ThenBy(p => p.Justificativa);
                    break;

                case "VALORDECRESCENTE":
                    query = query.OrderByDescending(p => p.Valor).ThenBy(p => p.DataInclusao).ThenBy(p => p.Justificativa);
                    break;

                case "DATAINCLUSAOCRESCENTE":
                    query = query.OrderBy(p => p.DataInclusao).ThenBy(p => p.Justificativa).ThenBy(p => p.Valor);
                    break;

                case "DATAINCLUSAODECRESCENTE":
                    query = query.OrderByDescending(p => p.DataInclusao).ThenBy(p => p.Justificativa).ThenBy(p => p.Valor);
                    break;

                default:
                    query = query.OrderBy(p => p.DataInclusao).ThenBy(p => p.Justificativa).ThenBy(p => p.Valor);
                    break;
            }

            double totalItens = query.Count();
            if (totalItens == 0)
            {
                retornoDto.NumeroPaginas = 0;
                retornoDto.Mensagem = "Nenhum resultado encontrado.";
                retornoDto.Retorno = true;
                return true;
            }

            if (!requisicaoDto.NaoPaginarPesquisa)
            {
                double paginas = totalItens <= requisicaoDto.NumeroItensPorPagina ? 1 : totalItens / requisicaoDto.NumeroItensPorPagina;
                retornoDto.NumeroPaginas = (int)Math.Ceiling(paginas);

                int pular = (requisicaoDto.Pagina - 1) * requisicaoDto.NumeroItensPorPagina;
                query = query.Skip(pular).Take(requisicaoDto.NumeroItensPorPagina);
            }

            List<MovimentoCaixaVo> listaVo = query.ToList();
            foreach (var movimentoCaixa in listaVo)
            {
                MovimentoCaixaDto movimentoCaixaDto = new MovimentoCaixaDto();
                if (!ConverterVoParaDto(movimentoCaixa, ref movimentoCaixaDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaMovimentoCaixa, movimentoCaixa.Id, retornoDto.Mensagem);
                    return false;
                }

                retornoDto.ListaEntidades.Add(movimentoCaixaDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita uma movimentação de caixa
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<MovimentoCaixaDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Apenas usuários ADM podem editar movimentoCaixaes
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para editar os movimentoCaixaes é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarMovimentoCaixa, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            // Não deixar incluir uma movimentação de caixa repetido
            MovimentoCaixaVo movimentoCaixaVo;
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out movimentoCaixaVo, ref mensagemErro, true))
            {
                retornoDto.Mensagem = "Problemas para encontrar o movimentoCaixa: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarMovimentoCaixa, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref movimentoCaixaVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter a movimentação de caixa para Vo: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarMovimentoCaixa, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!EditarBd(movimentoCaixaVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados do movimentoCaixa: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarMovimentoCaixa, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao salvar os novos dados: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarMovimentoCaixa, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtem os dados necessários para popular a tela de caixa
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterFormasPagamentoDia(RequisicaoDataDto requisicaoDto, ref RetornoObterResumoCaixaDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterInformacoesDashboard, Guid.Empty, mensagemErro);
                return false;
            }

            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            string query = "SELECT CAST(ISNULL(SUM(RecebidoDinheiro), 0) AS float) AS RecebidoDinheiro," +
                " CAST(ISNULL(SUM(RecebidoCredito), 0) AS float) AS RecebidoCredito," +
                " CAST(ISNULL(SUM(RecebidoDebito), 0) AS float) AS RecebidoDebito," +
                " CAST(ISNULL(SUM(Troco), 0) AS float) AS Troco," +
                " CAST(ISNULL(SUM(TaxaEntrega), 0) AS float) AS TaxaEntrega" +
                " FROM PizzaByte.Pedidos" +
                " WHERE CAST(DataInclusao AS Date) = @dataCaixa AND Inativo = 0 AND Excluido = 0";

            listaFiltros.Add(new SqlParameter("dataCaixa", requisicaoDto.Data.Date));

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto = contexto.Database.SqlQuery<RetornoObterResumoCaixaDto>(query, listaFiltros.ToArray()).FirstOrDefault();
                retornoDto.TotalVendas = (retornoDto.RecebidoDinheiro - retornoDto.Troco) + retornoDto.RecebidoCredito + retornoDto.RecebidoDebito;

                retornoDto.Retorno = true;
                retornoDto.Mensagem = "Ok";
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterInformacoesDashboard, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtem o total de entrega por profissional
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterTotalEntregaPorProfissional(RequisicaoDataDto requisicaoDto, ref RetornoObterTotalEntregaPorProfissionalDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterTotalEntregaPorProfissional, Guid.Empty, mensagemErro);
                return false;
            }

            List<SqlParameter> listaFiltros = new List<SqlParameter>();
            listaFiltros.Add(new SqlParameter("dataCaixa", requisicaoDto.Data.Date));

            string query = "SELECT CAST(ISNULL(SUM(TaxaEntrega), 0) AS float) AS TotalEntregas," +
                " f.Nome AS NomeProfissional" +
                " FROM PizzaByte.PedidosEntregas AS e" +
                " INNER JOIN PizzaByte.Pedidos AS p ON e.IdPedido = p.Id" +
                " INNER JOIN PizzaByte.Funcionarios AS f ON e.IdFuncionario = f.Id AND f.Inativo = 0" +
                " AND f.Excluido = 0" +
                " WHERE CAST(e.DataInclusao AS Date) = @dataCaixa AND e.Inativo = 0 AND e.Excluido = 0" +
                " AND e.IdFuncionario IS NOT NULL" +
                " GROUP BY f.Nome" +
                " ORDER BY f.Nome";

            try
            {
                PizzaByteContexto contexto = new PizzaByteContexto();
                retornoDto.ListaTotais = contexto.Database.SqlQuery<TotalPorProfissional>(query, listaFiltros.ToArray()).ToList();

                retornoDto.Retorno = true;
                retornoDto.Mensagem = "Ok";
                return true;
            }
            catch (Exception ex)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter os dados: " + ex.Message;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterTotalEntregaPorProfissional, Guid.Empty, mensagemErro);
                return false;
            }
        }
    }
}

