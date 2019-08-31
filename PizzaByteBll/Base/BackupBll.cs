using PizzaByteBll.Base;
using PizzaByteDal;
using PizzaByteDto.Base;
using PizzaByteDto.RetornosRequisicoes;
using PizzaByteVo;
using PizzaByteVo.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using static PizzaByteEnum.Enumeradores;

namespace PizzaByteBll.Base
{
    public class BackupBll
    {
        private static LogBll logBll = new LogBll("BackupBll");
        private bool salvar = true;

        /// <summary>
        /// Iniciar com um novo contexto, indicando se deve salvar ou não as alterações
        /// </summary>
        /// <param name="salvarAlteracoes"></param>
        public BackupBll(bool salvarAlteracoes) : base(BackupBll)
        {
            salvar = salvarAlteracoes;
        }

        /// <summary>p
        /// Iniciar com um contexto existente, indicando se deve ou não salvar as alterações
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="salvarAlteracoes"></param>
        public BackupBll(PizzaByteContexto contexto, bool salvarAlteracoes) : base(logBll, contexto)
        {
            salvar = salvarAlteracoes;
        }

            // Se for mensagem do usuário, enviar alerta por email
          //  if (backupVo.Tipo == TipoMensagemBackup.Usuario)
        //    {
            //           string corpoEmail = $"<p> Há uma nova mensagem de backup!</p>" +
            //                      $"<p> Mensagem incluída as {backupVo.DataInclusao}:</p>" +
            //                      $"<p><strong>{backupVo.Mensagem}</strong></p><br/>" +
            //                      "<p> Entre com a senha de backup para responder à solicitação.</p>";

                // if (!UtilitarioBll.EnviarEmail("jlmanfrinato@hotmail.com", "Nova mensagem de backup", corpoEmail, ref mensagemErro)) 
                // {
                //     logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirBackup, backupVo.Id, $"Problemas para enviar a mensagem por email: {mensagemErro}");
                //}

              //  if (!UtilitarioBll.EnviarEmail("huxxley@hotmail.com.br", "Nova mensagem de backup", corpoEmail, ref mensagemErro))
              //  {
              //      logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirBackup, backupVo.Id, $"Problemas para enviar a mensagem por email: {mensagemErro}");
              //  }

                //                if (!UtilitarioBll.EnviarEmail("driramosbenite@gmail.com", "Nova mensagem de backup", corpoEmail, ref mensagemErro))
                // {
                // logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirBackup, backupVo.Id, $"Problemas para enviar a mensagem por email: {mensagemErro}");
                //}

                //if (!UtilitarioBll.EnviarEmail("barbaracocatosantos@gmail.com", "Nova mensagem de backup", corpoEmail, ref mensagemErro))
                //{
                //logBll.ResgistrarLog(requisicaoDto, LogRecursos.IncluirBackup, backupVo.Id, $"Problemas para enviar a mensagem por email: {mensagemErro}");
                //}
        //    }

       //     retornoDto.Retorno = true;
        //    retornoDto.Mensagem = "OK";
        //   return true;
        //}

       
        /// <summary>
        /// Obtém um backup pelo ID 
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool Obter(RequisicaoObterDto requisicaoDto, ref RetornoObterDto<BackupDto> retornoDto)
        {
            if (!base.Obter(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            BackupVo backupVo;

            if (!ObterPorIdBd(requisicaoDto.Id, out backupVo, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao obter o backup: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterBackup, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Mensagem = "Ok";
            if (backupVo == null)
            {
                retornoDto.Retorno = false;
                retornoDto.Mensagem = "Backup não encontrado";

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterBackup, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            BackupDto backupDto = new BackupDto();
            if (!ConverterVoParaDto(backupVo, ref backupDto, ref mensagemErro))
            {
                retornoDto.Mensagem = "Erro ao converter o backup: " + mensagemErro;
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterBackup, requisicaoDto.Id, retornoDto.Mensagem);
                return false;
            }

            retornoDto.Entidade = backupDto;
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Obtém uma lista de backups com filtros aplicados, podendo ser paginada
        /// </summary>
        /// <param name="requisicaoDto"></param>
        /// <param name="retornoDto"></param>
        /// <returns></returns>
        public override bool ObterListaFiltrada(RequisicaoObterListaDto requisicaoDto, ref RetornoObterListaDto<BackupDto> retornoDto)
        {
            if (!base.ObterListaFiltrada(requisicaoDto, ref retornoDto))
            {
                return false;
            }

            string mensagemErro = "";
            IQueryable<BackupVo> query;

            // Obter a query primária
            if (!this.ObterQueryBd(out query, ref mensagemErro))
            {
                retornoDto.Mensagem = $"Houve um problema ao listar as mensagens: {mensagemErro}";
                retornoDto.Retorno = false;

                logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaBackup, Guid.Empty, retornoDto.Mensagem);
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

                    default:
                        retornoDto.Mensagem = $"O filtro {filtro.Key} não está definido para esta pesquisa.";
                        retornoDto.Retorno = false;

                        logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaBackup, Guid.Empty, retornoDto.Mensagem);
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

            List<BackupVo> listaVo = query.ToList();
            foreach (var backup in listaVo)
            {
                BackupDto backupDto = new BackupDto();
                if (!ConverterVoParaDto(backup, ref backupDto, ref mensagemErro))
                {
                    retornoDto.Mensagem = "Erro ao converter para DTO: " + mensagemErro;
                    retornoDto.Retorno = false;

                    logBll.ResgistrarLog(requisicaoDto, LogRecursos.ObterListaBackup, backup.Id, retornoDto.Mensagem);
                    return false;
                }

                retornoDto.ListaEntidades.Add(backupDto);
            }

            retornoDto.Mensagem = "Ok";
            retornoDto.Retorno = true;
            return true;
        }

        /// <summary>
        /// Converte um backup Dto para um backup Vo
        /// </summary>
        /// <param name="backupDto"></param>
        /// <param name="backupVo"></param>
        /// <returns></returns>
        public override bool ConverterDtoParaVo(BackupDto backupDto, ref BackupVo backupVo, ref string mensagemErro)
        {
            if (!base.ConverterDtoParaVo(backupDto, ref backupVo, ref mensagemErro))
            {
                return false;
            }

            try
            {
                backupVo.Mensagem = string.IsNullOrWhiteSpace(backupDto.Mensagem) ? "" : backupDto.Mensagem.Trim();
                backupVo.Tipo = backupDto.Tipo;

                return true;
            }
            catch (Exception ex)
            {
                mensagemErro = "Erro ao converter a mensagem para Vo: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Converte um backup Dto para um backup Vo
        /// </summary>
        /// <param name="backupVo"></param>
        /// <param name="backupDto"></param>
        /// <returns></returns>
        public override bool ConverterVoParaDto(BackupVo backupVo, ref BackupDto backupDto, ref string mensagemErro)
        {
            if (!base.ConverterVoParaDto(backupVo, ref backupDto, ref mensagemErro))
            {
                return false;
            }

            try
            {
                backupDto.Mensagem = string.IsNullOrWhiteSpace(backupVo.Mensagem) ? "" : backupVo.Mensagem.Trim();
                backupDto.Tipo = backupVo.Tipo;

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
