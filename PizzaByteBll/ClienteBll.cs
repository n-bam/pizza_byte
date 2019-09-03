using PizzaByteBll.Base;
using PizzaByteDal;
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
    public class ClienteBll : BaseBll<ClienteVo, ClienteDto>
    {
        private static LogBll logBll = new LogBll("ClienteBll");
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public ClienteBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public ClienteBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui um cliente no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<ClienteDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Converte para VO a ser incluída no banco de dados
            string mensagemErro = "";
            ClienteVo clienteVo = new ClienteVo();
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref clienteVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o cliente para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirCliente, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Verifica se o cliente já existe
            ClienteVo clienteExistente = null;
            if (!VerificarClienteExistente(requisicaoDto.EntidadeDto, ref clienteExistente, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            // Se estiver excluído, restaurar o cadastro antigo
            if (clienteExistente != null && clienteExistente.Excluido == true)
            {
                clienteExistente.Cpf = clienteVo.Cpf;
                clienteExistente.Nome = clienteVo.Nome;
                clienteExistente.Telefone = clienteVo.Telefone;
                clienteExistente.Excluido = false;

                if (!EditarBd(clienteExistente, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao salvar os dados do produto: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirCliente, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }
            // Se não estiver excluído, não permitir incluir duplicado
            else if (clienteExistente != null && clienteExistente.Excluido == false)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Esse cadastro já existe, não é possível incluir cadastros duplicados.";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirCliente, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }
            else
            {
                // Prepara a inclusão no banco de dados
                if (!IncluirBd(clienteVo, ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao converter o cliente para VO: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirCliente, Guid.Empty, retornoDto.Mensagem);
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

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirCliente, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui um cliente do banco de dados a partir do ID
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para excluir clientes é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirCliente, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirCliente, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém um cliente pelo ID 
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<ClienteDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            ClienteVo clienteVo;

            if (!ObterPorIdBd(requisicaoDto.Id, out clienteVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o cliente: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterCliente, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Mensagem = "Ok";
            if (clienteVo == null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Cliente não encontrado";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterCliente, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            ClienteDto clienteDto = new ClienteDto();
            if (!ConverterVoParaDto(clienteVo, ref clienteDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o cliente: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterCliente, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Entidade = clienteDto;
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte um cliente Dto para um cliente Vo
        /// </summary>
        /// <param name="clienteDto"></param>
        /// <param name="clienteVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(ClienteDto clienteDto, ref ClienteVo clienteVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(clienteDto, ref clienteVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                clienteVo.Nome = string.IsNullOrWhiteSpace(clienteDto.Nome) ? "" : clienteDto.Nome.Trim();
                clienteVo.Telefone = string.IsNullOrWhiteSpace(clienteDto.Telefone) ? "" : clienteDto.Telefone.Trim().Replace("(", "").Replace(")", "").Replace("-", "");
                clienteVo.Cpf = string.IsNullOrWhiteSpace(clienteDto.Cpf) ? "" : clienteDto.Cpf.Trim().Replace(".", "").Replace("-", "");

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o cliente para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um cliente Dto para um cliente Vo
        /// </summary>
        /// <param name="clienteVo"></param>
        /// <param name="clienteDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(ClienteVo clienteVo, ref ClienteDto clienteDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(clienteVo, ref clienteDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                clienteDto.Nome = string.IsNullOrWhiteSpace(clienteVo.Nome) ? "" : clienteVo.Nome.Trim();
                clienteDto.Telefone = string.IsNullOrWhiteSpace(clienteVo.Telefone) ? "" : clienteVo.Telefone.Trim().Replace("(", "").Replace(")", "").Replace("-", "");
                clienteDto.Cpf = string.IsNullOrWhiteSpace(clienteVo.Cpf) ? "" : clienteVo.Cpf.Trim().Replace(".", "").Replace("-", "");

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o cliente para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Obtém uma lista de clientes com filtros aplicados, podendo ser paginada
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<ClienteDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            IQueryable<ClienteVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar os clientes: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaCliente, Guid.Empty, retornoDto.Mensagem);
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

                    case "CPF":
                        string cpf = string.IsNullOrWhiteSpace(filtro.Value) ? "" : filtro.Value.Trim().Replace(".", "").Replace("-", "");
                        query = query.Where(p => p.Cpf.Contains(cpf));
                        break;

                    case "TELEFONE":
                        string telefone = string.IsNullOrWhiteSpace(filtro.Value) ? "" : filtro.Value.Trim().Replace("(", "").Replace("-", "").Replace(")", "");
                        query = query.Where(p => p.Telefone.Contains(telefone));
                        break;

                    case "INATIVO":

                        bool filtroInativo;
                        if (!bool.TryParse(filtro.Value, out filtroInativo))
                        {
                            retornoDto.Mensagem = $"Fala ao converter o filtro de 'inativo'.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaCliente, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Inativo == filtroInativo);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaCliente, Guid.Empty, retornoDto.Mensagem);
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "NOME":
                    query = query.OrderBy(p => p.Nome).ThenBy(p => p.Telefone).ThenBy(p => p.Cpf);
                    break;

                case "CPF":
                    query = query.OrderBy(p => p.Cpf).ThenBy(p => p.Nome).ThenBy(p => p.Telefone);
                    break;

                case "TELEFONE":
                    query = query.OrderByDescending(p => p.Telefone).ThenBy(p => p.Nome).ThenBy(p => p.Cpf);
                    break;

                default:
                    query = query.OrderBy(p => p.Nome).ThenBy(p => p.Telefone).ThenBy(p => p.Cpf);
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

            List<ClienteVo> listaVo = query.ToList();
            foreach (var cliente in listaVo)
            {
                ClienteDto clienteDto = new ClienteDto();
                if (!ConverterVoParaDto(cliente, ref clienteDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaCliente, cliente.Id, retornoDto.Mensagem);
                    return false;
                }

                retornoDto.ListaEntidades.Add(clienteDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Edita um cliente
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Editar(RequisicaoEntidadeDto<ClienteDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            if (!base.Editar(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            // Apenas usuários ADM podem editar clientees
            string mensagemErro = "";
            if (!UtilitarioBll.ValidarUsuarioAdm(requisicaoDto.Identificacao, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Este usuário não é administrador. Para editar os clientees é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCliente, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            // Não deixar incluir um Cpf repetido
            ClienteVo clienteVo = new ClienteVo();
            if (!VerificarClienteExistente(requisicaoDto.EntidadeDto, ref clienteVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao validar o Cpf: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCliente, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (clienteVo != null && clienteVo.Excluido == true)
            {
                if (!ExcluirDefinitivoBd(clienteVo.Id, ref mensagemErro))
                {
                    mensagemErro = $"Houve um erro ao deletar o cliente duplicado.";
                    return false;
                }

                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Falha ao excluir o cliente duplicado: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCliente, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }
            else if (clienteVo != null && clienteVo.Excluido == false)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Esse cadastro já existe, não é possível incluir cadastros duplicados";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCliente, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out clienteVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas para encontrar o cliente: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCliente, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref clienteVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Problemas ao converter o cliente para Vo: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCliente, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (!EditarBd(clienteVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao editar os novos dados do cliente: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCliente, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = "Erro ao salvar os novos dados: " + mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.EditarCliente, requisicaoDto.EntidadeDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Valida se o CPF já existe
        /// </summary>
        /// <param name="clienteDto"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private bool VerificarClienteExistente(ClienteDto clienteDto, ref ClienteVo clienteVo, ref string mensagemErro)
        {
            if (clienteDto == null)
            {
                mensagemErro = "É necessário informar o cliente para validar o Cpf";
                return false;
            }

            if (string.IsNullOrWhiteSpace(clienteDto.Cpf))
            {
                return true;
            }

            IQueryable<ClienteVo> query;
            if (!this.ObterQueryBd(out query, ref mensagemErro, true))
            {
                mensagemErro = $"Houve um problema ao listar os clientes: {mensagemErro}";
                return false;
            }

            clienteDto.Cpf = clienteDto.Cpf.Replace(".", "").Replace("/", "").Replace("-", "");
            query = query.Where(p => p.Cpf == clienteDto.Cpf.Trim() && p.Id != clienteDto.Id);
            clienteVo = query.FirstOrDefault();
            return true;

        }
    }
}