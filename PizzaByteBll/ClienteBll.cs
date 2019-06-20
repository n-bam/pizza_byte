using PizzaByteBll.Base;
using PizzaByteDal;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteVo;
using System;

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

            ClienteVo clienteVo = new ClienteVo();
            string mensagemErro = "";

            // Converte para VO a ser incluída no banco de dados
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref clienteVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o cliente para VO: " + mensagemErro;
                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(clienteVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o cliente para VO: " + mensagemErro;
                return false;
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
        /// Edita os dados de um cliente
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

            string mensagemErro = "";
            ClienteVo clienteVo = new ClienteVo();
            if (!ObterPorIdBd(requisicaoDto.EntidadeDto.Id, out clienteVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter o cliente para edição: " + mensagemErro;
                return false;
            }

            // Converte para VO a ser incluída no banco de dados
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref clienteVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o cliente para VO: " + mensagemErro;
                return false;
            }

            if (!EditarBd(clienteVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao salvar os novos dados do cliente: " + mensagemErro;
                return false;
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

            ClienteVo clienteVo;
            string mensagemErro = "";

            if (!ObterPorIdBd(requisicaoDto.Id, out clienteVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o cliente: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.Mensagem = "Ok";
            if (clienteVo == null)
            {
                retornoDto.Mensagem = "Cliente não encontrado";
            }

            ClienteDto clienteDto = new ClienteDto();
            if (!ConverterVoParaDto(clienteVo, ref clienteDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o cliente: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.Entidade = clienteDto;
            retornoDto.Retorno = true;
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

            if (salvar)
            {
                // Salva as alterações
                string mensagemErro = "";
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
                clienteVo.Cpf = string.IsNullOrWhiteSpace(clienteDto.Cpf) ? "" : clienteDto.Cpf.Trim();
                clienteVo.Nome = string.IsNullOrWhiteSpace(clienteDto.Nome) ? "" : clienteDto.Nome.Trim();
                clienteVo.Telefone = string.IsNullOrWhiteSpace(clienteDto.Telefone) ? "" : clienteDto.Telefone.Trim();

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
                clienteDto.Cpf = string.IsNullOrWhiteSpace(clienteVo.Cpf) ? "" : clienteVo.Cpf.Trim();
                clienteDto.Nome = string.IsNullOrWhiteSpace(clienteVo.Nome) ? "" : clienteVo.Nome.Trim();
                clienteDto.Telefone = string.IsNullOrWhiteSpace(clienteVo.Telefone) ? "" : clienteVo.Telefone.Trim();

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o cliente para Vo: " + ex.Message;
                return false;
            }
        }
    }
}
