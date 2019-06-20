using PizzaByteBll.Base;
using PizzaByteDal;
using PizzaByteDto.Base;
using PizzaByteDto.Entidades;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteVo;
using System;

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
                retornoDto.Mensagem = "Falha ao converter o funcionário para VO: " + mensagemErro;
                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(funcionarioVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o funcionário para VO: " + mensagemErro;
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
        /// Edita os dados de um funcionario
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
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao obter o funcionario para edição: " + mensagemErro;
                return false;
            }

            // Converte para VO a ser incluída no banco de dados
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref funcionarioVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o funcionario para VO: " + mensagemErro;
                return false;
            }

            if (!EditarBd(funcionarioVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao salvar os novos dados do funcionario: " + mensagemErro;
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

            FuncionarioVo funcionarioVo;
            string mensagemErro = "";

            if (!ObterPorIdBd(requisicaoDto.Id, out funcionarioVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o funcionario: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.Mensagem = "Ok";
            if (funcionarioVo == null)
            {
                retornoDto.Mensagem = "Funcionario não encontrado";
            }

            FuncionarioDto funcionarioDto = new FuncionarioDto();
            if (!ConverterVoParaDto(funcionarioVo, ref funcionarioDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o funcionario: " + mensagemErro;
                retornoDto.Retorno = false;
                return false;
            }

            retornoDto.Entidade = funcionarioDto;
            retornoDto.Retorno = true;
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
                funcionarioVo.Tipo = funcionarioDto.Tipo;
                funcionarioVo.Nome = string.IsNullOrWhiteSpace(funcionarioDto.Nome) ? "" : funcionarioDto.Nome.Trim();
                funcionarioVo.Telefone = string.IsNullOrWhiteSpace(funcionarioDto.Telefone) ? "" : funcionarioDto.Telefone.Trim();

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
                funcionarioDto.Tipo = funcionarioVo.Tipo;
                funcionarioDto.Nome = string.IsNullOrWhiteSpace(funcionarioVo.Nome) ? "" : funcionarioVo.Nome.Trim();
                funcionarioDto.Telefone = string.IsNullOrWhiteSpace(funcionarioVo.Telefone) ? "" : funcionarioVo.Telefone.Trim();

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o funcionario para Vo: " + ex.Message;
                return false;
            }
        }
    }
}
