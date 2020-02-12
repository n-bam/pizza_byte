using PizzaByteBll.Base;
using PizzaByteDal;
using PizzaByteDto;
using PizzaByteDto.ClassesBase;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteVo;
using System;
using System.Collections.Generic;
using System.Linq;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteBll
{
    public class TaxaEntregaBll : BaseBll<TaxaEntregaVo, TaxaEntregaDto>
    {
        private static LogBll logBll = new LogBll("Taxa de entregaBll");
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public TaxaEntregaBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public TaxaEntregaBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Processa uma lista de taxas, editando as existentes e incluindo as novas
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool IncluirEditarLista(RequisicaoListaEntidadesDto<TaxaEntregaDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (requisicaoDto.ListaEntidadesDto.Count <= 0)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "É necessário ter ao menos uma taxa de entrega para incluir a lista de taxas.";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirAlterarListaTaxaEntrega, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirAlterarListaTaxaEntrega, Guid.Empty, mensagemErro);
                return false;
            }

            // Para cada taxa
            foreach (var taxa in requisicaoDto.ListaEntidadesDto)
            {
                // Se existir, editar os dados
                if (taxa.Id != Guid.Empty)
                {
                    TaxaEntregaVo taxaEntregaVo;
                    if (!ObterPorIdBd(taxa.Id, out taxaEntregaVo, ref mensagemErro, true))
                    {
                        retornoDto.Mensagem = "Erro ao obter a taxa de entrega: " + mensagemErro;
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirAlterarListaTaxaEntrega, taxa.Id, retornoDto.Mensagem);
                        return false;
                    }

                    if (!taxa.ValidarEntidade(ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = mensagemErro;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirAlterarListaTaxaEntrega, Guid.Empty, mensagemErro);
                        return false;
                    }

                    // Converte para VO a ser alterado no banco de dados
                    if (!ConverterDtoParaVo(taxa, ref taxaEntregaVo, ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = "Falha ao converter a taxa de entrega para VO: " + mensagemErro;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirAlterarListaTaxaEntrega, Guid.Empty, retornoDto.Mensagem);
                        return false;
                    }

                    if (!EditarBd(taxaEntregaVo, ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = "Falha ao salvar os novos dados da taxa de entrega: " + mensagemErro;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirAlterarListaTaxaEntrega, taxaEntregaVo.Id, retornoDto.Mensagem);
                        return false;
                    }
                }
                else // Se não existir, incluir
                {
                    taxa.Id = Guid.NewGuid();
                    if (!taxa.ValidarEntidade(ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = mensagemErro;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirAlterarListaTaxaEntrega, Guid.Empty, mensagemErro);
                        return false;
                    }

                    // Converte para VO a ser incluída no banco de dados
                    TaxaEntregaVo taxaEntregaVo = new TaxaEntregaVo();
                    if (!ConverterDtoParaVo(taxa, ref taxaEntregaVo, ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = "Falha ao converter a taxa de entrega para VO: " + mensagemErro;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirAlterarListaTaxaEntrega, Guid.Empty, retornoDto.Mensagem);
                        return false;
                    }

                    // Prepara a inclusão no banco de dados
                    if (!IncluirBd(taxaEntregaVo, ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = $"Falha ao incluir a taxa de entrega ({taxaEntregaVo.BairroCidade}) para VO: " + mensagemErro;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirAlterarListaTaxaEntrega, Guid.Empty, retornoDto.Mensagem);
                        return false;
                    }
                }
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirTaxaEntrega, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Mensagem = "OK";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Inclui um taxaEntrega no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<TaxaEntregaDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            TaxaEntregaVo taxaEntregaVo = new TaxaEntregaVo();
            string mensagemErro = "";

            // Converte para VO a ser incluída no banco de dados
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref taxaEntregaVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter a taxa de entrega para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirTaxaEntrega, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(taxaEntregaVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao incluir a taxa de entrega para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirTaxaEntrega, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirTaxaEntrega, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui um taxaEntrega do banco de dados a partir do ID
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para excluir taxas de entrega é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirTaxaEntrega, requisicaoDto.Id, retornoDto.Mensagem);
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

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirTaxaEntrega, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirTaxaEntrega, requisicaoDto.Id, "Taxa de entrega excluída.");
            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém uma lista com todos os bairros e suas respectivas taxas
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterListaBairrosComTaxa(BaseRequisicaoDto requisicaoDto, ref RetornoObterListaDto<TaxaEntregaDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaBairrosComTaxas, Guid.Empty, mensagemErro);
                return false;
            }

            CepBll cepBll = new CepBll(false);
            List<BairroCidadeDto> listaBairros = new List<BairroCidadeDto>();

            // Obter os bairros cadastrados
            if (!cepBll.ObterListaBairros(requisicaoDto, ref listaBairros, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Obter a query primária
            IQueryable<TaxaEntregaVo> query;
            List<TaxaEntregaVo> listaTaxasCadastradas;

            if (!this.ObterQueryBd(out query, ref mensagemErro, true))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as taxas de entrega: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaBairrosComTaxas, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            try
            {
                query = query.OrderBy(p => p.BairroCidade);
                listaTaxasCadastradas = query.ToList();
            }
            catch (Exception)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = $"Houve um problema ao listar os bairros: {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaBairrosComTaxas, Guid.Empty, mensagemErro);
                return false;
            }

            // Para cada bairro
            foreach (var bairro in listaBairros)
            {
                // Iniciar taxa
                TaxaEntregaDto taxaDto = new TaxaEntregaDto()
                {
                    BairroCidade = bairro.Bairro + "_" + bairro.Cidade.Trim(),
                    Cidade = bairro.Cidade,
                    Id = Guid.Empty,
                    ValorTaxa = 0
                };

                TaxaEntregaVo taxaVo = listaTaxasCadastradas.Where(p => p.BairroCidade.Trim() == (bairro.Bairro.Trim() + "_" + bairro.Cidade.Trim())).FirstOrDefault();
                if (taxaVo != null)
                {
                    if (!ConverterVoParaDto(taxaVo, ref taxaDto, ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = $"Falha ao converter a taxa para DTO: {mensagemErro}";

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaBairrosComTaxas, Guid.Empty, mensagemErro);
                        return false;
                    }

                    if (taxaVo.Excluido)
                    {
                        taxaDto.ValorTaxa = 0;
                    }
                }

                retornoDto.ListaEntidades.Add(taxaDto);
            }

            retornoDto.ListaEntidades = retornoDto.ListaEntidades.OrderBy(p => p.Cidade).ThenBy(p => p.BairroCidade).ToList();
            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém uma lista de taxaEntregaes com filtros aplicados, podendo ser paginada
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<TaxaEntregaDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            IQueryable<TaxaEntregaVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as taxas de entrega: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaTaxaEntrega, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "BAIRRO":
                        query = query.Where(p => p.BairroCidade.Contains(filtro.Value));
                        break;

                    case "VALORTAXAMAIOR":
                        float valorMaior;
                        if (!float.TryParse(filtro.Value, out valorMaior))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de valor (maior que).";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaTaxaEntrega, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.ValorTaxa >= valorMaior);
                        break;

                    case "VALORTAXAMENOR":
                        float valorMenor;
                        if (!float.TryParse(filtro.Value, out valorMenor))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de valor (menor que).";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaTaxaEntrega, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.ValorTaxa <= valorMenor);
                        break;

                    case "TAXAENTREGA":
                        float preco;
                        if (!float.TryParse(filtro.Value, out preco))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de preço.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaTaxaEntrega, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.ValorTaxa == preco);
                        break;

                    case "INATIVO":

                        bool filtroInativo;
                        if (!bool.TryParse(filtro.Value, out filtroInativo))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'inativo'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaTaxaEntrega, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Inativo == filtroInativo);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaTaxaEntrega, Guid.Empty, retornoDto.Mensagem);
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "BAIRRO":
                    query = query.OrderBy(p => p.BairroCidade).ThenBy(p => p.ValorTaxa);
                    break;

                case "VALORTAXACRESCENTE":
                    query = query.OrderBy(p => p.ValorTaxa).ThenBy(p => p.BairroCidade);
                    break;

                case "VALORTAXADESCRESCENTE":
                    query = query.OrderByDescending(p => p.ValorTaxa).ThenBy(p => p.BairroCidade);
                    break;

                default:
                    query = query.OrderBy(p => p.BairroCidade).ThenBy(p => p.ValorTaxa);
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

            List<TaxaEntregaVo> listaVo = query.ToList();
            foreach (var taxaEntrega in listaVo)
            {
                TaxaEntregaDto taxaEntregaDto = new TaxaEntregaDto();
                if (!ConverterVoParaDto(taxaEntrega, ref taxaEntregaDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaTaxaEntrega, taxaEntrega.Id, retornoDto.Mensagem);
                    return false;
                }

                retornoDto.ListaEntidades.Add(taxaEntregaDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtém um taxaEntrega pelo ID 
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<TaxaEntregaDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            TaxaEntregaVo taxaEntregaVo;

            if (!ObterPorIdBd(requisicaoDto.Id, out taxaEntregaVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter a taxa de entrega: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterTaxaEntrega, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }
                       
            TaxaEntregaDto taxaEntregaDto = new TaxaEntregaDto();
            if (!ConverterVoParaDto(taxaEntregaVo, ref taxaEntregaDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter a taxa de entrega: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterTaxaEntrega, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            CepBll cepBll = new CepBll(false);
            string cidade = "";
            if (!cepBll.ObterCidadePorBairro(requisicaoDto, taxaEntregaDto.BairroCidade, ref cidade, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter a cidade que a taxa se refere: " + mensagemErro;
                retornoDto.Retorno = false;
            }

            taxaEntregaDto.Cidade = cidade;
            retornoDto.Entidade = taxaEntregaDto;

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte um taxaEntrega Dto para um taxaEntrega Vo
        /// </summary>
        /// <param name="taxaEntregaDto"></param>
        /// <param name="taxaEntregaVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(TaxaEntregaDto taxaEntregaDto, ref TaxaEntregaVo taxaEntregaVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(taxaEntregaDto, ref taxaEntregaVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                taxaEntregaVo.BairroCidade = string.IsNullOrWhiteSpace(taxaEntregaDto.BairroCidade) ? "" : taxaEntregaDto.BairroCidade.Trim();
                taxaEntregaVo.ValorTaxa = taxaEntregaDto.ValorTaxa;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a taxa de entrega para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um taxaEntrega Dto para um taxaEntrega Vo
        /// </summary>
        /// <param name="taxaEntregaVo"></param>
        /// <param name="taxaEntregaDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(TaxaEntregaVo taxaEntregaVo, ref TaxaEntregaDto taxaEntregaDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(taxaEntregaVo, ref taxaEntregaDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                taxaEntregaDto.BairroCidade = string.IsNullOrWhiteSpace(taxaEntregaVo.BairroCidade) ? "" : taxaEntregaVo.BairroCidade.Trim();
                taxaEntregaDto.ValorTaxa = taxaEntregaVo.ValorTaxa;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a taxa de entrega para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Edita um taxaEntrega
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<TaxaEntregaDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            TaxaEntregaVo taxaEntregaVo = new TaxaEntregaVo();
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out taxaEntregaVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar a taxa de entrega: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarTaxaEntrega, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref taxaEntregaVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter a taxa de entrega para Vo: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarTaxaEntrega, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!EditarBd(taxaEntregaVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao salvar os novos dados da taxa de entrega: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarTaxaEntrega, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarTaxaEntrega, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém uma taxa de entrega pelo bairro, se não existir, retorna nulo
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterTaxaPorBairro(RequisicaoObterTaxaPorBairroDto requisicaoDto, ref RetornoObterDto<TaxaEntregaDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterTaxaPorBairro, Guid.Empty, mensagemErro);
                return false;
            }

            // Obter a query primária
            IQueryable<TaxaEntregaVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as taxas de entrega: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterTaxaPorBairro, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            string bairro = string.IsNullOrWhiteSpace(requisicaoDto.BairroCidade) ? "" : requisicaoDto.BairroCidade.Trim();
            query = query.Where(p => p.BairroCidade == bairro);

            TaxaEntregaVo taxaEntregaVo = query.FirstOrDefault();
            if (taxaEntregaVo != null)
            {
                TaxaEntregaDto taxaEntregaDto = new TaxaEntregaDto();
                if (!ConverterVoParaDto(taxaEntregaVo, ref taxaEntregaDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterTaxaPorBairro, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }

                retornoDto.Entidade = taxaEntregaDto;
            }
            else
            {
                retornoDto.Entidade = null;
            }

            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Inclui ou atualiza os dados de uma taxa de entrega
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="taxaEntregaDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool IncluirEditar(BaseRequisicaoDto requisicaoDto, TaxaEntregaDto taxaEntregaDto, ref string mensagemErro)
        {
            if (taxaEntregaDto == null)
            {
                mensagemErro = "Preencha os dados da taxa que deseja incluir/editar.";
                return false;
            }

            RequisicaoObterTaxaPorBairroDto requisicaoObterDto = new RequisicaoObterTaxaPorBairroDto()
            {
                Identificacao = requisicaoDto.Identificacao,
                IdUsuario = requisicaoDto.IdUsuario,
                BairroCidade = taxaEntregaDto.BairroCidade
            };

            RetornoObterDto<TaxaEntregaDto> retornoObterDto = new RetornoObterDto<TaxaEntregaDto>();
            if (!ObterTaxaPorBairro(requisicaoObterDto, ref retornoObterDto))
            {
                mensagemErro = retornoObterDto.Mensagem;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirEditarTaxaEntrega, taxaEntregaDto.Id, mensagemErro);
                return false;
            }

            // Incluir se não existir
            if (retornoObterDto.Entidade == null)
            {
                taxaEntregaDto.Id = Guid.NewGuid();
                RequisicaoEntidadeDto<TaxaEntregaDto> requisicaoIncluirDto = new RequisicaoEntidadeDto<TaxaEntregaDto>()
                {
                    EntidadeDto = taxaEntregaDto,
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
                if (taxaEntregaDto.ValorTaxa != retornoObterDto.Entidade.ValorTaxa)
                {
                    retornoObterDto.Entidade.ValorTaxa = taxaEntregaDto.ValorTaxa;
                    RequisicaoEntidadeDto<TaxaEntregaDto> requisicaoEditarDto = new RequisicaoEntidadeDto<TaxaEntregaDto>()
                    {
                        EntidadeDto = retornoObterDto.Entidade,
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

    }
}
