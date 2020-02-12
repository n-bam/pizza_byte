using PizzaByteBll.Base;
using PizzaByteDal;
using PizzaByteDto.ClassesBase;
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
    public class PedidoEntregaBll : BaseBll<PedidoEntregaVo, PedidoEntregaDto>
    {
        private static LogBll logBll = new LogBll("PedidoEntregaBll");
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public PedidoEntregaBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public PedidoEntregaBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui uma entrega no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<PedidoEntregaDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Converte para VO a ser incluída no banco de dados
            string mensagemErro = "";
            PedidoEntregaVo pedidoEntregaVo = new PedidoEntregaVo();
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref pedidoEntregaVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter a entrega para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirPedidoEntrega, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(pedidoEntregaVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter a entrega para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirPedidoEntrega, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirPedidoEntrega, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui uma entrega do banco de dados a partir do ID
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para excluir entregas é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirPedidoEntrega, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!base.Excluir(requisicaoDto, ref retornoDto))
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

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirPedidoEntrega, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirPedidoEntrega, requisicaoDto.Id, "Entrega excluída.");
            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui a entrega que tiver um id pedido específico
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool ExcluirPorIdPedido(RequisicaoObterDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            IQueryable<PedidoEntregaVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = $"Houve um problema ao listar as entregas: {mensagemErro}";
                return false;
            }

            query = query.Where(p => p.IdPedido == requisicaoDto.Id);
            List<Guid> listaEntregas = query.Select(p => p.Id).ToList();

            foreach (var entrega in listaEntregas)
            {
                // Exclui do banco de dados
                if (!ExcluirBd(entrega, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;
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
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém uma entrega pelo ID 
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<PedidoEntregaDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            PedidoEntregaVo pedidoEntregaVo;

            if (!ObterPorIdBd(requisicaoDto.Id, out pedidoEntregaVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter a entrega: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterPedidoEntrega, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            // Converter para DTO
            PedidoEntregaDto pedidoEntregaDto = new PedidoEntregaDto();
            if (!ConverterVoParaDto(pedidoEntregaVo, ref pedidoEntregaDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter a entrega: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterPedidoEntrega, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            // Obter o endereço de entrega
            ClienteEnderecoBll clienteEnderecoBll = new ClienteEnderecoBll(pizzaByteContexto, false);
            requisicaoDto.Id = pedidoEntregaDto.IdEndereco;

            RetornoObterDto<ClienteEnderecoDto> retornoEnderecoDto = new RetornoObterDto<ClienteEnderecoDto>();
            if (!clienteEnderecoBll.Obter(requisicaoDto, ref retornoEnderecoDto))
            {
                retornoDto.Mensagem = "Erro ao obter o endereço: " + retornoEnderecoDto.Mensagem;
                retornoDto.Retorno = false;
            }

            pedidoEntregaDto.ClienteEndereco = retornoEnderecoDto.Entidade;

            retornoDto.Entidade = pedidoEntregaDto;
            retornoDto.Retorno = true;
            retornoDto.Mensagem = "Ok";
            return true;
        }

        /// <summary>
        /// Converte uma entrega Dto para uma entrega Vo
        /// </summary>
        /// <param name="pedidoEntregaDto"></param>
        /// <param name="pedidoEntregaVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(PedidoEntregaDto pedidoEntregaDto, ref PedidoEntregaVo pedidoEntregaVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(pedidoEntregaDto, ref pedidoEntregaVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                pedidoEntregaVo.Obs = string.IsNullOrWhiteSpace(pedidoEntregaDto.Obs) ? "" : pedidoEntregaDto.Obs.Trim();
                pedidoEntregaVo.Conferido = pedidoEntregaDto.Conferido;
                pedidoEntregaVo.IdEndereco = pedidoEntregaDto.IdEndereco;
                pedidoEntregaVo.IdFuncionario = pedidoEntregaDto.IdFuncionario;
                pedidoEntregaVo.IdPedido = pedidoEntregaDto.IdPedido;
                pedidoEntregaVo.ValorRetorno = pedidoEntregaDto.ValorRetorno;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a entrega para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte uma entrega Dto para uma entrega Vo
        /// </summary>
        /// <param name="pedidoEntregaVo"></param>
        /// <param name="pedidoEntregaDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(PedidoEntregaVo pedidoEntregaVo, ref PedidoEntregaDto pedidoEntregaDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(pedidoEntregaVo, ref pedidoEntregaDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                pedidoEntregaDto.Obs = string.IsNullOrWhiteSpace(pedidoEntregaVo.Obs) ? "" : pedidoEntregaVo.Obs.Trim();
                pedidoEntregaDto.Conferido = pedidoEntregaVo.Conferido;
                pedidoEntregaDto.IdEndereco = pedidoEntregaVo.IdEndereco;
                pedidoEntregaDto.IdFuncionario = pedidoEntregaVo.IdFuncionario;
                pedidoEntregaDto.IdPedido = pedidoEntregaVo.IdPedido;
                pedidoEntregaDto.ValorRetorno = pedidoEntregaVo.ValorRetorno;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a entrega para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém uma lista de entregas com filtros aplicados, podendo ser paginada
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<PedidoEntregaDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            IQueryable<PedidoEntregaVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as entregas: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoEntrega, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "IDENDERECO":
                        Guid idEndereco;
                        if (!Guid.TryParse(filtro.Value, out idEndereco))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de endereço.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoEntrega, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.IdEndereco == idEndereco);
                        break;

                    case "DATAINCLUSAOINICIO":

                        DateTime data;
                        if (!DateTime.TryParse(filtro.Value, out data))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'data de inclusão'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoEntrega, Guid.Empty, retornoDto.Mensagem);
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

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoEntrega, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataInclusao) <= dataFim);
                        break;

                    case "IDPEDIDO":
                        Guid idPedido;
                        if (!Guid.TryParse(filtro.Value, out idPedido))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de pedido.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoEntrega, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.IdPedido == idPedido);
                        break;

                    case "IDFUNCIONARIO":
                        Guid idFuncinario;
                        if (!Guid.TryParse(filtro.Value, out idFuncinario))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de pedido.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoEntrega, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.IdFuncionario == idFuncinario);
                        break;

                    case "VALORRETORNO":
                        float valor;
                        if (!float.TryParse(filtro.Value, out valor))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de preço.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoEntrega, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.ValorRetorno == valor);
                        break;

                    case "CONFERIDO":
                        bool conferido;
                        if (!bool.TryParse(filtro.Value, out conferido))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'inativo'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoEntrega, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Conferido == conferido);
                        break;

                    case "INATIVO":
                        bool filtroInativo;
                        if (!bool.TryParse(filtro.Value, out filtroInativo))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'inativo'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoEntrega, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Inativo == filtroInativo);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoEntrega, Guid.Empty, retornoDto.Mensagem);
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "DATAINCLUSAO":
                    query = query.OrderBy(p => p.DataInclusao).ThenBy(p => p.IdFuncionario);
                    break;

                case "IDFUNCIONARIO":
                    query = query.OrderBy(p => p.IdFuncionario).ThenBy(p => p.DataInclusao);
                    break;

                case "DATAINCLUSAODESCRESCENTE":
                    query = query.OrderByDescending(p => p.DataInclusao).ThenBy(p => p.IdFuncionario);
                    break;

                default:
                    query = query.OrderByDescending(p => p.DataInclusao).ThenBy(p => p.IdFuncionario);
                    break;
            }

            double totalItens = query.Count();
            if (totalItens == 0)
            {
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

            // Obter entregas e os IDs dos endereços
            List<PedidoEntregaVo> listaVo = query.ToList();
            List<Guid> listaIds = listaVo.Select(p => p.IdEndereco).ToList();

            // Obter os endereços pelo ID
            RetornoObterListaDto<ClienteEnderecoDto> retornoEnderecoDto = new RetornoObterListaDto<ClienteEnderecoDto>();
            RequisicaoListaGuidsDto requisicaoEnderecoDto = new RequisicaoListaGuidsDto()
            {
                Identificacao = requisicaoDto.Identificacao,
                IdUsuario = requisicaoDto.IdUsuario,
                ListaGuids = listaIds
            };

            ClienteEnderecoBll clienteEnderecoBll = new ClienteEnderecoBll(false);
            if (!clienteEnderecoBll.ObterListaPorId(requisicaoEnderecoDto, ref retornoEnderecoDto))
            {
                retornoDto.Mensagem = "Erro ao obter os endereços: " + retornoEnderecoDto.Mensagem;
                retornoDto.Retorno = false;
            }

            // Converter da dto
            ClienteEnderecoDto enderedoNaoEncontrado = new ClienteEnderecoDto();
            enderedoNaoEncontrado.Endereco.Logradouro = "Endereço não encontrado";

            foreach (var pedidoEntrega in listaVo)
            {
                PedidoEntregaDto pedidoEntregaDto = new PedidoEntregaDto();
                if (!ConverterVoParaDto(pedidoEntrega, ref pedidoEntregaDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoEntrega, pedidoEntrega.Id, retornoDto.Mensagem);
                    return false;
                }

                ClienteEnderecoDto endereco = retornoEnderecoDto.ListaEntidades.Where(p => p.Id == pedidoEntregaDto.IdEndereco).FirstOrDefault();
                pedidoEntregaDto.ClienteEndereco = (endereco == null) ? enderedoNaoEncontrado : endereco;

                retornoDto.ListaEntidades.Add(pedidoEntregaDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita uma entrega
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<PedidoEntregaDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Apenas usuários ADM podem editar pedidoEntregaes
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para editar as entregas é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedidoEntrega, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            PedidoEntregaVo pedidoEntregaVo = new PedidoEntregaVo();
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out pedidoEntregaVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar a entrega: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedidoEntrega, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref pedidoEntregaVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter a entrega para Vo: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedidoEntrega, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!EditarBd(pedidoEntregaVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados da entrega: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedidoEntrega, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao salvar os novos dados: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedidoEntrega, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Inclui ou atualiza os dados de uma entrega
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="entregaDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool IncluirEditar(BaseRequisicaoDto requisicaoDto, PedidoEntregaDto entregaDto, ref string mensagemErro)
        {
            if (entregaDto == null)
            {
                mensagemErro = "Preencha a entrega que deseja incluir/editar.";
                return false;
            }

            PedidoEntregaVo entregaVo;
            if (!ObterPorIdBd(entregaDto.Id, out entregaVo, ref mensagemErro) && mensagemErro != "Cadastro não encontrado")
            {
                mensagemErro = "Problemas para encontrar a entrega: " + mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirEditarPedidoEntrega, entregaDto.Id, mensagemErro);
                return false;
            }

            // Incluir se não existir
            if (entregaVo == null)
            {
                RequisicaoEntidadeDto<PedidoEntregaDto> requisicaoIncluirDto = new RequisicaoEntidadeDto<PedidoEntregaDto>()
                {
                    EntidadeDto = entregaDto,
                    Identificacao = requisicaoDto.Identificacao,
                    IdUsuario = requisicaoDto.IdUsuario
                };

                RetornoDto retornoDto = new RetornoDto();
                if (!Incluir(requisicaoIncluirDto, ref retornoDto))
                {
                    mensagemErro = retornoDto.Mensagem;
                    return false;
                }
            }
            else
            {
                // verificar se precisa editar
                if (entregaDto.IdEndereco != entregaVo.IdEndereco)
                {
                    if (entregaVo.Conferido)
                    {
                        mensagemErro = "Não é possível alterar dados de entregas já conferidas";
                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirEditarPedidoEntrega, entregaDto.Id, mensagemErro);
                        return false;
                    }

                    RequisicaoEntidadeDto<PedidoEntregaDto> requisicaoEditarDto = new RequisicaoEntidadeDto<PedidoEntregaDto>()
                    {
                        EntidadeDto = entregaDto,
                        Identificacao = requisicaoDto.Identificacao,
                        IdUsuario = requisicaoDto.IdUsuario
                    };

                    RetornoDto retornoDto = new RetornoDto();
                    if (!Editar(requisicaoEditarDto, ref retornoDto))
                    {
                        mensagemErro = retornoDto.Mensagem;
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Obtem uma entrega por ID do pedido
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="entregaDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool ObterEntregaPorIdPedido(RequisicaoObterDto requisicaoDto, ref PedidoEntregaDto entregaDto, ref string mensagemErro)
        {
            if (requisicaoDto.Id == Guid.Empty || requisicaoDto.Id == null)
            {
                mensagemErro = "Preencha o id do pedido para obter a sua entrega.";
                return false;
            }

            // Obter a query primária
            IQueryable<PedidoEntregaVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                mensagemErro = $"Houve um problema ao obter a entrega: {mensagemErro}";
                return false;
            }

            PedidoEntregaVo entregaVo = query.Where(p => p.IdPedido == requisicaoDto.Id).FirstOrDefault();
            if (entregaVo != null)
            {
                if (!ConverterVoParaDto(entregaVo, ref entregaDto, ref mensagemErro))
                {
                    return false;
                }

                // Obter o endereço
                RetornoObterDto<ClienteEnderecoDto> retornoDto = new RetornoObterDto<ClienteEnderecoDto>();
                ClienteEnderecoBll clienteEnderecoBll = new ClienteEnderecoBll(pizzaByteContexto, false);

                requisicaoDto.Id = entregaDto.IdEndereco;
                if (!clienteEnderecoBll.Obter(requisicaoDto, ref retornoDto))
                {
                    mensagemErro = retornoDto.Mensagem;
                    return false;
                }

                entregaDto.ClienteEndereco = retornoDto.Entidade;
                return true;
            }
            else
            {
                mensagemErro = "Entrega não encontrada";
                return false;
            }
        }

        /// <summary>
        /// Cancela a entrega de acordo com o id do pedido
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool CancelarEntrega(RequisicaoObterDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            IQueryable<PedidoEntregaVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = $"Houve um problema ao obter a entrega: {mensagemErro}";
                return false;
            }

            PedidoEntregaVo entregaVo = query.Where(p => p.IdPedido == requisicaoDto.Id).FirstOrDefault();
            if (entregaVo != null)
            {
                entregaVo.Inativo = true;
                if (!EditarBd(entregaVo, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao editar os novos dados da entrega: " + mensagemErro;
                    return false;
                }

                if (salvar)
                {
                    // Salva as alterações
                    if (!pizzaByteContexto.Salvar(ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = "Erro ao salvar o cancelamento da entrega: " + mensagemErro;
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Confere uma entrega, registrando o valor retornado
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ConferirEntrega(RequisicaoConferirEntregaDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ConferirEntrega, requisicaoDto.Id, mensagemErro);
                return false;
            }

            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para conferir entregas é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ConferirEntrega, requisicaoDto.Id, retornoDto.Mensagem);
            }

            // Validar o id da entrega
            if (requisicaoDto.Id == Guid.Empty || requisicaoDto.Id == null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "O id da entrega não foi preenchido.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ConferirEntrega, requisicaoDto.Id, mensagemErro);
                return false;
            }

            // Validar o valor
            if (requisicaoDto.ValorRetornado < 0)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "O valor retornado não pode ser menor que 0 (zero).";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ConferirEntrega, requisicaoDto.Id, mensagemErro);
                return false;
            }

            PedidoEntregaVo entregaVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out entregaVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o pedido: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ConferirEntrega, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            // Preencher os dados da conferência
            entregaVo.Conferido = true;
            entregaVo.ValorRetorno = requisicaoDto.ValorRetornado;

            if (!EditarBd(entregaVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados da entrega: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ConferirEntrega, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao salvar os novos dados: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ConferirEntrega, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Altera o funcionário responsável pela entrega
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool AlterarFuncionarioEntrega(RequisicaoAlterarFuncionarioEntregaDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.AlterarFuncionarioEntrega, requisicaoDto.Id, mensagemErro);
                return false;
            }

            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para alterar o funcionário das entregas é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.AlterarFuncionarioEntrega, requisicaoDto.Id, retornoDto.Mensagem);
            }

            // Validar o id da entrega
            if (requisicaoDto.Id == Guid.Empty || requisicaoDto.Id == null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "O id da entrega não foi preenchido.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.AlterarFuncionarioEntrega, requisicaoDto.Id, mensagemErro);
                return false;
            }

            // Validar o id do funcionário
            if (requisicaoDto.IdFuncionario == Guid.Empty || requisicaoDto.IdFuncionario == null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "O funcionário não foi informado.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.AlterarFuncionarioEntrega, requisicaoDto.Id, mensagemErro);
                return false;
            }

            PedidoEntregaVo entregaVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out entregaVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o pedido: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.AlterarFuncionarioEntrega, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            // Preencher os dados da conferência
            entregaVo.IdFuncionario = requisicaoDto.IdFuncionario;

            if (!EditarBd(entregaVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados da entrega: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.AlterarFuncionarioEntrega, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao salvar os novos dados: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.AlterarFuncionarioEntrega, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }
    }
}
