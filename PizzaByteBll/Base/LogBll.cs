using PizzaByteDto.Base;
using PizzaByteDto.ClassesBase;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteVo.Base;
using System;
using System.Runtime.CompilerServices;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteBll.Base
{
    public class LogBll : BaseBll<LogVo, LogDto>
    {
        private static LogBll logBll = new LogBll("LogBll");
        private string classeOrigem = "";

        /// <summary>
        /// Começa a classe com a identificação da classe de origem
        /// </summary>
        /// <param name="bllOrigem"></param>
        public LogBll(string bllOrigem) : base(logBll)
        {
            this.classeOrigem = bllOrigem;
        }

        ///// <summary>
        ///// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        ///// </summary>
        ///// <param name="salvarAlteracoes"></param>
        //public LogBll(bool salvarAlteracoes) : base()
        //{
        //    salvar = salvarAlteracoes;
        //}

        ///// <summary>
        ///// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        ///// </summary>
        ///// <param name="contexto"></param>
        ///// <param name="salvarAlteracoes"></param>
        //public LogBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(contexto)
        //{
        //    salvar = salvarAlteracoes;
        //}

        ///// <summary>
        ///// Inclui um log no banco de dados
        ///// </summary>
        ///// <param name="requisicaoDto"></param>
        ///// <param name="retornoDto"></param>
        ///// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<LogDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                // Logado dentro do método base.Incluir
                return false;
            }

            LogVo logVo = new LogVo();
            string mensagemErro = "";

            // Converte para VO a ser incluída no banco de dados
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref logVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o log para VO: " + mensagemErro;
                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(logVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o log para VO: " + mensagemErro;
                return false;
            }

            // Salva as alterações
            if (!pizzaByteContexto.Salvar(ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = mensagemErro;
                return false;
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Método prático para registrar um log no banco de dados
        /// </summary>
        /// <param name="requisicaoBaseDto"></param>
        /// <param name="recurso"></param>
        /// <param name="idEntidade"></param>
        /// <param name="retornoDto"></param>
        /// <param name="linha"></param>
        /// <param name="metodo"></param>
        internal void ResgistrarLog(BaseRequisicaoDto requisicaoBaseDto, LogRecursos recurso, Guid idEntidade, string mensagem, [CallerLineNumber]int linha = 0, [CallerMemberName]string metodo = "")
        {
            RequisicaoEntidadeDto<LogDto> requisicaoDto = new RequisicaoEntidadeDto<LogDto>()
            {
                IdUsuario = requisicaoBaseDto.IdUsuario,
                Identificacao = requisicaoBaseDto.Identificacao,
                EntidadeDto = new LogDto()
            };

            requisicaoDto.EntidadeDto.IdEntidade = idEntidade;
            requisicaoDto.EntidadeDto.IdUsuario = requisicaoBaseDto.IdUsuario;
            requisicaoDto.EntidadeDto.Recurso = recurso;
            requisicaoDto.EntidadeDto.Id = Guid.NewGuid();
            requisicaoDto.EntidadeDto.Inativo = false;
            requisicaoDto.EntidadeDto.Mensagem = $"Classe: {classeOrigem}. \n " +
                                                 $"Método: {metodo} \n" +
                                                 $"Linha: {linha} \n" +
                                                 $"Mensagem: {mensagem}";

            RetornoDto retornoIncluirDto = new RetornoDto();
            if (!Incluir(requisicaoDto, ref retornoIncluirDto))
            {
                //TODO: Enviar email para avisar que não está logando
            }
        }

        /// <summary>
        /// Registra um LOG se precisar fazer o login
        /// </summary>
        /// <param name="logVo"></param>
        /// <param name="retornoDto"></param>
        internal void RegistrarLogVo(LogVo logVo, [CallerLineNumber]int linha = 0, [CallerMemberName]string metodo = "")
        {
            string mensagemErro = "";

            logVo.Mensagem = $"Classe: {classeOrigem}. \n " +
                             $"Método: {metodo} \n" +
                             $"Linha: {linha} \n" +
                             $"Mensagem: {logVo.Mensagem}";

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(logVo, ref mensagemErro))
            {
                return;
            }

            // Salva as alterações
            pizzaByteContexto.Salvar(ref mensagemErro);
        }

        /// <summary>
        /// Converte um log Vo para um log Dto
        /// </summary>
        /// <param name="logDto"></param>
        /// <param name="logVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(LogDto logDto, ref LogVo logVo, ref string mensagemErro)
        {
            try
            {
                logVo.Recurso = logDto.Recurso;
                logVo.IdUsuario = logDto.IdUsuario;
                logVo.IdEntidade = logDto.IdEntidade;
                logVo.Mensagem = string.IsNullOrWhiteSpace(logDto.Mensagem) ? "" : logDto.Mensagem.Trim();
                logVo.Id = logDto.Id;
                logVo.Inativo = logDto.Inativo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o log para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um log Dto para um log Vo
        /// </summary>
        /// <param name="logVo"></param>
        /// <param name="logDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(LogVo logVo, ref LogDto logDto, ref string mensagemErro)
        {
            try
            {
                logDto.Recurso = logVo.Recurso;
                logDto.IdUsuario = logVo.IdUsuario;
                logDto.IdEntidade = logVo.IdEntidade;
                logDto.Id = logVo.Id;
                logDto.Inativo = logVo.Inativo;
                logDto.DataAlteracao = logVo.DataAlteracao;
                logDto.DataInclusao = logVo.DataInclusao;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter o log para Vo: " + ex.Message;
                return false;
            }
        }

    }
}
