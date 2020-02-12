using PizzaByteBll.Base;
using PizzaByteDal;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteVo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteBll
{
    public class ContaReceberBll : BaseBll<ContaReceberVo, ContaReceberDto>
    {
        private static LogBll logBll = new LogBll("ContaReceberBll");
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public ContaReceberBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public ContaReceberBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui uma conta no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<ContaReceberDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            ContaReceberVo contaReceberVo = new ContaReceberVo();
            string mensagemErro = "";

            // Converte para VO a ser incluída no banco de dados
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref contaReceberVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter a conta para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirContaReceber, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(contaReceberVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter a conta para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirContaReceber, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirContaReceber, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui uma conta do banco de dados a partir do ID
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
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para excluir a conta é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirContaReceber, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirContaReceber, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirContaReceber, requisicaoDto.Id, "Conta a receber excluída.");
            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém uma conta pelo ID 
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<ContaReceberDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            ContaReceberVo contaReceberVo;

            if (!ObterPorIdBd(requisicaoDto.Id, out contaReceberVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter a conta: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterContaReceber, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            ContaReceberDto contaReceberDto = new ContaReceberDto();
            if (!ConverterVoParaDto(contaReceberVo, ref contaReceberDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter a conta: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterContaReceber, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Entidade = contaReceberDto;
            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte um conta Dto para uma conta Vo
        /// </summary>
        /// <param name="contaReceberDto"></param>
        /// <param name="contaReceberVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(ContaReceberDto contaReceberDto, ref ContaReceberVo contaReceberVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(contaReceberDto, ref contaReceberVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                contaReceberVo.Descricao = string.IsNullOrWhiteSpace(contaReceberDto.Descricao) ? "" : contaReceberDto.Descricao.Trim();
                contaReceberVo.Valor = contaReceberDto.Valor;
                contaReceberVo.Status = contaReceberDto.Status;
                contaReceberVo.IdPedido = contaReceberDto.IdPedido;
                contaReceberVo.DataVencimento = contaReceberDto.DataVencimento;
                contaReceberVo.DataCompetencia = contaReceberDto.DataCompetencia;


                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a conta para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte uma conta Vo para uma conta Dto
        /// </summary>
        /// <param name="contaReceberVo"></param>
        /// <param name="contaReceberDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(ContaReceberVo contaReceberVo, ref ContaReceberDto contaReceberDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(contaReceberVo, ref contaReceberDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                contaReceberDto.Descricao = string.IsNullOrWhiteSpace(contaReceberVo.Descricao) ? "" : contaReceberVo.Descricao.Trim();
                contaReceberDto.Valor = contaReceberVo.Valor;
                contaReceberDto.Status = contaReceberVo.Status;
                contaReceberDto.IdPedido = contaReceberVo.IdPedido;
                contaReceberDto.DataVencimento = contaReceberVo.DataVencimento;
                contaReceberDto.DataCompetencia = contaReceberVo.DataCompetencia;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a conta para Dto: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém uma lista de contas com filtros aplicados, podendo ser paginada
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<ContaReceberDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            IQueryable<ContaReceberVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as contas: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaReceber, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "DESCRICAO":
                        query = query.Where(p => p.Descricao.Contains(filtro.Value));
                        break;

                    case "PRECOMAIOR":
                        float valorMaior;
                        if (!float.TryParse(filtro.Value, out valorMaior))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de preço (maior que).";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaReceber, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Valor >= valorMaior);
                        break;

                    case "PRECOMENOR":
                        float valorMenor;
                        if (!float.TryParse(filtro.Value, out valorMenor))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de preço (menor que).";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaReceber, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Valor <= valorMenor);
                        break;

                    case "VALOR":
                        float Valor;
                        if (!float.TryParse(filtro.Value, out Valor))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o valor da conta.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaReceber, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }
                        query = query.Where(p => p.Valor == Valor);
                        break;

                    case "DATAINICIOCOMPETENCIA":

                        DateTime data;
                        if (!DateTime.TryParse(filtro.Value, out data))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'data de competência'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaReceber, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataCompetencia) >= data);
                        break;

                    case "DATAFIMCOMPETENCIA":

                        DateTime dataFim;
                        if (!DateTime.TryParse(filtro.Value, out dataFim))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'data de competência'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaReceber, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataCompetencia) <= dataFim);
                        break;

                    case "DATAINICIOVENCIMENTO":

                        DateTime dataVencimento;
                        if (!DateTime.TryParse(filtro.Value, out dataVencimento))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'data de vencimento'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaReceber, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataVencimento) >= dataVencimento);
                        break;

                    case "DATAFIMVENCIMENTO":

                        DateTime dataFimVencimento;
                        if (!DateTime.TryParse(filtro.Value, out dataFimVencimento))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'data de vencimento'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaReceber, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataVencimento) <= dataFimVencimento);
                        break;

                    case "DATAINICIOINCLUSAO":

                        DateTime dataInclusao;
                        if (!DateTime.TryParse(filtro.Value, out dataInclusao))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'data de inclusão'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaReceber, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataInclusao) >= dataInclusao);
                        break;

                    case "DATAFIMINCLUSAO":

                        DateTime dataFimInclusao;
                        if (!DateTime.TryParse(filtro.Value, out dataFimInclusao))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'data de inclusão'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaReceber, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataInclusao) <= dataFimInclusao);
                        break;

                    case "STATUS":

                        int status;
                        if (!int.TryParse(filtro.Value, out status))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de status.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaReceber, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Status == (StatusConta)status);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaReceber, Guid.Empty, retornoDto.Mensagem);
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "DESCRIÇÂO":
                    query = query.OrderBy(p => p.Descricao).ThenBy(p => p.DataVencimento);
                    break;

                case "VALOR":
                    query = query.OrderBy(p => p.Valor).ThenBy(p => p.DataVencimento);
                    break;

                case "VENCIMENTO":
                    query = query.OrderBy(p => p.DataVencimento).ThenBy(p => p.Descricao);
                    break;
            }

            double totalItens = query.Count();
            double paginas = totalItens <= requisicaoDto.NumeroItensPorPagina ? 1 : totalItens / requisicaoDto.NumeroItensPorPagina;
            retornoDto.NumeroPaginas = (int)Math.Ceiling(paginas);

            int pular = (requisicaoDto.Pagina - 1) * requisicaoDto.NumeroItensPorPagina;
            query = query.Skip(pular).Take(requisicaoDto.NumeroItensPorPagina);

            if (totalItens == 0)
            {
                retornoDto.Mensagem = "Nenhum resultado encontrado.";
                retornoDto.Retorno = true;
                return true;
            }

            List<ContaReceberVo> listaVo = query.ToList();
            foreach (var contaReceber in listaVo)
            {
                ContaReceberDto contaReceberDto = new ContaReceberDto();
                if (!ConverterVoParaDto(contaReceber, ref contaReceberDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaReceber, contaReceber.Id, retornoDto.Mensagem);
                    return false;
                }

                retornoDto.ListaEntidades.Add(contaReceberDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita um conta
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<ContaReceberDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            ContaReceberVo contaReceberVo = new ContaReceberVo();
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out contaReceberVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar a conta: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarContaReceber, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref contaReceberVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter a conta para Vo: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarContaReceber, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!EditarBd(contaReceberVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao salvar os novos dados da conta: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarContaReceber, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarContaReceber, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Edita as contas vindas de pedidos
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool EditarContasPedido(RequisicaoListaEntidadesDto<ContaReceberDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarListaContasReceberPedido, Guid.Empty, mensagemErro);
                return false;
            }

            List<ContaReceberVo> listaContas;
            IQueryable<ContaReceberVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as contas: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarListaContasReceberPedido, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            listaContas = query.Where(p => p.IdPedido == requisicaoDto.IdComum).ToList();
            foreach (var conta in listaContas)
            {
                if (!ExcluirDefinitivoBd(conta.Id, ref mensagemErro))
                {
                    retornoDto.Mensagem = $"Houve um problema ao excluir as contas: {mensagemErro}";
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarListaContasReceberPedido, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            //Confirmar o id do pedido e um novo id para cada item
            requisicaoDto.ListaEntidadesDto.ForEach(p => p.IdPedido = requisicaoDto.IdComum);
            requisicaoDto.ListaEntidadesDto.ForEach(p => p.Id = (p.Id == null || p.Id == Guid.Empty) ? Guid.NewGuid() : p.Id);

            // Para cada item
            foreach (var item in requisicaoDto.ListaEntidadesDto)
            {
                if (!item.ValidarEntidade(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;
                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarListaContasReceberPedido, Guid.Empty, mensagemErro);
                    return false;
                }

                ContaReceberVo itemVo = new ContaReceberVo();
                if (!ConverterDtoParaVo(item, ref itemVo, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao converter a conta do pedido para VO: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarListaContasReceberPedido, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }

                // Prepara a inclusão no banco de dados
                if (!IncluirBd(itemVo, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao salvar a conta do pedido: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarListaContasReceberPedido, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarListaContasReceberPedido, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Inclui/edita as contas vindas de pedidos
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool IncluirContasPedido(RequisicaoListaEntidadesDto<ContaReceberDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirListaContasReceberPedido, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.ListaEntidadesDto.Count <= 0)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Informe as contas que deseja incluir.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirListaContasReceberPedido, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            //Confirmar o id do pedido e um novo id para cada item
            requisicaoDto.ListaEntidadesDto.ForEach(p => p.IdPedido = requisicaoDto.IdComum);
            requisicaoDto.ListaEntidadesDto.ForEach(p => p.Id = (p.Id == null || p.Id == Guid.Empty) ? Guid.NewGuid() : p.Id);

            // Para cada item
            foreach (var item in requisicaoDto.ListaEntidadesDto)
            {
                if (!item.ValidarEntidade(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;
                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirListaContasReceberPedido, Guid.Empty, mensagemErro);
                    return false;
                }

                ContaReceberVo itemVo = new ContaReceberVo();
                if (!ConverterDtoParaVo(item, ref itemVo, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao converter a conta do pedido para VO: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirListaContasReceberPedido, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }

                // Prepara a inclusão no banco de dados
                if (!IncluirBd(itemVo, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao salvar a conta do pedido: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirListaContasReceberPedido, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirListaContasReceberPedido, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Edita as contas vindas de pedidos
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool EstornarContasPedido(RequisicaoCancelarPedidoDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EstornarContasReceberPedido, Guid.Empty, mensagemErro);
                return false;
            }

            List<ContaReceberVo> listaContas;
            IQueryable<ContaReceberVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as contas: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EstornarContasReceberPedido, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            listaContas = query.Where(p => p.IdPedido == requisicaoDto.Id).ToList();
            foreach (var conta in listaContas)
            {
                conta.Status = StatusConta.Estornada;
                if (!EditarBd(conta, ref mensagemErro))
                {
                    retornoDto.Mensagem = $"Houve um problema ao editar as contas: {mensagemErro}";
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EstornarContasReceberPedido, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }
            
            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarListaContasReceberPedido, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui as contas vindas de pedidos
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool ExcluirContasPedido(RequisicaoObterDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirContasReceberPedido, requisicaoDto.Id, mensagemErro);
                return false;
            }

            List<ContaReceberVo> listaContas;
            IQueryable<ContaReceberVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as contas: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirContasReceberPedido, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            listaContas = query.Where(p => p.IdPedido == requisicaoDto.Id).ToList();
            foreach (var conta in listaContas)
            {
                if (!ExcluirBd(conta.Id, ref mensagemErro))
                {
                    retornoDto.Mensagem = $"Houve um problema ao excluir as contas: {mensagemErro}";
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirContasReceberPedido, conta.Id, retornoDto.Mensagem);
                    return false;
                }
            }
            
            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarListaContasReceberPedido, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }
    }
}
