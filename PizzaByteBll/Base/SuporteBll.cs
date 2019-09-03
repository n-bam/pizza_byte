using PizzaByteDal;
using PizzaByteDto.Base;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteVo.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteBll.Base
{
    public class SuporteBll : BaseBll<SuporteVo, SuporteDto>
    {
        private static LogBll logBll = new LogBll("SuporteBll");
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public SuporteBll(bool salvarAlteracoes) : base(logBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public SuporteBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>
        /// Inclui um suporte no banco de dados
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Incluir(RequisicaoEntidadeDto<SuporteDto> requisicaoDto, ref RetornoDto retornoDto)
        {
            // Valida a requisição
            if (!base.Incluir(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            SuporteVo suporteVo = new SuporteVo();
            string mensagemErro = "";

            // Converte para VO a ser incluída no banco de dados
            if (!ConverterDtoParaVo(requisicaoDto.EntidadeDto, ref suporteVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o suporte para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirSuporte, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Prepara a inclusão no banco de dados
            if (!IncluirBd(suporteVo, ref mensagemErro))
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Falha ao converter o suporte para VO: " + mensagemErro;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirSuporte, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirSuporte, Guid.Empty, retornoDto.Mensagem);
                    return false;
                }
            }

            // Se for mensagem do usuário, enviar alerta por email
            if (suporteVo.Tipo == TipoMensagemSuporte.Usuario)
            {
                string corpoEmail = $"<p> Há uma nova mensagem de suporte!</p>" +
                                  $"<p> Mensagem incluída as {suporteVo.DataInclusao}:</p>" +
                                  $"<p><strong>{suporteVo.Mensagem}</strong></p><br/>" +
                                  "<p> Entre com a senha de suporte para responder à solicitação.</p>";

                if (!UtilitarioBll.EnviarEmail("jlmanfrinato@hotmail.com", "Nova mensagem de suporte", corpoEmail, ref mensagemErro))
                {
                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirSuporte, suporteVo.Id, $"Problemas para enviar a mensagem por email: {mensagemErro}");
                }

                if (!UtilitarioBll.EnviarEmail("huxxley@hotmail.com.br", "Nova mensagem de suporte", corpoEmail, ref mensagemErro))
                {
                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirSuporte, suporteVo.Id, $"Problemas para enviar a mensagem por email: {mensagemErro}");
                }

                if (!UtilitarioBll.EnviarEmail("driramosbenite@gmail.com", "Nova mensagem de suporte", corpoEmail, ref mensagemErro))
                {
                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirSuporte, suporteVo.Id, $"Problemas para enviar a mensagem por email: {mensagemErro}");
                }

                if (!UtilitarioBll.EnviarEmail("barbaracocatosantos@gmail.com", "Nova mensagem de suporte", corpoEmail, ref mensagemErro))
                {
                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirSuporte, suporteVo.Id, $"Problemas para enviar a mensagem por email: {mensagemErro}");
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Exclui um suporte do banco de dados a partir do ID
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
                retornoDto.Mensagem = "Este usuário não é administrador. Para excluir mensagens de suporte é necessário " +
                    $"logar com um usuário administrador. {mensagemErro}";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirSuporte, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            if (salvar)
            {
                // Salva as alterações
                if (!pizzaByteContexto.Salvar(ref mensagemErro))
                {
                    retornoDto.Retorno = false;
                    retornoDto.Mensagem = mensagemErro;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ExcluirSuporte, requisicaoDto.Id, retornoDto.Mensagem);
                    return false;
                }
            }

            retornoDto.Retorno = true;
            retornoDto.Mensagem = "OK";
            return true;
        }

        /// <summary>
        /// Obtém um suporte pelo ID 
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<SuporteDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            SuporteVo suporteVo;

            if (!ObterPorIdBd(requisicaoDto.Id, out suporteVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o suporte: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterSuporte, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Mensagem = "Ok";
            if (suporteVo == null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Suporte não encontrado";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterSuporte, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            SuporteDto suporteDto = new SuporteDto();
            if (!ConverterVoParaDto(suporteVo, ref suporteDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o suporte: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterSuporte, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Entidade = suporteDto;
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtém uma lista de suportes com filtros aplicados, podendo ser paginada
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<SuporteDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            IQueryable<SuporteVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as mensagens: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaSuporte, Guid.Empty, retornoDto.Mensagem);
                return false;
            }

            // Aplicar os filtros
            foreach (var filtro in requisicaoDto.ListaFiltros)
            {
                switch (filtro.Key)
                {
                    case "MENSAGEM":
                        query = query.Where(p => p.Mensagem.Contains(filtro.Value));
                        break;

                    case "TIPO":

                        int tipo;
                        if (!int.TryParse(filtro.Value, out tipo))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de tipo.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaSuporte, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        query = query.Where(p => p.Tipo == (TipoMensagemSuporte)tipo);
                        break;

                    case "DATAINCLUSAOMAIOR":
                        DateTime data;
                        if (!DateTime.TryParse(filtro.Value, out data))
                        {
                            retornoDto.Mensagem = $"Problema ao converter o filtro de data inclusão.";
                            retornoDto.Retorno = false;

                            logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaSuporte, Guid.Empty, retornoDto.Mensagem);
                            return false;
                        }

                        data = data.AddMilliseconds(999);
                        query = query.Where(p => p.DataInclusao > data);
                        break;

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaSuporte, Guid.Empty, retornoDto.Mensagem);
                        return false;
                }
            }

            requisicaoDto.CampoOrdem = string.IsNullOrWhiteSpace(requisicaoDto.CampoOrdem) ? "" : requisicaoDto.CampoOrdem.ToUpper().Trim();
            switch (requisicaoDto.CampoOrdem)
            {
                case "MENSAGEM":
                    query = query.OrderBy(p => p.Mensagem);
                    break;

                case "DATACRESCENTE":
                    query = query.OrderBy(p => p.DataInclusao);
                    break;

                case "DATADESCRESCENTE":
                    query = query.OrderByDescending(p => p.DataInclusao);
                    break;

                default:
                    query = query.OrderByDescending(p => p.DataInclusao);
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

            List<SuporteVo> listaVo = query.ToList();
            foreach (var suporte in listaVo)
            {
                SuporteDto suporteDto = new SuporteDto();
                if (!ConverterVoParaDto(suporte, ref suporteDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaSuporte, suporte.Id, retornoDto.Mensagem);
                    return false;
                }

                retornoDto.ListaEntidades.Add(suporteDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte um suporte Dto para um suporte Vo
        /// </summary>
        /// <param name="suporteDto"></param>
        /// <param name="suporteVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(SuporteDto suporteDto, ref SuporteVo suporteVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(suporteDto, ref suporteVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                suporteVo.Mensagem = string.IsNullOrWhiteSpace(suporteDto.Mensagem) ? "" : suporteDto.Mensagem.Trim();
                suporteVo.Tipo = suporteDto.Tipo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a mensagem para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um suporte Dto para um suporte Vo
        /// </summary>
        /// <param name="suporteVo"></param>
        /// <param name="suporteDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(SuporteVo suporteVo, ref SuporteDto suporteDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(suporteVo, ref suporteDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                suporteDto.Mensagem = string.IsNullOrWhiteSpace(suporteVo.Mensagem) ? "" : suporteVo.Mensagem.Trim();
                suporteDto.Tipo = suporteVo.Tipo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a mensagem para Vo: " + ex.Message;
                return false;
            }
        }
    }
}
