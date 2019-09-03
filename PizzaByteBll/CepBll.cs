using PizzaByteBll.Base;
using PizzaByteDal;
using PizzaByteDto;
using PizzaByteDto.Base;
using PizzaByteDto.ClassesBase;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteVo;
using System;
using System.Collections.Generic;
using System.Linq;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteBll
{
    public class CepBll : BaseBll<CepVo, CepDto>
    {
        private bool salvar = true;
        private static LogBll logBll = new LogBll("CepBll");

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public CepBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public CepBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui um cep no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<CepDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Converte para VO a ser incluída no banco de dados
            string mensagemErro = "";
            CepVo cepVo = new CepVo();
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref cepVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o cep para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirCep, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Verifica se o cep já existe
            CepVo cepExistente = null;
            if (!VerificarCepExistente(requisicaoDto.EntidadeDto, ref cepExistente, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Se estiver excluído, restaurar o cadastro antigo
            if (cepExistente != null && cepExistente.Excluido == true)
            {
                cepExistente.Logradouro = cepVo.Logradouro;
                cepExistente.Cidade = cepVo.Cidade;
                cepExistente.Bairro = cepVo.Bairro;
                cepExistente.Cep = cepVo.Cep;
                cepExistente.Excluido = false;

                if (!EditarBd(cepExistente, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao salvar os dados do Cep: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirCep, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }
            // Se não estiver excluído, não permitir incluir duplicado
            else if (cepExistente != null && cepExistente.Excluido == false)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Esse cadastro já existe, não é possível incluir cadastros duplicados.";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirCep, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }
            else
            {
                // Prepara a inclusão no banco de dados
                if (!IncluirBd(cepVo, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao converter o cep para VO: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirCep, Guid.Empty, retornoDto.Mensagem);
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

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirCep, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui um cep do banco de dados a partir do ID
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

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirCep, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém um cep pelo ID 
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<CepDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            CepVo cepVo;

            if (!ObterPorIdBd(requisicaoDto.Id, out cepVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o cep: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterCep, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Mensagem = "Ok";
            if (cepVo == null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Cep não encontrado";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterCep, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            CepDto cepDto = new CepDto();
            if (!ConverterVoParaDto(cepVo, ref cepDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o cep: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterCep, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Entidade = cepDto;
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtem o endereço pelo CEP
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ObterPorCep(RequisicaoObterCepPorCepDto requisicaoDto, ref RetornoObterDto<CepDto> retornoDto)
        {
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            if (string.IsNullOrWhiteSpace(requisicaoDto.Cep))
            {
                retornoDto.Mensagem = $"Informe um CEP para obter o endereço";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterCepPorCep, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Obter a query primária
            IQueryable<CepVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os CEPs: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterCepPorCep, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            requisicaoDto.Cep = requisicaoDto.Cep.Replace("-", "");
            query = query.Where(p => p.Inativo == false && p.Cep == requisicaoDto.Cep.Trim());
            CepVo cepVo = query.FirstOrDefault();

            if (cepVo == null)
            {
                retornoDto.Retorno = true;
                retornoDto.Mensagem = "CEP não encontrado";
                retornoDto.Entidade = null;

                return true;
            }
            else
            {
                CepDto cepDto = new CepDto();
                if (!ConverterVoParaDto(cepVo, ref cepDto, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = $"Erro ao converter o CEP para DTO: {mensagemErro}";

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterCepPorCep, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }

                retornoDto.Retorno = true;
                retornoDto.Mensagem = "Ok";
                retornoDto.Entidade = cepDto;

                return true;
            }
        }

        /// <summary>
        /// Converte um cep Dto para um cep Vo
        /// </summary>
        /// <param name="cepDto"></param>
        /// <param name="cepVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(CepDto cepDto, ref CepVo cepVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(cepDto, ref cepVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                cepVo.Cep = string.IsNullOrWhiteSpace(cepDto.Cep) ? "" : cepDto.Cep.Trim().Replace("-", "");
                cepVo.Bairro = string.IsNullOrWhiteSpace(cepDto.Bairro) ? "" : cepDto.Bairro.Trim();
                cepVo.Cidade = string.IsNullOrWhiteSpace(cepDto.Cidade) ? "" : cepDto.Cidade.Trim();
                cepVo.Logradouro = string.IsNullOrWhiteSpace(cepDto.Logradouro) ? "" : cepDto.Logradouro.Trim();

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o cep para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um cep Dto para um cep Vo
        /// </summary>
        /// <param name="cepVo"></param>
        /// <param name="cepDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(CepVo cepVo, ref CepDto cepDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(cepVo, ref cepDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                cepDto.Cep = string.IsNullOrWhiteSpace(cepVo.Cep) ? "" : cepVo.Cep.Trim();
                cepDto.Bairro = string.IsNullOrWhiteSpace(cepVo.Bairro) ? "" : cepVo.Bairro.Trim();
                cepDto.Cidade = string.IsNullOrWhiteSpace(cepVo.Cidade) ? "" : cepVo.Cidade.Trim();
                cepDto.Logradouro = string.IsNullOrWhiteSpace(cepVo.Logradouro) ? "" : cepVo.Logradouro.Trim();

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o cep para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém uma lista de CEPs com filtros aplicados, podendo ser paginada
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<CepDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            IQueryable<CepVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os CEPs: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaCep, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "CEP":
                        string cep = filtro.Value.Replace("-", "");
                        query = query.Where(p => p.Cep.Contains(cep));
                        break;

                    case "LOGRADOURO":
                        query = query.Where(p => p.Logradouro.Contains(filtro.Value));
                        break;

                    case "CIDADE":
                        query = query.Where(p => p.Cidade.Contains(filtro.Value));
                        break;

                    case "BAIRRO":
                        query = query.Where(p => p.Bairro.Contains(filtro.Value));
                        break;

                    case "INATIVO":

                        bool filtroInativo;
                        if (!bool.TryParse(filtro.Value, out filtroInativo))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'inativo'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaCep, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Inativo == filtroInativo);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaCep, Guid.Empty, retornoDto.Mensagem);
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "LOGRADOURO":
                    query = query.OrderBy(p => p.Logradouro).ThenBy(p => p.Bairro).ThenBy(p => p.Cidade);
                    break;

                case "BAIRRO":
                    query = query.OrderBy(p => p.Bairro).ThenBy(p => p.Cidade).ThenBy(p => p.Logradouro);
                    break;

                case "CIDADE":
                    query = query.OrderBy(p => p.Cidade).ThenBy(p => p.Logradouro).ThenBy(p => p.Bairro);
                    break;

                default:
                    query = query.OrderBy(p => p.Logradouro).ThenBy(p => p.Bairro).ThenBy(p => p.Cidade);
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

            List<CepVo> listaVo = query.ToList();
            foreach (var cep in listaVo)
            {
                CepDto cepDto = new CepDto();
                if (!ConverterVoParaDto(cep, ref cepDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaCep, cep.Id, retornoDto.Mensagem);
                    return false;
                }

                retornoDto.ListaEntidades.Add(cepDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita um cep
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<CepDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Apenas usuários ADM podem editar cepes
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para editar os cepes é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCep, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            // Não deixar incluir um cep repetido
            CepVo cepVo = new CepVo();
            if (!VerificarCepExistente(requisicaoDto.EntidadeDto, ref cepVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao validar o Cpf: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCep, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (cepVo != null && cepVo.Excluido == true)
            {
                if (!ExcluirDefinitivoBd(cepVo.Id, ref mensagemErro))
                {
                    mensagemErro = $"Houve um erro ao deletar o cep duplicado.";
                    return false;
                }

                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao excluir o cep duplicado: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCep, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }
            else if (cepVo != null && cepVo.Excluido == false)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Esse cadastro já existe, não é possível incluir cadastros duplicados";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCep, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out cepVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar o cep: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCep, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref cepVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter o cep para Vo: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCep, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!EditarBd(cepVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados do cep: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCep, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao salvar os novos dados: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCep, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtem a lista de bairros cadastrados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="listaBairros"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool ObterListaBairros(BaseRequisicaoDto requisicaoDto, ref List<BairroCidadeDto> listaBairros, ref string mensagemErro)
        {
            IQueryable<CepVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                mensagemErro = $"Houve um problema ao listar os bairros: {mensagemErro}";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaBairro, Guid.Empty, mensagemErro);
                return false;
            }

            try
            {
                query = query.Where(p => p.Inativo == false).OrderBy(p => p.Bairro);
                listaBairros = query.GroupBy(p => new { p.Cidade, p.Bairro }).Select(p => new BairroCidadeDto { Cidade = p.Key.Cidade, Bairro = p.Key.Bairro }).ToList();
                return true;
            }
            catch (Exception)
            {
                mensagemErro = $"Houve um problema ao listar os bairros: {mensagemErro}";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaBairro, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtem a cidade que o bairro pertence
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="bairro"></param>
        /// <param name="cidade"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool ObterCidadePorBairro(BaseRequisicaoDto requisicaoDto, string bairro, ref string cidade, ref string mensagemErro)
        {
            if (string.IsNullOrWhiteSpace(bairro))
            {
                mensagemErro = "Informe o bairro para obter a cidade";
                return false;
            }

            // Obter a query primária
            IQueryable<CepVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                mensagemErro = $"Houve um problema ao obter a cidade: {mensagemErro}";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterCidadePorBairro, Guid.Empty, mensagemErro);
                return false;
            }

            try
            {
                bairro = bairro.Trim();
                CepVo endereco = query.Where(p => p.Bairro == bairro).FirstOrDefault();
                cidade = endereco == null ? "" : endereco.Cidade.Trim();
                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = $"Houve um problema ao obter a cidade: {ex.Message}";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterCidadePorBairro, Guid.Empty, mensagemErro);
                return false;
            }
        }

        /// <summary>
        /// Obtem uma lista de CEP por id
        /// </summary>
        /// <param name="listaIds"></param>
        /// <param name="listaCeps"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool ObterListaEnderecosPorId(List<Guid> listaIds, ref List<CepVo> listaCeps, ref string mensagemErro)
        {
            if (listaIds.Count <= 0)
            {
                return true;
            }

            // Obter a query primária
            IQueryable<CepVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                mensagemErro = $"Houve um problema ao listar os CEPs: {mensagemErro}";
                return false;
            }

            try
            {
                query = query.Where(p => listaIds.Contains(p.Id));
                listaCeps = query.ToList();
                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = $"Houve um problema para obter a lista de CEPs: {ex.Message}";
                return false;
            }

        }

        /// <summary>
        /// Verifica se um CEP já existe na tabela
        /// </summary>
        /// <param name="cepVo"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool VerificarExistente(BaseRequisicaoDto requisicaoDto, string cep, out CepVo cepVo, ref string mensagemErro)
        {
            cepVo = null;
            if (string.IsNullOrWhiteSpace(cep))
            {
                mensagemErro = "Informe um CEP para verificar a existência no banco de dados";
                return false;
            }

            // Obter a query primária
            IQueryable<CepVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro, true))
            {
                mensagemErro = $"Houve um problema ao listar os CEPs: {mensagemErro}";
                return false;
            }

            try
            {
                cep = cep.Replace("-", "");
                query = query.Where(p => p.Cep == cep.Trim());
                cepVo = query.FirstOrDefault();

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Valida se o Cep já existe
        /// </summary>
        /// <param name="cepDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool VerificarCepExistente(CepDto cepDto, ref CepVo cepVo, ref string mensagemErro)
        {
            if (cepDto == null)
            {
                mensagemErro = "É necessário informar o cep para validar o Cpf";
                return false;
            }

            if (string.IsNullOrWhiteSpace(cepDto.Cep))
            {
                return true;
            }

            IQueryable<CepVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro, true))
            {
                mensagemErro = $"Houve um problema ao listar os CEPs: {mensagemErro}";
                return false;
            }

            cepDto.Cep = cepDto.Cep.Replace(".", "").Replace("/", "").Replace("-", "");
            query = query.Where(p => p.Cep == cepDto.Cep.Trim() && p.Id != cepDto.Id);
            cepVo = query.FirstOrDefault();
            return true;

        }

    }
}

