using PizzaByteBll.Base;
using PizzaByteDal;
using PizzaByteDto.Base;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteVo;
using System;
using System.Collections.Generic;
using System.Linq;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteBll
{
    public class FornecedorBll : BaseBll<FornecedorVo, FornecedorDto>
    {
        private static LogBll logBll = new LogBll("FornecedorBll");
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public FornecedorBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public FornecedorBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui um fornecedor no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<FornecedorDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Converte para VO a ser incluída no banco de dados
            string mensagemErro = "";
            FornecedorVo fornecedorVo = new FornecedorVo();
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref fornecedorVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o fornecedor para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirFornecedor, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Verifica se o fornecedor já existe
            FornecedorVo fornecedorExistente = null;
            if (!VerificarFornecedorExistente(requisicaoDto.EntidadeDto, ref fornecedorExistente, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Se estiver excluído, restaurar o cadastro antigo
            if (fornecedorExistente != null && fornecedorExistente.Excluido == true)
            {
                fornecedorExistente.NomeFantasia = fornecedorVo.NomeFantasia;
                fornecedorExistente.RazaoSocial = fornecedorVo.RazaoSocial;
                fornecedorExistente.Telefone = fornecedorVo.Telefone;
                fornecedorExistente.Cnpj = fornecedorVo.Cnpj;
                fornecedorExistente.NumeroEndereco = fornecedorVo.NumeroEndereco;
                fornecedorExistente.ComplementoEndereco = fornecedorVo.ComplementoEndereco;
                fornecedorExistente.Obs = fornecedorVo.Obs;
                fornecedorExistente.IdCep = fornecedorVo.IdCep;
                fornecedorExistente.Endereco = fornecedorVo.Endereco;
                fornecedorExistente.Excluido = false;

                if (!EditarBd(fornecedorExistente, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao salvar os dados do produto: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirFornecedor, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }
            // Se não estiver excluído, não permitir incluir duplicado
            else if (fornecedorExistente != null && fornecedorExistente.Excluido == false)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Esse cadastro (fornecedor) já existe, não é possível incluir cadastros duplicados.";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirFornecedor, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }
            else
            {
                // Prepara a inclusão no banco de dados
                if (!IncluirBd(fornecedorVo, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao converter o fornecedor para VO: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirFornecedor, Guid.Empty, retornoDto.Mensagem);
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

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirFornecedor, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Edita um fornecedor
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<FornecedorDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Apenas usuários ADM podem editar fornecedores
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para editar os fornecedores é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarFornecedor, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            // Não deixar incluir um CNPJ repetido
            FornecedorVo fornecedorVo = null;
            if (!VerificarFornecedorExistente(requisicaoDto.EntidadeDto, ref fornecedorVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao validar o CNPJ: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarFornecedor, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (fornecedorVo != null && fornecedorVo.Excluido == true)
            {
                if (!ExcluirDefinitivoBd(fornecedorVo.Id, ref mensagemErro))
                {
                    mensagemErro = $"Houve um erro ao deletar o fornecedor duplicado.";
                    return false;
                }

                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao excluir o fornecedor duplicado: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarFornecedor, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }
            else if (fornecedorVo != null && fornecedorVo.Excluido == false)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Esse cadastro (fornecedor) já existe, não é possível incluir cadastros duplicados";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarFornecedor, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out fornecedorVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar o fornecedor: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarFornecedor, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref fornecedorVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter o fornecedor para Vo: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarFornecedor, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!EditarBd(fornecedorVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados do fornecedor: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarFornecedor, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao salvar os novos dados: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarFornecedor, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui um fornecedor do banco de dados a partir do ID
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para excluir os fornecedores é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirFornecedor, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;
                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirFornecedor, requisicaoDto.Id, mensagemErro);
                    return false;
                }
            }

            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirFornecedor, requisicaoDto.Id, "Fornecedor excluído.");
            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém um fornecedor pelo ID
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<FornecedorDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            FornecedorVo fornecedorVo;

            if (!ObterPorIdBd(requisicaoDto.Id, out fornecedorVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o fornecedor: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterFornecedor, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            FornecedorDto fornecedorDto = new FornecedorDto();
            if (!ConverterVoParaDto(fornecedorVo, ref fornecedorDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o fornecedor: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterFornecedor, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (fornecedorDto.Endereco.Id != Guid.Empty)
            {
                RequisicaoObterDto requisicaoObterCep = new RequisicaoObterDto()
                {
                    Id = fornecedorDto.Endereco.Id,
                    IdUsuario = requisicaoDto.IdUsuario,
                    Identificacao = requisicaoDto.Identificacao
                };

                RetornoObterDto<CepDto> retornoCepDto = new RetornoObterDto<CepDto>();
                CepBll cepBll = new CepBll(true);

                if (!cepBll.Obter(requisicaoObterCep, ref retornoCepDto))
                {
                    retornoDto.Mensagem = "Erro ao obter o endereço: " + mensagemErro;
                    retornoDto.Retorno = false;
                    return false;
                }

                fornecedorDto.Endereco = (retornoCepDto.Entidade == null ? new CepDto() : retornoCepDto.Entidade);
            }

            retornoDto.Entidade = fornecedorDto;
            retornoDto.Retorno = true;
            retornoDto.Mensagem = "Ok";
            return true;
        }

        /// <summary>
        /// Obtém uma lista de fornecedores com filtros aplicados, podendo ser paginada
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<FornecedorDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            IQueryable<FornecedorVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os fornecedores: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaFornecedor, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "NOMEFANTASIA":
                        query = query.Where(p => p.NomeFantasia.Contains(filtro.Value));
                        break;

                    case "RAZAOSOCIAL":
                        query = query.Where(p => p.RazaoSocial.Contains(filtro.Value));
                        break;

                    case "CNPJ":
                        string cnpj = filtro.Value.Replace(".", "").Replace("-", "").Replace("/", "");
                        query = query.Where(p => p.Cnpj.Contains(cnpj));
                        break;

                    case "TELEFONE":
                        string telefone = filtro.Value.Replace("(", "").Replace(")", "").Replace("-", "");
                        query = query.Where(p => p.Telefone.Contains(telefone));
                        break;

                    case "INATIVO":

                        bool filtroInativo;
                        if (!bool.TryParse(filtro.Value, out filtroInativo))
                        {
                            retornoDto.Mensagem = $"Falha ao converter o filtro de 'inativo'.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.Inativo == filtroInativo);
                        break;

                    case "NOMEFANTASIACNPJ":
                        query = query.Where(p => p.NomeFantasia.Contains(filtro.Value) || p.Cnpj.Contains(filtro.Value));
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaFornecedor, Guid.Empty, retornoDto.Mensagem);
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "NOMEFANTASIA":
                    query = query.OrderBy(p => p.NomeFantasia);
                    break;

                case "RAZAOSOCIAL":
                    query = query.OrderBy(p => p.RazaoSocial);
                    break;

                default:
                    query = query.OrderBy(p => p.NomeFantasia);
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

            List<FornecedorVo> listaVo = query.ToList();
            foreach (var fornecedor in listaVo)
            {
                FornecedorDto fornecedorDto = new FornecedorDto();
                if (!ConverterVoParaDto(fornecedor, ref fornecedorDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaFornecedor, fornecedor.Id, retornoDto.Mensagem);
                    return false;
                }

                retornoDto.ListaEntidades.Add(fornecedorDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte um fornecedor Dto para um fornecedor Vo
        /// </summary>
        /// <param name="fornecedorDto"></param>
        /// <param name="fornecedorVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(FornecedorDto fornecedorDto, ref FornecedorVo fornecedorVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(fornecedorDto, ref fornecedorVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                Guid? idNulo = null;
                fornecedorVo.IdCep = fornecedorDto.Endereco.Id != Guid.Empty ? fornecedorDto.Endereco.Id : idNulo;
                fornecedorVo.Cnpj = string.IsNullOrWhiteSpace(fornecedorDto.Cnpj) ? "" : fornecedorDto.Cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "");
                fornecedorVo.ComplementoEndereco = string.IsNullOrWhiteSpace(fornecedorDto.ComplementoEndereco) ? "" : fornecedorDto.ComplementoEndereco.Trim();
                fornecedorVo.NomeFantasia = string.IsNullOrWhiteSpace(fornecedorDto.NomeFantasia) ? "" : fornecedorDto.NomeFantasia.Trim();
                fornecedorVo.NumeroEndereco = string.IsNullOrWhiteSpace(fornecedorDto.NumeroEndereco) ? "" : fornecedorDto.NumeroEndereco.Trim();
                fornecedorVo.Obs = string.IsNullOrWhiteSpace(fornecedorDto.Obs) ? "" : fornecedorDto.Obs.Trim();
                fornecedorVo.RazaoSocial = string.IsNullOrWhiteSpace(fornecedorDto.RazaoSocial) ? "" : fornecedorDto.RazaoSocial.Trim();
                fornecedorVo.Telefone = string.IsNullOrWhiteSpace(fornecedorDto.Telefone) ? "" : fornecedorDto.Telefone.Trim().Replace("(", "").Replace(")", "").Replace("-", "");

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o fornecedor para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um fornecedor Dto para um fornecedor Vo
        /// </summary>
        /// <param name="fornecedorVo"></param>
        /// <param name="fornecedorDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(FornecedorVo fornecedorVo, ref FornecedorDto fornecedorDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(fornecedorVo, ref fornecedorDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                fornecedorDto.Endereco.Id = fornecedorVo.IdCep == null ? Guid.Empty : fornecedorVo.IdCep.Value;
                fornecedorDto.Cnpj = string.IsNullOrWhiteSpace(fornecedorVo.Cnpj) ? "" : fornecedorVo.Cnpj.Trim();
                fornecedorDto.ComplementoEndereco = string.IsNullOrWhiteSpace(fornecedorVo.ComplementoEndereco) ? "" : fornecedorVo.ComplementoEndereco.Trim();
                fornecedorDto.NomeFantasia = string.IsNullOrWhiteSpace(fornecedorVo.NomeFantasia) ? "" : fornecedorVo.NomeFantasia.Trim();
                fornecedorDto.NumeroEndereco = string.IsNullOrWhiteSpace(fornecedorVo.NumeroEndereco) ? "" : fornecedorVo.NumeroEndereco.Trim();
                fornecedorDto.Obs = string.IsNullOrWhiteSpace(fornecedorVo.Obs) ? "" : fornecedorVo.Obs.Trim();
                fornecedorDto.RazaoSocial = string.IsNullOrWhiteSpace(fornecedorVo.RazaoSocial) ? "" : fornecedorVo.RazaoSocial.Trim();
                fornecedorDto.Telefone = string.IsNullOrWhiteSpace(fornecedorVo.Telefone) ? "" : fornecedorVo.Telefone.Trim();

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o fornecedor para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Valida se o CNPJ já existe
        /// </summary>
        /// <param name="fornecedorDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool ValidarCnpj(FornecedorDto fornecedorDto, ref string mensagemErro)
        {
            if (fornecedorDto == null)
            {
                mensagemErro = "É necessário informar o fornecedor para validar o CNPJ";
                return false;
            }

            if (string.IsNullOrWhiteSpace(fornecedorDto.Cnpj))
            {
                return true;
            }

            IQueryable<FornecedorVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                mensagemErro = $"Houve um problema ao listar os fornecedores: {mensagemErro}";
                return false;
            }

            fornecedorDto.Cnpj = fornecedorDto.Cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            query = query.Where(p => p.Cnpj == fornecedorDto.Cnpj.Trim() && p.Id != fornecedorDto.Id);

            if (query.Count() > 0)
            {
                mensagemErro = $"Já existe um fornecedor com este CNPJ. Para evitar duplicidade de cadastros, " +
                    $"não é possível incluir/editar outro fornecedor com este CNPJ.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida se o CPF já existe
        /// </summary>
        /// <param name="fornecedorDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool VerificarFornecedorExistente(FornecedorDto fornecedorDto, ref FornecedorVo fornecedorVo, ref string mensagemErro)
        {
            if (fornecedorDto == null)
            {
                mensagemErro = "É necessário informar o fornecedor para validar o CNPJ";
                return false;
            }

            if (string.IsNullOrWhiteSpace(fornecedorDto.Cnpj))
            {
                return true;
            }

            IQueryable<FornecedorVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro, true))
            {
                mensagemErro = $"Houve um problema ao listar os fornecedores: {mensagemErro}";
                return false;
            }

            fornecedorDto.Cnpj = fornecedorDto.Cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            query = query.Where(p => p.Cnpj == fornecedorDto.Cnpj.Trim() && p.Id != fornecedorDto.Id);
            fornecedorVo = query.FirstOrDefault();
            return true;

        }
    }
}
