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
    public class FuncionarioBll : BaseBll<FuncionarioVo, FuncionarioDto>
    {
        private static LogBll logBll = new LogBll("FuncionarioBll");
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public FuncionarioBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public FuncionarioBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui um funcionario no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<FuncionarioDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            FuncionarioVo funcionarioVo = new FuncionarioVo();
            string mensagemErro = "";

            // Converte para VO a ser incluída no banco de dados
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref funcionarioVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o funcionario para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirFuncionario, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(funcionarioVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o funcionario para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirFuncionario, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirFuncionario, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui um funcionario do banco de dados a partir do ID
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para excluir funcionarios é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirFuncionario, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirFuncionario, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém um funcionario pelo ID 
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<FuncionarioDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            FuncionarioVo funcionarioVo;

            if (!ObterPorIdBd(requisicaoDto.Id, out funcionarioVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o funcionario: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterFuncionario, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Mensagem = "Ok";
            if (funcionarioVo == null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Funcionario não encontrado";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterFuncionario, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            FuncionarioDto funcionarioDto = new FuncionarioDto();
            if (!ConverterVoParaDto(funcionarioVo, ref funcionarioDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o funcionario: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterFuncionario, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Entidade = funcionarioDto;
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte um funcionario Dto para um funcionario Vo
        /// </summary>
        /// <param name="funcionarioDto"></param>
        /// <param name="funcionarioVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(FuncionarioDto funcionarioDto, ref FuncionarioVo funcionarioVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(funcionarioDto, ref funcionarioVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                funcionarioVo.Nome = string.IsNullOrWhiteSpace(funcionarioDto.Nome) ? "" : funcionarioDto.Nome.Trim();
                funcionarioVo.Telefone = string.IsNullOrWhiteSpace(funcionarioDto.Telefone) ? "" : funcionarioDto.Telefone.Trim().Replace("(", "").Replace(")", "").Replace("-", "");
                funcionarioVo.Tipo = funcionarioDto.Tipo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o funcionario para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um funcionario Dto para um funcionario Vo
        /// </summary>
        /// <param name="funcionarioVo"></param>
        /// <param name="funcionarioDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(FuncionarioVo funcionarioVo, ref FuncionarioDto funcionarioDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(funcionarioVo, ref funcionarioDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                funcionarioDto.Nome = string.IsNullOrWhiteSpace(funcionarioVo.Nome) ? "" : funcionarioVo.Nome.Trim();
                funcionarioDto.Telefone = string.IsNullOrWhiteSpace(funcionarioVo.Telefone) ? "" : funcionarioVo.Telefone.Trim().Replace("(", "").Replace(")", "").Replace("-", "");
                funcionarioDto.Tipo = funcionarioVo.Tipo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o funcionario para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém uma lista de funcionarios com filtros aplicados, podendo ser paginada
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<FuncionarioDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            IQueryable<FuncionarioVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os funcionarios: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaFuncionario, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "NOME":
                        query = query.Where(p => p.Nome.Contains(filtro.Value));
                        break;

                    case "TELEFONE":
                        string telefone = filtro.Value.Replace("(", "").Replace(")", "").Replace("-", "");
                        query = query.Where(p => p.Telefone.Contains(telefone));
                        break;

                    case "TIPO":

                        int tipo;
                        if (!int.TryParse(filtro.Value, out tipo))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de tipo.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaFuncionario, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Tipo == (TipoFuncionario)tipo);
                        break;

                    case "INATIVO":

                        bool filtroInativo;
                        if (!bool.TryParse(filtro.Value, out filtroInativo))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'inativo'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaFuncionario, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Inativo == filtroInativo);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaFuncionario, Guid.Empty, retornoDto.Mensagem);
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "NOME":
                    query = query.OrderBy(p => p.Nome);
                    break;

                case "TELEFONE":
                    query = query.OrderBy(p => p.Telefone);
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

            List<FuncionarioVo> listaVo = query.ToList();
            foreach (var funcionario in listaVo)
            {
                FuncionarioDto funcionarioDto = new FuncionarioDto();
                if (!ConverterVoParaDto(funcionario, ref funcionarioDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaFuncionario, funcionario.Id, retornoDto.Mensagem);
                    return false;
                }

                retornoDto.ListaEntidades.Add(funcionarioDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita um funcionario
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<FuncionarioDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            FuncionarioVo funcionarioVo = new FuncionarioVo();
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out funcionarioVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar o funcionario: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarFuncionario, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref funcionarioVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter o funcionario para Vo: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarFuncionario, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!EditarBd(funcionarioVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao salvar os novos dados do funcionario: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarFuncionario, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarFuncionario, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }
    }
}
