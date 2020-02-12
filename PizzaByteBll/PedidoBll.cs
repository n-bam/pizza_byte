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
    public class PedidoBll : BaseBll<PedidoVo, PedidoDto>
    {
        private static LogBll logBll = new LogBll("PedidoBll");
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public PedidoBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public PedidoBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui um pedido, seus itens, sua entrega e seu cliente no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<PedidoDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Se não houver cliente, deixar o id Nulo
            if (requisicaoDto.EntidadeDto.Cliente == null || requisicaoDto.EntidadeDto.Cliente.Id == Guid.Empty)
            {
                requisicaoDto.EntidadeDto.IdCliente = null;
            }

            // Converte para VO a ser incluída no banco de dados
            string mensagemErro = "";
            PedidoVo pedidoVo = new PedidoVo();
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref pedidoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o pedido para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirPedido, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(pedidoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o pedido para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirPedido, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            //Incluir Itens
            PedidoItemBll pedidoItemBll = new PedidoItemBll(pizzaByteContexto, false);
            RequisicaoListaEntidadesDto<PedidoItemDto> requisicaoItensDto = new RequisicaoListaEntidadesDto<PedidoItemDto>()
            {
                IdComum = pedidoVo.Id,
                Identificacao = requisicaoDto.Identificacao,
                IdUsuario = requisicaoDto.IdUsuario,
                ListaEntidadesDto = requisicaoDto.EntidadeDto.ListaItens
            };

            if (!pedidoItemBll.IncluirLista(requisicaoItensDto, ref retornoDto))
            {
                return false;
            }

            // Incluir/editar cliente, enredeço e taxas
            if (!AtualizarCadastros(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Se houver pagamentos, incluir as contas a receber
            if (requisicaoDto.EntidadeDto.RecebidoCredito != 0 ||
                requisicaoDto.EntidadeDto.RecebidoDebito != 0 ||
                requisicaoDto.EntidadeDto.RecebidoDinheiro != 0)
            {
                RequisicaoListaEntidadesDto<ContaReceberDto> requisicaoContasDto = new RequisicaoListaEntidadesDto<ContaReceberDto>()
                {
                    IdUsuario = requisicaoDto.IdUsuario,
                    Identificacao = requisicaoDto.Identificacao,
                    IdComum = requisicaoDto.EntidadeDto.Id
                };

                requisicaoDto.EntidadeDto.DataInclusao = DateTime.Now.AddHours(4);
                List<ContaReceberDto> listaContas = new List<ContaReceberDto>();
                if (!GerarListaContas(requisicaoDto.EntidadeDto, ref listaContas, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao gerar as contas: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirPedido, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }

                requisicaoContasDto.ListaEntidadesDto = listaContas;

                ContaReceberBll contaReceberBll = new ContaReceberBll(pizzaByteContexto, false);
                if (!contaReceberBll.IncluirContasPedido(requisicaoContasDto, ref retornoDto))
                {
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

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirPedido, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui um pedido do banco de dados a partir do ID
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Excluir(RequisicaoObterDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para excluir pedidos é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirPedido, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!base.Excluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            //Excluir itens
            PedidoItemBll pedidoItemBll = new PedidoItemBll(pizzaByteContexto, false);
            if (!pedidoItemBll.ExcluirItensPedido(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            //Excluir entregas, se houver
            PedidoEntregaBll pedidoEntregaBll = new PedidoEntregaBll(pizzaByteContexto, false);
            if (!pedidoEntregaBll.ExcluirPorIdPedido(requisicaoDto, ref retornoDto))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirPedido, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            // Excluir as contas
            ContaReceberBll contaReceberBll = new ContaReceberBll(pizzaByteContexto, false);
            if (!contaReceberBll.ExcluirContasPedido(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirPedido, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirPedido, requisicaoDto.Id, "Pedido excluído.");
            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém um pedido pelo ID 
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<PedidoDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            PedidoVo pedidoVo;

            if (!ObterPorIdBd(requisicaoDto.Id, out pedidoVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o pedido: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterPedido, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            PedidoDto pedidoDto = new PedidoDto();
            if (!ConverterVoParaDto(pedidoVo, ref pedidoDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o pedido: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterPedido, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Entidade = pedidoDto;

            //Obter itens
            PedidoItemBll pedidoItemBll = new PedidoItemBll(true);
            RequisicaoObterListaDto requisicaoItensDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "DESCRICAOPRODUTO",
                NaoPaginarPesquisa = true,
                Identificacao = requisicaoDto.Identificacao,
                IdUsuario = requisicaoDto.IdUsuario
            };

            requisicaoItensDto.ListaFiltros.Add("IDPEDIDO", requisicaoDto.Id.ToString());
            RetornoObterListaDto<PedidoItemDto> retornoItemDto = new RetornoObterListaDto<PedidoItemDto>();

            if (!pedidoItemBll.ObterListaFiltrada(requisicaoItensDto, ref retornoItemDto))
            {
                return false;
            }

            retornoDto.Entidade.ListaItens = retornoItemDto.ListaEntidades;

            //Obter entrega
            if (pedidoDto.Tipo == TipoPedido.Entrega)
            {
                PedidoEntregaDto entregaDto = new PedidoEntregaDto();
                PedidoEntregaBll pedidoEntregaBll = new PedidoEntregaBll(pizzaByteContexto, false);
                if (!pedidoEntregaBll.ObterEntregaPorIdPedido(requisicaoDto, ref entregaDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao obter a entrega do pedido: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterPedido, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }

                pedidoDto.Entrega = entregaDto;
            }

            //Obter cliente
            if (pedidoDto.IdCliente != null && pedidoDto.IdCliente != Guid.Empty)
            {
                requisicaoDto.Id = pedidoDto.IdCliente.Value;
                ClienteBll clienteBll = new ClienteBll(false);

                RetornoObterDto<ClienteDto> retornoClienteDto = new RetornoObterDto<ClienteDto>();
                if (!clienteBll.Obter(requisicaoDto, ref retornoClienteDto) && retornoClienteDto.Mensagem != "Erro ao obter o cliente: Cadastro não encontrado")
                {
                    retornoDto.Mensagem = retornoClienteDto.Mensagem;
                    retornoDto.Retorno = false;

                    return false;
                }

                if (retornoClienteDto.Mensagem == "Erro ao obter o cliente: Cadastro não encontrado")
                {
                    pedidoDto.Cliente.Nome = "Cadastro não encontro";
                }
                else
                {
                    pedidoDto.Cliente = retornoClienteDto.Entidade;
                }
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte um pedido Dto para um pedido Vo
        /// </summary>
        /// <param name="pedidoDto"></param>
        /// <param name="pedidoVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(PedidoDto pedidoDto, ref PedidoVo pedidoVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(pedidoDto, ref pedidoVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                pedidoVo.JustificativaCancelamento = string.IsNullOrWhiteSpace(pedidoDto.JustificativaCancelamento) ? "" : pedidoDto.JustificativaCancelamento.Trim();
                pedidoVo.Obs = string.IsNullOrWhiteSpace(pedidoDto.Obs) ? "" : pedidoDto.Obs.Trim();
                pedidoVo.IdCliente = pedidoDto.IdCliente;
                pedidoVo.PedidoIfood = pedidoDto.PedidoIfood;
                pedidoVo.RecebidoCredito = pedidoDto.RecebidoCredito;
                pedidoVo.RecebidoDebito = pedidoDto.RecebidoDebito;
                pedidoVo.RecebidoDinheiro = pedidoDto.RecebidoDinheiro;
                pedidoVo.TaxaEntrega = pedidoDto.TaxaEntrega;
                pedidoVo.Total = pedidoDto.Total;
                pedidoVo.Troco = pedidoDto.Troco;
                pedidoVo.Tipo = pedidoDto.Tipo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o pedido para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um pedido Dto para um pedido Vo
        /// </summary>
        /// <param name="pedidoVo"></param>
        /// <param name="pedidoDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(PedidoVo pedidoVo, ref PedidoDto pedidoDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(pedidoVo, ref pedidoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                pedidoDto.JustificativaCancelamento = string.IsNullOrWhiteSpace(pedidoVo.JustificativaCancelamento) ? "" : pedidoVo.JustificativaCancelamento.Trim();
                pedidoDto.Obs = string.IsNullOrWhiteSpace(pedidoVo.Obs) ? "" : pedidoVo.Obs.Trim();
                pedidoDto.IdCliente = pedidoVo.IdCliente;
                pedidoDto.PedidoIfood = pedidoVo.PedidoIfood;
                pedidoDto.RecebidoCredito = pedidoVo.RecebidoCredito;
                pedidoDto.RecebidoDebito = pedidoVo.RecebidoDebito;
                pedidoDto.RecebidoDinheiro = pedidoVo.RecebidoDinheiro;
                pedidoDto.TaxaEntrega = pedidoVo.TaxaEntrega;
                pedidoDto.Total = pedidoVo.Total;
                pedidoDto.Troco = pedidoVo.Troco;
                pedidoDto.Tipo = pedidoVo.Tipo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o pedido para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém uma lista de pedidos com filtros aplicados, podendo ser paginada
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<PedidoDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            IQueryable<PedidoVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os pedidos: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "JUSTIFICATIVA":
                        query = query.Where(p => p.JustificativaCancelamento.Contains(filtro.Value));
                        break;

                    case "TOTALMAIOR":
                        float totalMaior;
                        if (!float.TryParse(filtro.Value, out totalMaior))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de total (maior que).";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Total >= totalMaior);
                        break;

                    case "TOTALMENOR":
                        float totalMenor;
                        if (!float.TryParse(filtro.Value, out totalMenor))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de total (menor que).";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Total <= totalMenor);
                        break;

                    case "TOTAL":
                        float preco;
                        if (!float.TryParse(filtro.Value, out preco))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de preço.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Total == preco);
                        break;

                    case "TAXAENTREGA":
                        float taxa;
                        if (!float.TryParse(filtro.Value, out taxa))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de taxa de entrega.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.TaxaEntrega == taxa);
                        break;

                    case "TROCO":
                        float troco;
                        if (!float.TryParse(filtro.Value, out troco))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de preço.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Troco == troco);
                        break;

                    case "IDCLIENTE":
                        Guid idCliente;
                        if (!Guid.TryParse(filtro.Value, out idCliente))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de id do cliente.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.IdCliente == idCliente);
                        break;

                    case "RECEBIDOCARTAOCREDITO":

                        query = query.Where(p => p.RecebidoCredito > 0);
                        break;

                    case "RECEBIDOCARTAODEBITO":

                        query = query.Where(p => p.RecebidoDebito > 0);
                        break;

                    case "RECEBIDODINHEIRO":

                        query = query.Where(p => p.RecebidoDinheiro > 0);
                        break;

                    case "TIPO":

                        int tipo;
                        if (!int.TryParse(filtro.Value, out tipo))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de tipo.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Tipo == (TipoPedido)tipo);
                        break;

                    case "PEDIDOIFOOD":

                        bool filtroifood;
                        if (!bool.TryParse(filtro.Value, out filtroifood))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'pedido ifood'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Inativo == filtroifood);
                        break;

                    case "DATAINCLUSAOINICIO":

                        DateTime data;
                        if (!DateTime.TryParse(filtro.Value, out data))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'data de inclusão'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataInclusao) >= data);
                        break;

                    case "DATAINCLUSAOFIM":

                        DateTime dataFim;
                        if (!DateTime.TryParse(filtro.Value, out dataFim))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'data de inclusão'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataInclusao) <= dataFim);
                        break;

                    case "INATIVO":

                        bool filtroInativo;
                        if (!bool.TryParse(filtro.Value, out filtroInativo))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'inativo'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Inativo == filtroInativo);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, Guid.Empty, retornoDto.Mensagem);
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "DATAINCLUSAO":
                    query = query.OrderBy(p => p.DataInclusao).ThenBy(p => p.Tipo).ThenBy(p => p.Total);
                    break;

                case "TOTAL":
                    query = query.OrderBy(p => p.Total).ThenBy(p => p.DataInclusao).ThenBy(p => p.Tipo);
                    break;

                case "TIPO":
                    query = query.OrderByDescending(p => p.Tipo).ThenBy(p => p.DataInclusao).ThenBy(p => p.Total);
                    break;

                default:
                    query = query.OrderBy(p => p.DataInclusao).ThenBy(p => p.Tipo).ThenBy(p => p.Total);
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

            double paginas = totalItens <= requisicaoDto.NumeroItensPorPagina ? 1 : totalItens / requisicaoDto.NumeroItensPorPagina;
            retornoDto.NumeroPaginas = (int)Math.Ceiling(paginas);

            int pular = (requisicaoDto.Pagina - 1) * requisicaoDto.NumeroItensPorPagina;
            query = query.Skip(pular).Take(requisicaoDto.NumeroItensPorPagina);

            List<PedidoVo> listaVo = query.ToList();
            ClienteBll clienteBll = new ClienteBll(false);
            IQueryable<ClienteVo> queryCliente;

            // Obter a query primária
            if (!clienteBll.ObterQueryBd(out queryCliente, ref mensagemErro, true))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os pedidos: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            List<Guid?> listaIds = listaVo.Where(p => p.IdCliente != null && p.IdCliente != Guid.Empty).Select(p => p.IdCliente).ToList();
            List<ClienteVo> listaCliente = queryCliente.Where(p => listaIds.Contains(p.Id)).ToList();

            foreach (var pedido in listaVo)
            {
                PedidoDto pedidoDto = new PedidoDto();
                if (!ConverterVoParaDto(pedido, ref pedidoDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedido, pedido.Id, retornoDto.Mensagem);
                    return false;
                }

                if (pedidoDto.IdCliente != null && pedidoDto.IdCliente != Guid.Empty)
                {
                    ClienteVo cliente = listaCliente.Where(p => p.Id == pedidoDto.IdCliente).FirstOrDefault();
                    pedidoDto.NomeCliente = cliente == null ? "Cliente não encontrado" : cliente.Nome;
                }
                else
                {
                    pedidoDto.NomeCliente = "Cliente não identificado";
                }

                retornoDto.ListaEntidades.Add(pedidoDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita um pedido, seus itens, entrega e cliente, se houver
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<PedidoDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            PedidoVo pedidoVo = new PedidoVo();
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out pedidoVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar o pedido: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedido, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            // Se não houver cliente, deixar o id Nulo
            if (requisicaoDto.EntidadeDto.Cliente == null || requisicaoDto.EntidadeDto.Cliente.Id == Guid.Empty)
            {
                requisicaoDto.EntidadeDto.IdCliente = null;
            }

            TipoPedido tipoAntigo = pedidoVo.Tipo;
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref pedidoVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter o pedido para Vo: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedido, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!EditarBd(pedidoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados do pedido: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedido, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            //Editar itens
            PedidoItemBll pedidoItemBll = new PedidoItemBll(this.pizzaByteContexto, false);
            RequisicaoListaEntidadesDto<PedidoItemDto> requisicaoItensDto = new RequisicaoListaEntidadesDto<PedidoItemDto>()
            {
                IdComum = pedidoVo.Id,
                Identificacao = requisicaoDto.Identificacao,
                IdUsuario = requisicaoDto.IdUsuario,
                ListaEntidadesDto = requisicaoDto.EntidadeDto.ListaItens
            };

            if (!pedidoItemBll.EditarLista(requisicaoItensDto, ref retornoDto))
            {
                return false;
            }

            // Incluir/editar cliente, enredeço e taxas
            if (!AtualizarCadastros(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            //Editar entrega
            if (tipoAntigo == TipoPedido.Entrega && pedidoVo.Tipo != TipoPedido.Entrega)
            {
                PedidoEntregaBll pedidoEntregaBll = new PedidoEntregaBll(pizzaByteContexto, false);
                RequisicaoObterDto requisicaoEntidadeDto = new RequisicaoObterDto()
                {
                    Id = pedidoVo.Id,
                    Identificacao = requisicaoDto.Identificacao,
                    IdUsuario = requisicaoDto.IdUsuario
                };

                if (!pedidoEntregaBll.ExcluirPorIdPedido(requisicaoEntidadeDto, ref retornoDto))
                {
                    return false;
                }
            }

            // Editar contas, se houver pagamentos alterados
            RequisicaoListaEntidadesDto<ContaReceberDto> requisicaoContasDto = new RequisicaoListaEntidadesDto<ContaReceberDto>()
            {
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao,
                IdComum = requisicaoDto.EntidadeDto.Id
            };

            requisicaoDto.EntidadeDto.DataInclusao = pedidoVo.DataInclusao;
            List<ContaReceberDto> listaContas = new List<ContaReceberDto>();
            if (!GerarListaContas(requisicaoDto.EntidadeDto, ref listaContas, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao gerar as contas: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirPedido, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            requisicaoContasDto.ListaEntidadesDto = listaContas;
            ContaReceberBll contaReceberBll = new ContaReceberBll(pizzaByteContexto, false);
            if (!contaReceberBll.EditarContasPedido(requisicaoContasDto, ref retornoDto))
            {
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao salvar os novos dados: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedido, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Atualiza ou incluir dados de clientes, endereços e taxas
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        private bool AtualizarCadastros(RequisicaoEntidadeDto<PedidoDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";

            // Incluir/editar cliente
            if (requisicaoDto.EntidadeDto.Cliente != null && requisicaoDto.EntidadeDto.Cliente.Id != Guid.Empty)
            {
                ClienteBll clienteBll = new ClienteBll(pizzaByteContexto, false);
                if (!clienteBll.IncluirEditar(requisicaoDto, requisicaoDto.EntidadeDto.Cliente, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;
                    return false;
                }

                // Verificar endereço
                if (requisicaoDto.EntidadeDto.Entrega.ClienteEndereco != null && requisicaoDto.EntidadeDto.Entrega.ClienteEndereco.Id != Guid.Empty)
                {
                    CepBll cepBll = new CepBll(pizzaByteContexto, false);
                    if (!cepBll.IncluirEditar(requisicaoDto, requisicaoDto.EntidadeDto.Entrega.ClienteEndereco.Endereco, ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = mensagemErro;
                        return false;
                    }

                    // Incluir/editar endereço do cliente
                    requisicaoDto.EntidadeDto.Entrega.ClienteEndereco.IdCliente = requisicaoDto.EntidadeDto.Cliente.Id;
                    ClienteEnderecoBll clienteEnderecoBll = new ClienteEnderecoBll(pizzaByteContexto, false);
                    if (!clienteEnderecoBll.IncluirEditar(requisicaoDto, requisicaoDto.EntidadeDto.Entrega.ClienteEndereco, ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = mensagemErro;
                        return false;
                    }

                    // Incluir/editar a taxa de entrega, se houver
                    if (requisicaoDto.EntidadeDto.TaxaEntrega > 0 && !string.IsNullOrWhiteSpace(requisicaoDto.EntidadeDto.Entrega.ClienteEndereco.Endereco.Bairro)
                        && !string.IsNullOrWhiteSpace(requisicaoDto.EntidadeDto.Entrega.ClienteEndereco.Endereco.Cidade))
                    {
                        TaxaEntregaBll taxaEntregaBll = new TaxaEntregaBll(pizzaByteContexto, false);
                        TaxaEntregaDto taxaDto = new TaxaEntregaDto()
                        {
                            BairroCidade = requisicaoDto.EntidadeDto.Entrega.ClienteEndereco.Endereco.Bairro.Trim() + "_" + requisicaoDto.EntidadeDto.Entrega.ClienteEndereco.Endereco.Cidade,
                            ValorTaxa = requisicaoDto.EntidadeDto.TaxaEntrega
                        };

                        if (!taxaEntregaBll.IncluirEditar(requisicaoDto, taxaDto, ref mensagemErro))
                        {
                            retornoDto.Retorno = false;
                            retornoDto.Mensagem = mensagemErro;
                            return false;
                        }
                    }
                }
            }

            // Se for do tipo entrega
            if (requisicaoDto.EntidadeDto.Tipo == TipoPedido.Entrega)
            {
                requisicaoDto.EntidadeDto.Entrega.IdPedido = requisicaoDto.EntidadeDto.Id;
                requisicaoDto.EntidadeDto.Entrega.IdEndereco = requisicaoDto.EntidadeDto.Entrega.ClienteEndereco.Id;

                PedidoEntregaBll pedidoEntregaBll = new PedidoEntregaBll(pizzaByteContexto, false);
                if (!pedidoEntregaBll.IncluirEditar(requisicaoDto, requisicaoDto.EntidadeDto.Entrega, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gera uma lista de contas para serem incluídas ou editadas
        /// </summary>
        /// <param name="pedidoDto"></param>
        /// <param name="nomeCliente"></param>
        /// <param name="listaContas"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool GerarListaContas(PedidoDto pedidoDto, ref List<ContaReceberDto> listaContas, ref string mensagemErro)
        {
            if (pedidoDto == null)
            {
                mensagemErro = "Pedido não informado para inclusão das contas";
                return false;
            }

            if (string.IsNullOrWhiteSpace(pedidoDto.NomeCliente))
            {
                mensagemErro = "Informe o nome do cliente para gerar as contas do pedido";
                return false;
            }

            listaContas = new List<ContaReceberDto>();
            if (pedidoDto.RecebidoCredito > 0)
            {
                listaContas.Add(new ContaReceberDto()
                {
                    DataCompetencia = pedidoDto.DataInclusao,
                    DataVencimento = pedidoDto.DataInclusao.AddDays(30),
                    Descricao = $"Cartão de crédito ({pedidoDto.NomeCliente.Trim()})",
                    Status = StatusConta.Paga,
                    IdPedido = pedidoDto.Id,
                    Id = Guid.NewGuid(),
                    Valor = pedidoDto.RecebidoCredito
                });
            }

            if (pedidoDto.RecebidoDebito > 0)
            {
                listaContas.Add(new ContaReceberDto()
                {
                    DataCompetencia = pedidoDto.DataInclusao,
                    DataVencimento = pedidoDto.DataInclusao.AddDays(1),
                    Descricao = $"Cartão de débito ({pedidoDto.NomeCliente.Trim()})",
                    Status = StatusConta.Paga,
                    IdPedido = pedidoDto.Id,
                    Id = Guid.NewGuid(),
                    Valor = pedidoDto.RecebidoDebito
                });
            }

            if (pedidoDto.RecebidoDinheiro > 0)
            {
                listaContas.Add(new ContaReceberDto()
                {
                    DataCompetencia = pedidoDto.DataInclusao,
                    DataVencimento = pedidoDto.DataInclusao,
                    Descricao = $"Recebimento em dinheiro ({pedidoDto.NomeCliente.Trim()})",
                    Status = StatusConta.Paga,
                    IdPedido = pedidoDto.Id,
                    Id = Guid.NewGuid(),
                    Valor = pedidoDto.RecebidoDinheiro - pedidoDto.Troco
                });
            }

            return true;
        }

        /// <summary>
        /// Cancela um pedido e sua entrega, se houver
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool CancelarPedido(RequisicaoCancelarPedidoDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.CancelarPedido, requisicaoDto.Id, mensagemErro);
                return false;
            }

            PedidoVo pedidoVo = new PedidoVo();
            if (!ObterPorIdBd(requisicaoDto.Id, out pedidoVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar o pedido: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.CancelarPedido, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            // Mudar o inativo que se refere ao cancelado para true
            pedidoVo.Inativo = true;
            pedidoVo.JustificativaCancelamento = string.IsNullOrWhiteSpace(requisicaoDto.Justificativa) ? "" : requisicaoDto.Justificativa.Trim();
            if (!EditarBd(pedidoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao cancelar o pedido: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.CancelarPedido, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            // Cancelar a entrega se houver
            if (pedidoVo.Tipo == TipoPedido.Entrega)
            {
                PedidoEntregaBll pedidoEntregaBll = new PedidoEntregaBll(pizzaByteContexto, false);
                if (!pedidoEntregaBll.CancelarEntrega(requisicaoDto, ref retornoDto))
                {
                    retornoDto.Retorno = false;
                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.CancelarPedido, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            // Cancelar contas
            ContaReceberBll contaReceberBll = new ContaReceberBll(pizzaByteContexto, false);
            if (!contaReceberBll.EstornarContasPedido(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Cancelar itens
            PedidoItemBll pedidoItemBll = new PedidoItemBll(pizzaByteContexto, false);
            if (!pedidoItemBll.EstornarLista(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao salvar o cancelamento: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.CancelarPedido, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            logBll.ResgistrarLog(requisicaoDto, LogRecursos.CancelarPedido, requisicaoDto.Id, "Pedido cancelado.");
            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém os detalhes do pedido de uma entrega
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterPedidoResumido(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<PedidoResumidoDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterPedidoResumido, requisicaoDto.Id, mensagemErro);
                return false;
            }

            PedidoVo pedidoVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out pedidoVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o pedido: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterPedido, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Entidade = new PedidoResumidoDto()
            {
                PedidoIfood = pedidoVo.PedidoIfood,
                RecebidoCredito = pedidoVo.RecebidoCredito,
                RecebidoDebito = pedidoVo.RecebidoDebito,
                RecebidoDinheiro = pedidoVo.RecebidoDinheiro,
                TaxaEntrega = pedidoVo.TaxaEntrega,
                Troco = pedidoVo.Troco,
                Total = pedidoVo.Total,
                Tipo = pedidoVo.Tipo,
                Id = pedidoVo.Id
            };

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }
    }
}
