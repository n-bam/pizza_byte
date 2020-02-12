using PizzaByteBll.Base;
using PizzaByteDal;
using PizzaByteDto.Base;
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
    public class ClienteEnderecoBll : BaseBll<ClienteEnderecoVo, ClienteEnderecoDto>
    {
        private static LogBll logBll = new LogBll("ClienteEnderecoBll");
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public ClienteEnderecoBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public ClienteEnderecoBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui um endereço de cliente no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<ClienteEnderecoDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Se houver endereço preenchido e ele não existir ainda
            if (requisicaoDto.EntidadeDto.Endereco.Id == Guid.Empty
                && !string.IsNullOrWhiteSpace(requisicaoDto.EntidadeDto.Endereco.Cep))
            {
                CepBll cepBll = new CepBll(this.pizzaByteContexto, false);
                RequisicaoEntidadeDto<CepDto> requisicaoCepDto = new RequisicaoEntidadeDto<CepDto>()
                {
                    IdUsuario = requisicaoDto.IdUsuario,
                    Identificacao = requisicaoDto.Identificacao,
                    EntidadeDto = requisicaoDto.EntidadeDto.Endereco
                };

                requisicaoCepDto.EntidadeDto.Id = Guid.NewGuid();

                // Incluir o endereço
                if (!cepBll.Incluir(requisicaoCepDto, ref retornoDto))
                {
                    // Logado na classe de CEP
                    return false;
                }
            }

            ClienteEnderecoVo clienteEnderecoVo = new ClienteEnderecoVo();
            string mensagemErro = "";

            // Converte para VO a ser incluída no banco de dados
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref clienteEnderecoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o endereço do cliente para VO: " + mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirClienteEndereco, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(clienteEnderecoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao incluir o endereço do cliente no banco de dados: " + mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirClienteEndereco, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao salvar o endereço do cliente: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirClienteEndereco, Guid.Empty, mensagemErro);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Edita um endereço de cliente
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<ClienteEnderecoDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            ClienteEnderecoVo clienteEnderecoVo = new ClienteEnderecoVo();
            string mensagemErro = "";

            // Obtém o endereço
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out clienteEnderecoVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar o endereço do cliente: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarClienteEndereco, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            // Se houver endereço preenchido e ele não existir ainda
            if (requisicaoDto.EntidadeDto.Endereco.Id == Guid.Empty
                && !string.IsNullOrWhiteSpace(requisicaoDto.EntidadeDto.Endereco.Cep))
            {
                requisicaoDto.EntidadeDto.Endereco.Id = Guid.NewGuid();

                CepBll cepBll = new CepBll(this.pizzaByteContexto, false);
                RequisicaoEntidadeDto<CepDto> requisicaoCepDto = new RequisicaoEntidadeDto<CepDto>()
                {
                    IdUsuario = requisicaoDto.IdUsuario,
                    Identificacao = requisicaoDto.Identificacao,
                    EntidadeDto = requisicaoDto.EntidadeDto.Endereco
                };

                // Incluir o endereço
                if (!cepBll.Incluir(requisicaoCepDto, ref retornoDto))
                {
                    // Logado na BLL de CEP
                    return false;
                }
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref clienteEnderecoVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter o endereço do cliente para Vo: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarClienteEndereco, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!EditarBd(clienteEnderecoVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados do clienteEndereco: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarClienteEndereco, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao salvar os novos dados: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarClienteEndereco, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui um endereço de cliente do banco de dados a partir do ID
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para excluir endereços de clientes é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirClienteEndereco, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;
                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirClienteEndereco, requisicaoDto.Id, mensagemErro);
                    return false;
                }
            }

            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirClienteEndereco, requisicaoDto.Id, "Endereço excluído.");
            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém um endereço de cliente pelo ID
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<ClienteEnderecoDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = ""; ClienteEnderecoVo clienteEnderecoVo;
            if (!ObterPorIdBd(requisicaoDto.Id, out clienteEnderecoVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o endereço do cliente: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterClienteEndereco, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            ClienteEnderecoDto clienteEnderecoDto = new ClienteEnderecoDto();
            if (!ConverterVoParaDto(clienteEnderecoVo, ref clienteEnderecoDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o endereço do cliente: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterClienteEndereco, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (clienteEnderecoDto.IdCep != Guid.Empty)
            {
                RequisicaoObterDto requisicaoObterCep = new RequisicaoObterDto()
                {
                    Id = clienteEnderecoDto.IdCep,
                    IdUsuario = requisicaoDto.IdUsuario,
                    Identificacao = requisicaoDto.Identificacao
                };

                RetornoObterDto<CepDto> retornoCepDto = new RetornoObterDto<CepDto>();
                CepBll cepBll = new CepBll(true);

                if (!cepBll.Obter(requisicaoObterCep, ref retornoCepDto))
                {
                    retornoDto.Mensagem = $"Erro ao obter o endereço: {retornoCepDto.Mensagem}";
                    retornoDto.Retorno = false;
                    return false;
                }

                clienteEnderecoDto.Endereco = (retornoCepDto.Entidade == null ? new CepDto() : retornoCepDto.Entidade);
            }

            retornoDto.Entidade = clienteEnderecoDto;
            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtém uma lista de clienteEnderecoes com filtros aplicados, podendo ser paginada
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<ClienteEnderecoDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Obter a query primária
            string mensagemErro = ""; IQueryable<ClienteEnderecoVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os endereços de clientes: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaClienteEndereco, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "IDCLIENTE":
                        Guid id;
                        if (!Guid.TryParse(filtro.Value, out id))
                        {
                            retornoDto.Mensagem = $"Falha ao converter o filtro de 'IdCliente'.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.IdCliente == id);
                        break;

                    case "IDCEP":
                        Guid idCep;
                        if (!Guid.TryParse(filtro.Value, out idCep))
                        {
                            retornoDto.Mensagem = $"Falha ao converter o filtro de 'IdCep'.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.IdCep == idCep);
                        break;

                    case "INATIVO":

                        bool filtroInativo;
                        if (!bool.TryParse(filtro.Value, out filtroInativo))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'inativo'.";
                            retornoDto.Retorno = false;
                            return false;
                        }

                        query = query.Where(p => p.Inativo == filtroInativo);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaClienteEndereco, Guid.Empty, retornoDto.Mensagem);
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "IDCLIENTE":
                    query = query.OrderBy(p => p.IdCliente).ThenBy(p => p.IdCep);
                    break;

                case "IDCEP":
                    query = query.OrderBy(p => p.IdCep).ThenBy(p => p.IdCliente);
                    break;

                default:
                    query = query.OrderBy(p => p.IdCliente).ThenBy(p => p.IdCep);
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

            List<ClienteEnderecoVo> listaVo = query.ToList();
            List<CepVo> listaCepsVo = new List<CepVo>();

            CepBll cepBll = new CepBll(false);
            if (!cepBll.ObterListaEnderecosPorId(listaVo.Select(p => p.IdCep).ToList(), ref listaCepsVo, ref mensagemErro))
            {
                retornoDto.Mensagem = mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaClienteEndereco, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            foreach (var clienteEndereco in listaVo)
            {
                CepVo cepDto = listaCepsVo.Where(p => p.Id == clienteEndereco.IdCep).FirstOrDefault();
                ClienteEnderecoDto clienteEnderecoDto = new ClienteEnderecoDto();
                if (!ConverterVoParaDto(clienteEndereco, ref clienteEnderecoDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaClienteEndereco, clienteEndereco.Id, retornoDto.Mensagem);
                    return false;
                }

                //Preencher dados do endereço
                clienteEnderecoDto.Endereco = new CepDto()
                {
                    Bairro = cepDto == null ? "" : cepDto.Bairro,
                    Cep = cepDto == null ? "" : cepDto.Cep,
                    Cidade = cepDto == null ? "" : cepDto.Cidade,
                    Logradouro = cepDto == null ? "" : cepDto.Logradouro,
                    Id = cepDto == null ? Guid.Empty : cepDto.Id
                };

                retornoDto.ListaEntidades.Add(clienteEnderecoDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte um endereço de cliente Dto para um endereço de cliente Vo
        /// </summary>
        /// <param name="clienteEnderecoDto"></param>
        /// <param name="clienteEnderecoVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(ClienteEnderecoDto clienteEnderecoDto, ref ClienteEnderecoVo clienteEnderecoVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(clienteEnderecoDto, ref clienteEnderecoVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                clienteEnderecoVo.IdCep = clienteEnderecoDto.Endereco.Id;
                clienteEnderecoVo.NumeroEndereco = string.IsNullOrWhiteSpace(clienteEnderecoDto.NumeroEndereco) ? "" : clienteEnderecoDto.NumeroEndereco.Trim();
                clienteEnderecoVo.ComplementoEndereco = string.IsNullOrWhiteSpace(clienteEnderecoDto.ComplementoEndereco) ? "" : clienteEnderecoDto.ComplementoEndereco.Trim();
                clienteEnderecoVo.IdCliente = clienteEnderecoDto.IdCliente;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o endereço do cliente para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um endereço de cliente Dto para um endereço de cliente Vo
        /// </summary>
        /// <param name="clienteEnderecoVo"></param>
        /// <param name="clienteEnderecoDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(ClienteEnderecoVo clienteEnderecoVo, ref ClienteEnderecoDto clienteEnderecoDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(clienteEnderecoVo, ref clienteEnderecoDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                clienteEnderecoDto.IdCep = clienteEnderecoVo.IdCep;
                clienteEnderecoDto.NumeroEndereco = string.IsNullOrWhiteSpace(clienteEnderecoVo.NumeroEndereco) ? "" : clienteEnderecoVo.NumeroEndereco.Trim();
                clienteEnderecoDto.ComplementoEndereco = string.IsNullOrWhiteSpace(clienteEnderecoVo.ComplementoEndereco) ? "" : clienteEnderecoVo.ComplementoEndereco.Trim();
                clienteEnderecoDto.IdCliente = clienteEnderecoVo.IdCliente;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o endereço do cliente para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Inclui ou atualiza os dados de um endereço de cliente
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="clienteEnderecoDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        internal bool IncluirEditar(BaseRequisicaoDto requisicaoDto, ClienteEnderecoDto clienteEnderecoDto, ref string mensagemErro)
        {
            if (clienteEnderecoDto == null)
            {
                mensagemErro = "Preencha o endereço que deseja incluir/editar.";
                return false;
            }

            ClienteEnderecoVo clienteEnderecoVo;
            if (!ObterPorIdBd(clienteEnderecoDto.Id, out clienteEnderecoVo, ref mensagemErro) && mensagemErro != "Cadastro não encontrado")
            {
                mensagemErro = "Problemas para encontrar o endereço do cliente: " + mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirEditarClienteEndereco, clienteEnderecoDto.Id, mensagemErro);
                return false;
            }

            // Incluir se não existir
            if (clienteEnderecoVo == null)
            {
                RequisicaoEntidadeDto<ClienteEnderecoDto> requisicaoIncluirDto = new RequisicaoEntidadeDto<ClienteEnderecoDto>()
                {
                    EntidadeDto = clienteEnderecoDto,
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
                if ((string.IsNullOrWhiteSpace(clienteEnderecoDto.ComplementoEndereco) ? "" : clienteEnderecoDto.ComplementoEndereco.Trim()) != (string.IsNullOrWhiteSpace(clienteEnderecoVo.ComplementoEndereco) ? "" : clienteEnderecoVo.ComplementoEndereco.Trim())
                    || (string.IsNullOrWhiteSpace(clienteEnderecoDto.NumeroEndereco) ? "" : clienteEnderecoDto.NumeroEndereco.Trim()) != (string.IsNullOrWhiteSpace(clienteEnderecoVo.NumeroEndereco) ? "" : clienteEnderecoVo.NumeroEndereco.Trim())
                    || clienteEnderecoDto.IdCep != clienteEnderecoVo.IdCep)
                {
                    RequisicaoEntidadeDto<ClienteEnderecoDto> requisicaoEditarDto = new RequisicaoEntidadeDto<ClienteEnderecoDto>()
                    {
                        EntidadeDto = clienteEnderecoDto,
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
        /// Exclui os endereços de um cliente pelo seu ID
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public bool ExcluirPorIdCliente(RequisicaoObterDto requisicaoDto, ref RetornoDto retornoDto)
        {
            if (requisicaoDto.Id == Guid.Empty)
            {
                retornoDto.Mensagem = "Informe o ID do cliente para excluir seus endereços.";
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirEnderecosPorIdCliente, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            string mensagemErro = "";
            if (!UtilitarioBll.ValidarIdentificacao(requisicaoDto.Identificacao, requisicaoDto.IdUsuario, ref mensagemErro))
            {
                retornoDto.Mensagem = mensagemErro;
                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirEnderecosPorIdCliente, requisicaoDto.Id, mensagemErro);
                return false;
            }

            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para excluir endereços de clientes é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirEnderecosPorIdCliente, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            // Obter a query primária
            IQueryable<ClienteEnderecoVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os endereços de clientes: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirEnderecosPorIdCliente, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            query = query.Where(p => p.IdCliente == requisicaoDto.Id);
            List<ClienteEnderecoVo> enderecosEncontrados = query.ToList();

            // Excluir os endereços encontrados
            foreach (var enderecoVo in enderecosEncontrados)
            {
                if (!ExcluirBd(enderecoVo.Id, ref mensagemErro))
                {
                    retornoDto.Mensagem = $"Houve um problema ao excluir o endereço do cliente: {mensagemErro}";
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirEnderecosPorIdCliente, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }

                if (salvar)
                {
                    // Salva as alterações
                    if (!pizzaByteContexto.Salvar(ref mensagemErro))
                    {
                        retornoDto.Retorno = false;
                        retornoDto.Mensagem = "Houve um problema ao excluir o endereço do cliente: " + mensagemErro;
                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirEnderecosPorIdCliente, requisicaoDto.Id, mensagemErro);
                        return false;
                    }
                }

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirEnderecosPorIdCliente, requisicaoDto.Id, "endereço excluído.");
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtem uma lista de endereços de cliente pelos IDs
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        internal bool ObterListaPorId(RequisicaoListaGuidsDto requisicaoDto, ref RetornoObterListaDto<ClienteEnderecoDto> retornoDto)
        {
            // Obter a query primária
            string mensagemErro = ""; IQueryable<ClienteEnderecoVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os endereços de clientes: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaEnderecosClientePorId, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            List<ClienteEnderecoVo> listaVo = query.Where(p => requisicaoDto.ListaGuids.Contains(p.Id)).ToList();
            List<CepVo> listaCepsVo = new List<CepVo>();

            // Obter CEPs
            CepBll cepBll = new CepBll(false);
            if (!cepBll.ObterListaEnderecosPorId(listaVo.Select(p => p.IdCep).ToList(), ref listaCepsVo, ref mensagemErro))
            {
                retornoDto.Mensagem = mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaEnderecosClientePorId, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Converter
            foreach (var clienteEndereco in listaVo)
            {
                CepVo cepDto = listaCepsVo.Where(p => p.Id == clienteEndereco.IdCep).FirstOrDefault();
                ClienteEnderecoDto clienteEnderecoDto = new ClienteEnderecoDto();
                if (!ConverterVoParaDto(clienteEndereco, ref clienteEnderecoDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaEnderecosClientePorId, clienteEndereco.Id, retornoDto.Mensagem);
                    return false;
                }

                //Preencher dados do endereço
                clienteEnderecoDto.Endereco = new CepDto()
                {
                    Bairro = cepDto == null ? "" : cepDto.Bairro,
                    Cep = cepDto == null ? "" : cepDto.Cep,
                    Cidade = cepDto == null ? "" : cepDto.Cidade,
                    Logradouro = cepDto == null ? "" : cepDto.Logradouro,
                    Id = cepDto == null ? Guid.Empty : cepDto.Id
                };

                retornoDto.ListaEntidades.Add(clienteEnderecoDto);
            }

            return true;
        }
    }
}
