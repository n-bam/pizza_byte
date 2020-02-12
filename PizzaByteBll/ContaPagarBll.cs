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
    public class ContaPagarBll : BaseBll<ContaPagarVo, ContaPagarDto>
    {
        private static LogBll logBll = new LogBll("ContaPagarBll");
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public ContaPagarBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public ContaPagarBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui uma conta no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<ContaPagarDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            ContaPagarVo contaPagarVo = new ContaPagarVo();
            string mensagemErro = "";

            // Converte para VO a ser incluída no banco de dados
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref contaPagarVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter a conta para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirContaPagar, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(contaPagarVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter a conta para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirContaPagar, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirContaPagar, Guid.Empty, retornoDto.Mensagem);
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

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirContaPagar, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirContaPagar, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirContaPagar, requisicaoDto.Id, "Conta a pagar excluída.");
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
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<ContaPagarDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            ContaPagarVo contaPagarVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out contaPagarVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter a conta: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterContaPagar, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            ContaPagarDto contaPagarDto = new ContaPagarDto();
            if (!ConverterVoParaDto(contaPagarVo, ref contaPagarDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter a conta: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterContaPagar, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (contaPagarDto.IdFornecedor != null && contaPagarDto.IdFornecedor != Guid.Empty)
            {
                requisicaoDto.Id = contaPagarDto.IdFornecedor.Value;
                RetornoObterDto<FornecedorDto> retornoFornecedorDto = new RetornoObterDto<FornecedorDto>();

                FornecedorBll fornecedorBll = new FornecedorBll(pizzaByteContexto, false);
                if (!fornecedorBll.Obter(requisicaoDto, ref retornoFornecedorDto))
                {
                    retornoDto.Mensagem = retornoFornecedorDto.Mensagem;
                    retornoDto.Retorno = false;

                    return false;
                }

                if (retornoFornecedorDto.Mensagem == "Erro ao obter o fornecedor: Cadastro não encontrado")
                {
                    contaPagarDto.NomeFornecedor = "Cadastro não encontro";
                }
                else
                {
                    contaPagarDto.NomeFornecedor = retornoFornecedorDto.Entidade.NomeFantasia + " (CNPJ: " + UtilitarioBll.RetornarCnpjFormatado(retornoFornecedorDto.Entidade.Cnpj) + ")";
                }
            }


            retornoDto.Entidade = contaPagarDto;
            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte de dto para vo
        /// </summary>
        /// <param name="contaPagarDto"></param>
        /// <param name="contaPagarVo"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(ContaPagarDto contaPagarDto, ref ContaPagarVo contaPagarVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(contaPagarDto, ref contaPagarVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                contaPagarVo.Descricao = string.IsNullOrWhiteSpace(contaPagarDto.Descricao) ? "" : contaPagarDto.Descricao.Trim();
                contaPagarVo.Valor = contaPagarDto.Valor;
                contaPagarVo.Status = contaPagarDto.Status;
                contaPagarVo.IdFornecedor = contaPagarDto.IdFornecedor;
                contaPagarVo.DataCompetencia = contaPagarDto.DataCompetencia;
                contaPagarVo.DataVencimento = contaPagarDto.DataVencimento;
                contaPagarVo.DataPagamento = contaPagarDto.DataPagamento;

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
        /// <param name="contaPagarVo"></param>
        /// <param name="contaPagarDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(ContaPagarVo contaPagarVo, ref ContaPagarDto contaPagarDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(contaPagarVo, ref contaPagarDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                contaPagarDto.Descricao = string.IsNullOrWhiteSpace(contaPagarVo.Descricao) ? "" : contaPagarVo.Descricao.Trim();
                contaPagarDto.Valor = contaPagarVo.Valor;
                contaPagarDto.IdFornecedor = contaPagarVo.IdFornecedor;
                contaPagarDto.Status = contaPagarVo.Status;
                contaPagarDto.DataCompetencia = contaPagarVo.DataCompetencia;
                contaPagarDto.DataPagamento = contaPagarVo.DataPagamento;
                contaPagarDto.DataVencimento = contaPagarVo.DataVencimento;

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
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<ContaPagarDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            IQueryable<ContaPagarVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as contas: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaPagar, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "DESCRIÇÂO":
                        query = query.Where(p => p.Descricao.Contains(filtro.Value));
                        break;


                    case "PRECOMAIOR":
                        float valorMaior;
                        if (!float.TryParse(filtro.Value, out valorMaior))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de preço (maior que).";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaProduto, Guid.Empty, retornoDto.Mensagem);
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

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaProduto, Guid.Empty, retornoDto.Mensagem);
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

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaProduto, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }
                        query = query.Where(p => p.Valor == Valor);
                        break;

                    case "STATUS":

                        int status;
                        if (!int.TryParse(filtro.Value, out status))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de status.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaPagar, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Status == (StatusConta)status);
                        break;

                    case "DATAINICIOCOMPETENCIA":

                        DateTime data;
                        if (!DateTime.TryParse(filtro.Value, out data))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'data de competência'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterContaPagar, Guid.Empty, retornoDto.Mensagem);
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

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterContaPagar, Guid.Empty, retornoDto.Mensagem);
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

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterContaPagar, Guid.Empty, retornoDto.Mensagem);
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

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterContaPagar, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataVencimento) <= dataFimVencimento);
                        break;

                    case "DATAINICIOPAGAMENTO":

                        DateTime dataPagamento;
                        if (!DateTime.TryParse(filtro.Value, out dataPagamento))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'data de pagamento'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterContaPagar, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataPagamento) >= dataPagamento);
                        break;

                    case "DATAFIMPAGAMENTO":

                        DateTime dataFimPagamento;
                        if (!DateTime.TryParse(filtro.Value, out dataFimPagamento))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'data de pagamento'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterContaPagar, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => DbFunctions.TruncateTime(p.DataPagamento) <= dataFimPagamento);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaPagar, Guid.Empty, retornoDto.Mensagem);
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {

                case "DESCRIÇÂO":
                    query = query.OrderBy(p => p.Descricao);
                    break;

                case "VALOR":
                    query = query.OrderBy(p => p.Valor);
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

            List<ContaPagarVo> listaVo = query.ToList();
            foreach (var contaPagar in listaVo)
            {
                ContaPagarDto contaPagarDto = new ContaPagarDto();
                if (!ConverterVoParaDto(contaPagar, ref contaPagarDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaContaPagar, contaPagar.Id, retornoDto.Mensagem);
                    return false;
                }

                retornoDto.ListaEntidades.Add(contaPagarDto);
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
        public override bool Editar(RequisicaoEntidadeDto<ContaPagarDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            ContaPagarVo contaPagarVo = new ContaPagarVo();
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out contaPagarVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar a conta: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarContaPagar, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref contaPagarVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter a conta para Vo: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarContaPagar, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!EditarBd(contaPagarVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao salvar os novos dados da conta: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarContaPagar, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarContaPagar, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }
    }
}
