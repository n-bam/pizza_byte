using PizzaByteBll.Base;
using PizzaByteDal;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteVo;
using System;
using System.Collections.Generic;
using System.Linq;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteBll
{
    public class PedidoItemBll : BaseBll<PedidoItemVo, PedidoItemDto>
    {
        private static LogBll logBll = new LogBll("PedidoItemBll");
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public PedidoItemBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public PedidoItemBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui um item de pedido no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<PedidoItemDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            RequisicaoListaEntidadesDto<PedidoItemDto> requisicaoListaEntidadesDto = new RequisicaoListaEntidadesDto<PedidoItemDto>()
            {
                Identificacao = requisicaoDto.Identificacao,
                IdUsuario = requisicaoDto.IdUsuario
            };

            requisicaoListaEntidadesDto.ListaEntidadesDto.Add(requisicaoDto.EntidadeDto);
            return IncluirLista(requisicaoListaEntidadesDto, ref retornoDto);
        }

        /// <summary>
        /// Inclui uma lista de itens de um pedido no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool IncluirLista(RequisicaoListaEntidadesDto<PedidoItemDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirListaPedidoItem, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.ListaEntidadesDto.Count <= 0)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Informe ao menos um item para incluir.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            //Confirmar o id do pedido e um novo id para cada item
            requisicaoDto.ListaEntidadesDto.ForEach(p => p.IdPedido = requisicaoDto.IdComum);
            requisicaoDto.ListaEntidadesDto.ForEach(p => p.Id = Guid.NewGuid());

            // Para cada item
            foreach (var item in requisicaoDto.ListaEntidadesDto)
            {
                // Converte para VO a ser incluída no banco de dados
                PedidoItemVo pedidoItemVo = new PedidoItemVo();
                if (!ConverterDtoParaVo(item, ref pedidoItemVo, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao converter o item do pedido para VO: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }

                if (!item.ValidarEntidade(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;
                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirListaPedidoItem, Guid.Empty, mensagemErro);
                    return false;
                }

                // Prepara a inclusão no banco de dados
                if (!IncluirBd(pedidoItemVo, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao salvar o item do pedido: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
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

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui não será implementado para o item, pois a exclusão é feita em conjunto
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Excluir(RequisicaoObterDto requisicaoDto, ref RetornoDto retornoDto)
        {
            retornoDto.Retorno = false;
            retornoDto.Mensagem = "Método não implementado para esta entidade";
            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirPedidoItem, Guid.Empty, retornoDto.Mensagem);
            return false;
        }

        /// <summary>
        /// Exclui os itens de um pedido
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool ExcluirItensPedido(RequisicaoObterDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirListaItensPedido, Guid.Empty, mensagemErro);
                return false;
            }

            List<PedidoItemVo> listaExclusao;
            IQueryable<PedidoItemVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os itens: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirListaItensPedido, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Obter os itens do pedido
            query = query.Where(p => p.IdPedido == requisicaoDto.Id);
            listaExclusao = query.ToList();

            // Se algum item não veioda requisição
            foreach (var item in listaExclusao)
            {
                // Prepara a exclusão no banco de dados
                if (!ExcluirBd(item.Id, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao excluir o item do pedido: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirListaItensPedido, Guid.Empty, retornoDto.Mensagem);
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

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirListaItensPedido, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém um pedidoItem pelo ID 
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<PedidoItemDto> retornoDto)
        {
            retornoDto.Retorno = false;
            retornoDto.Mensagem = "Método não implementado para esta entidade";
            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterPedidoItem, Guid.Empty, retornoDto.Mensagem);
            return false;
        }

        /// <summary>
        /// Converte um pedidoItem Dto para um pedidoItem Vo
        /// </summary>
        /// <param name="pedidoItemDto"></param>
        /// <param name="pedidoItemVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(PedidoItemDto pedidoItemDto, ref PedidoItemVo pedidoItemVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(pedidoItemDto, ref pedidoItemVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                pedidoItemVo.DescricaoProduto = string.IsNullOrWhiteSpace(pedidoItemDto.DescricaoProduto) ? "" : pedidoItemDto.DescricaoProduto.Trim();
                pedidoItemVo.IdPedido = pedidoItemDto.IdPedido;
                pedidoItemVo.IdProdutoComposto = (pedidoItemDto.IdProdutoComposto == Guid.Empty) ? null : pedidoItemDto.IdProdutoComposto;
                pedidoItemVo.IdProduto = pedidoItemDto.IdProduto;
                pedidoItemVo.PrecoProduto = pedidoItemDto.PrecoProduto;
                pedidoItemVo.Quantidade = pedidoItemDto.Quantidade;
                pedidoItemVo.TipoProduto = pedidoItemDto.TipoProduto;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o item do pedido para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um pedidoItem Dto para um pedidoItem Vo
        /// </summary>
        /// <param name="pedidoItemVo"></param>
        /// <param name="pedidoItemDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(PedidoItemVo pedidoItemVo, ref PedidoItemDto pedidoItemDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(pedidoItemVo, ref pedidoItemDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                pedidoItemDto.DescricaoProduto = string.IsNullOrWhiteSpace(pedidoItemVo.DescricaoProduto) ? "" : pedidoItemVo.DescricaoProduto.Trim();
                pedidoItemDto.IdPedido = pedidoItemVo.IdPedido;
                pedidoItemDto.IdProdutoComposto = pedidoItemVo.IdProdutoComposto;
                pedidoItemDto.IdProduto = pedidoItemVo.IdProduto;
                pedidoItemDto.PrecoProduto = pedidoItemVo.PrecoProduto;
                pedidoItemDto.Quantidade = pedidoItemVo.Quantidade;
                pedidoItemDto.TipoProduto = pedidoItemVo.TipoProduto;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o item do pedido para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém uma lista de pedidoItems com filtros aplicados, podendo ser paginada
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<PedidoItemDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            IQueryable<PedidoItemVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os itens: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "DESCRICAOPRODUTO":
                        query = query.Where(p => p.DescricaoProduto.Contains(filtro.Value));
                        break;

                    case "PRECOMAIOR":
                        float precoMaior;
                        if (!float.TryParse(filtro.Value, out precoMaior))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de preço (maior que).";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.PrecoProduto >= precoMaior);
                        break;

                    case "PRECOMENOR":
                        float precoMenor;
                        if (!float.TryParse(filtro.Value, out precoMenor))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de preço (menor que).";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.PrecoProduto <= precoMenor);
                        break;

                    case "PRECO":
                        float preco;
                        if (!float.TryParse(filtro.Value, out preco))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de preço.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.PrecoProduto == preco);
                        break;

                    case "TIPOPRODUTO":

                        int tipo;
                        if (!int.TryParse(filtro.Value, out tipo))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de tipo do produto.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.TipoProduto == (TipoProduto)tipo);
                        break;

                    case "IDPEDIDO":

                        Guid idPedido;
                        if (!Guid.TryParse(filtro.Value, out idPedido))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de id do pedido.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.IdPedido == idPedido);
                        break;

                    case "IDPRODUTO":

                        Guid idProduto;
                        if (!Guid.TryParse(filtro.Value, out idProduto))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de id do produto.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.IdProduto == idProduto);
                        break;

                    case "INATIVO":

                        bool filtroInativo;
                        if (!bool.TryParse(filtro.Value, out filtroInativo))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'inativo'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Inativo == filtroInativo);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "DESCRICAOPRODUTO":
                    query = query.OrderBy(p => p.DescricaoProduto).ThenByDescending(p => p.PrecoProduto);
                    break;

                case "PRECOCRESCENTE":
                    query = query.OrderBy(p => p.PrecoProduto).ThenBy(p => p.DescricaoProduto);
                    break;

                case "PRECODESCRESCENTE":
                    query = query.OrderByDescending(p => p.PrecoProduto).ThenBy(p => p.DescricaoProduto);
                    break;

                default:
                    query = query.OrderBy(p => p.DescricaoProduto).ThenByDescending(p => p.PrecoProduto);
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

            List<PedidoItemVo> listaVo = query.ToList();
            foreach (var pedidoItem in listaVo)
            {
                PedidoItemDto pedidoItemDto = new PedidoItemDto();
                if (!ConverterVoParaDto(pedidoItem, ref pedidoItemDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaPedidoItem, pedidoItem.Id, retornoDto.Mensagem);
                    return false;
                }

                if (pedidoItem.IdProduto == UtilitarioBll.RetornaIdProdutoPromocao())
                {
                    retornoDto.ListaEntidades.Insert(0, pedidoItemDto);
                }
                else
                {
                    retornoDto.ListaEntidades.Add(pedidoItemDto);
                }
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita um item não será implementado, pois será feito em conjunto
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<PedidoItemDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            retornoDto.Retorno = false;
            retornoDto.Mensagem = "Método não implementado para esta entidade";
            logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedidoItem, Guid.Empty, retornoDto.Mensagem);
            return false;
        }

        /// <summary>
        /// Edita uma lista de itens de um pedido no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool EditarLista(RequisicaoListaEntidadesDto<PedidoItemDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedidoItem, Guid.Empty, mensagemErro);
                return false;
            }

            if (requisicaoDto.ListaEntidadesDto.Count <= 0)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Informe ao menos um item para editar.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedidoItem, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            List<PedidoItemVo> listaExclusao;
            IQueryable<PedidoItemVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os itens: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedidoItem, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Obter os itens do pedido
            query = query.Where(p => p.IdPedido == requisicaoDto.IdComum);
            listaExclusao = query.ToList();

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
                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarListaPedidoItem, Guid.Empty, mensagemErro);
                    return false;
                }

                PedidoItemVo itemVo = listaExclusao.Where(p => p.Id == item.Id).FirstOrDefault();
                if (itemVo == null)
                {
                    // Se não existir, incluir
                    itemVo = new PedidoItemVo();
                    if (!ConverterDtoParaVo(item, ref itemVo, ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = "Falha ao converter o item do pedido para VO: " + mensagemErro;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
                        return false;
                    }

                    // Prepara a inclusão no banco de dados
                    if (!IncluirBd(itemVo, ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = "Falha ao salvar o item do pedido: " + mensagemErro;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarListaPedidoItem, Guid.Empty, retornoDto.Mensagem);
                        return false;
                    }
                }
                else
                {
                    listaExclusao.Remove(itemVo);

                    // Se existir e mudar algum dado, editar
                    if (item.PrecoProduto != itemVo.PrecoProduto || item.Quantidade != itemVo.Quantidade)
                    {
                        itemVo.Quantidade = item.Quantidade;
                        itemVo.PrecoProduto = item.PrecoProduto;

                        // Prepara a edição no banco de dados
                        if (!EditarBd(itemVo, ref mensagemErro))
                        {
                            retornoDto.Retorno = false;
                            retornoDto.Mensagem = "Falha ao salvar o item do pedido: " + mensagemErro;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedidoItem, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }
                    }
                }
            }

            // Se algum item não veioda requisição
            foreach (var item in listaExclusao)
            {
                // Prepara a exclusão no banco de dados
                if (!ExcluirBd(item.Id, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao excluir o item do pedido: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedidoItem, Guid.Empty, retornoDto.Mensagem);
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

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarPedidoItem, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }


        /// <summary>
        /// Edita uma lista de itens de um pedido no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool EstornarLista(RequisicaoCancelarPedidoDto requisicaoDto, ref RetornoDto retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EstornarListaItensPedido, Guid.Empty, mensagemErro);
                return false;
            }

            List<PedidoItemVo> listaExclusao;
            IQueryable<PedidoItemVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os itens: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EstornarListaItensPedido, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Obter os itens do pedido
            query = query.Where(p => p.IdPedido == requisicaoDto.Id);
            listaExclusao = query.ToList();

            // Se algum item não veioda requisição
            foreach (var item in listaExclusao)
            {
                item.Inativo = true;

                // Prepara a exclusão no banco de dados
                if (!EditarBd(item, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao estornar o item do pedido: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EstornarListaItensPedido, Guid.Empty, retornoDto.Mensagem);
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

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EstornarListaItensPedido, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

    }
}
